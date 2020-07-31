using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    public class CelebrityService : ICelebrityService
    {
        private readonly IRepository<Celebrity> _celebrityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IRepository<CelebrityPicture> _celebrityPictureRepository;
        private readonly CommonSettings _commonSettings;
        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly CatalogSettings _catalogSettings;
        #region Ctor

        public CelebrityService(IRepository<Celebrity> celebrityRepository, IEventPublisher eventPublisher, ICacheKeyService cacheKeyService,
          IRepository<CelebrityPicture> celebrityPictureRepository,
          CommonSettings commonSettings,
          ILanguageService languageService,
          IStoreMappingService storeMappingService,
          IStoreService storeService,
          CatalogSettings catalogSettings
)
        {
            _celebrityRepository = celebrityRepository;
            _eventPublisher = eventPublisher;
            _cacheKeyService = cacheKeyService;
            _celebrityPictureRepository = celebrityPictureRepository;
            _commonSettings = commonSettings;
            _languageService = languageService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _catalogSettings = catalogSettings;
        }

        #endregion
        #region Methods

        /// <summary>
        /// Delete a celebrity
        /// </summary>
        /// <param name="celebrity">celebrity</param>
        public virtual void DeleteCelebrity(Celebrity celebrity)
        {
            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            _celebrityRepository.Delete(celebrity);

            //event notification
            _eventPublisher.EntityDeleted(celebrity);
        }

        /// <summary>
        /// Delete celebrities
        /// </summary>
        /// <param name="celebrities">Celebrities</param>
        public virtual void DeleteCelebrities(IList<Celebrity> celebrities)
        {
            if (celebrities == null)
                throw new ArgumentNullException(nameof(celebrities));

            foreach (var celebrity in celebrities)
            {
                DeleteCelebrity(celebrity);
            }
        }

        /// <summary>
        /// Gets all celebrities
        /// </summary>
        /// <param name="celebrityName">Tag name</param>
        /// <returns>Celebrities</returns>
        public virtual IList<Celebrity> GetAllCelebrities(string celebrityName = null)
        {
            var query = _celebrityRepository.Table;

            var allCelebrities = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CelebrityTagAllCacheKey));

            if (!string.IsNullOrEmpty(celebrityName))
            {
                allCelebrities = allCelebrities.Where(tag => tag.Name.Contains(celebrityName)).ToList();
            }

            return allCelebrities;
        }

        ///// <summary>
        ///// Search celebrities
        ///// </summary>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageSize">Page size</param>
        ///// <param name="celebrityTagId">Celebrity tag identifier; 0 to load all records</param>
        ///// <param name="keywords">Keywords</param>
        ///// <param name="searchCelebrityTags">A value indicating whether to search by a specified "keyword" in celebrity tags</param>
        ///// <param name="languageId">Language identifier (search for text searching)</param>
        ///// </param>
        ///// <returns>Celebrities</returns>
        //public virtual IPagedList<Product> SearchCelebrities(
        //    int pageIndex = 0,
        //    int pageSize = int.MaxValue,
        //    int celebrityTagId = 0,
        //    string keywords = null,
        //    bool searchCelebrityTags = false,
        //    int languageId = 0,
        //    ProductSortingEnum orderBy = ProductSortingEnum.Position,
        //    bool showHidden = false
        //    )
        //{
        //    return SearchCelebrities(pageIndex, pageSize, celebrityTagId, keywords, searchCelebrityTags, languageId, orderBy, showHidden);
        //}

        /// <summary>
        /// Search celebrities
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="celebrityTagId">Celebrity tag identifier; 0 to load all records</param>
        /// <param name="searchCelebrityTags">A value indicating whether to search by a specified "keyword" in celebrity tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="orderBy">Order by</param>
        /// </param>
        /// <returns>Celebrities</returns>
        public virtual IPagedList<Celebrity> SearchCelebrities(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            int storeId = 0,
            int celebrityTagId = 0,
            bool searchCelebrityTags = false,
            int languageId = 0,
            ProductSortingEnum orderBy = ProductSortingEnum.Position
)
        {
            //search by keyword
            var searchLocalizedValue = false;
            if (languageId > 0)
            {
                    //ensure that we have at least two published languages
                    var totalPublishedLanguages = _languageService.GetAllLanguages().Count;
                    searchLocalizedValue = totalPublishedLanguages >= 2;
            }

            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            //prepare input parameters
            var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", !_catalogSettings.IgnoreStoreLimitations ? storeId : 0);
            var pCelebrityTagId = SqlParameterHelper.GetInt32Parameter("CelebrityTagId", celebrityTagId);
            var pSearchCelebrityTags = SqlParameterHelper.GetBooleanParameter("SearchCelebrityTags", searchCelebrityTags);
            var pUseFullTextSearch = SqlParameterHelper.GetBooleanParameter("UseFullTextSearch", _commonSettings.UseFullTextSearch);
            var pFullTextMode = SqlParameterHelper.GetInt32Parameter("FullTextMode", (int)_commonSettings.FullTextMode);
            var pLanguageId = SqlParameterHelper.GetInt32Parameter("LanguageId", searchLocalizedValue ? languageId : 0);
            var pOrderBy = SqlParameterHelper.GetInt32Parameter("OrderBy", (int)orderBy);
            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);

            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var celebrities = _celebrityRepository.EntityFromSql("CelebrityLoadAllPaged",
                pStoreId,
                pCelebrityTagId,
                pSearchCelebrityTags,
                pUseFullTextSearch,
                pFullTextMode,
                pLanguageId,
                pOrderBy,
                pPageIndex,
                pPageSize,
                pTotalRecords).ToList();


            //return celebrities
            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;

            return new PagedList<Celebrity>(celebrities, pageIndex, pageSize, totalRecords);
        }

        /// <summary>
        /// Gets celebrity
        /// </summary>
        /// <param name="celebrityId">Celebrity tag identifier</param>
        /// <returns>Celebrity</returns>
        public virtual Celebrity GetCelebrityById(int celebrityId)
        {
            if (celebrityId == 0)
                return null;

            return _celebrityRepository.ToCachedGetById(celebrityId);
        }

        /// <summary>
        /// Gets celebrities
        /// </summary>
        /// <param name="celebrityIds">celebrities identifiers</param>
        /// <returns>Celebrities</returns>
        public virtual IList<Celebrity> GetCelebritiesByIds(int[] celebrityIds)
        {
            if (celebrityIds == null || celebrityIds.Length == 0)
                return new List<Celebrity>();

            var query = from p in _celebrityRepository.Table
                        where celebrityIds.Contains(p.Id)
                        select p;

            return query.ToList();
        }

        /// <summary>
        /// Gets celebrity by name
        /// </summary>
        /// <param name="name">Celebrities name</param>
        /// <returns>Celebrities</returns>
        public virtual IList<Celebrity> GetCelebritiesByName(string name)
        {
            var query = from pt in _celebrityRepository.Table
                        where pt.Name == name
                        select pt;

            var celebrities = query.ToList();
            return celebrities;
        }

        /// <summary>
        /// Inserts a celebrity
        /// </summary>
        /// <param name="celebrity">Celebrity</param>
        public virtual void InsertCelebrity(Celebrity celebrity)
        {
            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            _celebrityRepository.Insert(celebrity);

            //event notification
            _eventPublisher.EntityInserted(celebrity);
        }

        /// <summary>
        /// Updates the celebrity
        /// </summary>
        /// <param name="celebrity">Celebrity</param>
        public virtual void UpdateCelebrity(Celebrity celebrity)
        {
            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            //update
            _celebrityRepository.Update(celebrity);

            //event notification
            _eventPublisher.EntityUpdated(celebrity);
        }

        /// <summary>
        /// Gets a celebrity pictures by celebrity identifier
        /// </summary>
        /// <param name="celebrityId">The celebrity identifier</param>
        /// <returns>Celebrity pictures</returns>
        public virtual IList<CelebrityPicture> GetCelebrityPicturesByCelebrityId(int celebrityId)
        {
            var query = from pp in _celebrityPictureRepository.Table
                        where pp.CelebrityId == celebrityId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;

            var celebrityPictures = query.ToList();

            return celebrityPictures;
        }
        /// <summary>
        /// Gets a celebrity picture
        /// </summary>
        /// <param name="celebrityPictureId">Celebrity picture identifier</param>
        /// <returns>Celebrity picture</returns>
        public virtual CelebrityPicture GetCelebrityPictureById(int celebrityPictureId)
        {
            if (celebrityPictureId == 0)
                return null;

            return _celebrityPictureRepository.ToCachedGetById(celebrityPictureId);
        }

        /// <summary>
        /// Updates a celebrity picture
        /// </summary>
        /// <param name="celebrityPicture">Celebrity picture</param>
        public virtual void UpdateCelebrityPicture(CelebrityPicture celebrityPicture)
        {
            if (celebrityPicture == null)
                throw new ArgumentNullException(nameof(celebrityPicture));

            _celebrityPictureRepository.Update(celebrityPicture);

            //event notification
            _eventPublisher.EntityUpdated(celebrityPicture);
        }

        /// <summary>
        /// Deletes a celebrity picture
        /// </summary>
        /// <param name="celebrityPicture">Celebrity picture</param>
        public virtual void DeleteCelebrityPicture(CelebrityPicture celebrityPicture)
        {
            if (celebrityPicture == null)
                throw new ArgumentNullException(nameof(celebrityPicture));

            _celebrityPictureRepository.Delete(celebrityPicture);

            //event notification
            _eventPublisher.EntityDeleted(celebrityPicture);
        }

        /// <summary>
        /// Inserts a celebrity picture
        /// </summary>
        /// <param name="celebrityPicture">Celebrity picture</param>
        public virtual void InsertCelebrityPicture(CelebrityPicture celebrityPicture)
        {
            if (celebrityPicture == null)
                throw new ArgumentNullException(nameof(celebrityPicture));

            _celebrityPictureRepository.Insert(celebrityPicture);

            //event notification
            _eventPublisher.EntityInserted(celebrityPicture);
        }

        /// <summary>
        /// Update celebrity store mappings
        /// </summary>
        /// <param name="celebrity">Celebrity</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        public virtual void UpdateCelebrityStoreMappings(Celebrity celebrity, IList<int> limitedToStoresIds)
        {
            celebrity.LimitedToStores = limitedToStoresIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(celebrity);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (limitedToStoresIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(celebrity, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
    }

}
