using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Celebrity tag service
    /// </summary>
    public partial class CelebrityTagService : ICelebrityTagService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICustomerService _customerService;
        private readonly INopDataProvider _dataProvider;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CelebrityCelebrityTagMapping> _celebrityCelebrityTagMappingRepository;
        private readonly IRepository<CelebrityTag> _celebrityTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CelebrityTagService(CatalogSettings catalogSettings,
            ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IEventPublisher eventPublisher,
            IRepository<CelebrityCelebrityTagMapping> celebrityCelebrityTagMappingRepository,
            IRepository<CelebrityTag> celebrityTagRepository,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _eventPublisher = eventPublisher;
            _celebrityCelebrityTagMappingRepository = celebrityCelebrityTagMappingRepository;
            _celebrityTagRepository = celebrityTagRepository;
            _staticCacheManager = staticCacheManager;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Delete a celebrity-celebrity tag mapping
        /// </summary>
        /// <param name="celebrityId">Celebrity identifier</param>
        /// <param name="CelebrityTagId">Celebrity tag identifier</param>
        public virtual void DeleteCelebrityCelebrityTagMapping(int celebrityId, int celebrityTagId)
        {
            var mappitngRecord = _celebrityCelebrityTagMappingRepository.Table.FirstOrDefault(pptm => pptm.CelebrityId == celebrityId && pptm.CelebrityTagId == celebrityTagId);

            if (mappitngRecord is null)
                throw new Exception("Mppaing record not found");

            _celebrityCelebrityTagMappingRepository.Delete(mappitngRecord);

            //event notification
            _eventPublisher.EntityDeleted(mappitngRecord);
        }

        /// <summary>
        /// Get celebrity count for each of existing celebrity tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Dictionary of "celebrity tag ID : celebrity count"</returns>
        private Dictionary<int, int> GetCelebrityCount(int storeId, bool showHidden)
        {
            var allowedCustomerRolesIds = string.Empty;
            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                //pass customer role identifiers as comma-delimited string
                allowedCustomerRolesIds = string.Join(",", _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer));
            }

            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CelebrityTagCountCacheKey, storeId, 
                _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer), 
                showHidden);
           
            return _staticCacheManager.Get(key, () =>
            {
                //prepare input parameters
                var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", storeId);
                var pAllowedCustomerRoleIds = SqlParameterHelper.GetStringParameter("AllowedCustomerRoleIds", allowedCustomerRolesIds);

                //invoke stored procedure
                return _dataProvider.QueryProc<CelebrityTagWithCount>("CelebrityTagCountLoadAll",
                        pStoreId,
                        pAllowedCustomerRoleIds)
                    .ToDictionary(item => item.CelebrityTagId, item => item.CelebrityCount);
            });
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Delete a celebrity tag
        /// </summary>
        /// <param name="celebrityTag">Celebrity tag</param>
        public virtual void DeleteCelebrityTag(CelebrityTag celebrityTag)
        {
            if (celebrityTag == null)
                throw new ArgumentNullException(nameof(celebrityTag));

            _celebrityTagRepository.Delete(celebrityTag);
            
            //event notification
            _eventPublisher.EntityDeleted(celebrityTag);
        }

        /// <summary>
        /// Delete celebrity tags
        /// </summary>
        /// <param name="celebrityTags">Celebrity tags</param>
        public virtual void DeleteCelebrityTags(IList<CelebrityTag> celebrityTags)
        {
            if (celebrityTags == null)
                throw new ArgumentNullException(nameof(celebrityTags));

            foreach (var celebrityTag in celebrityTags)
            {
                DeleteCelebrityTag(celebrityTag);
            }
        }

        /// <summary>
        /// Gets all celebrity tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>Celebrity tags</returns>
        public virtual IList<CelebrityTag> GetAllCelebrityTags(string tagName = null)
        {
            var query = _celebrityTagRepository.Table;
            
            var allCelebrityTags = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CelebrityTagAllCacheKey));

            if(!string.IsNullOrEmpty(tagName))
            {
                allCelebrityTags = allCelebrityTags.Where(tag => tag.Name.Contains(tagName)).ToList();
            }
            
            return allCelebrityTags;
        }

        /// <summary>
        /// Gets all celebrity tags by celebrity identifier
        /// </summary>
        /// <param name="celebrityId">Celebrity identifier</param>
        /// <returns>Celebrity tags</returns>
        public virtual IList<CelebrityTag> GetAllCelebrityTagsByCelebrityId(int celebrityId)
        {
            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.CelebrityTagAllByCelebrityIdCacheKey, celebrityId);

            var query = from pt in _celebrityTagRepository.Table
                join ppt in _celebrityCelebrityTagMappingRepository.Table on pt.Id equals ppt.CelebrityTagId
                where ppt.CelebrityId == celebrityId
                orderby pt.Id
                select pt;

            var celebrityTags = query.ToCachedList(key);

            return celebrityTags;
        }

        /// <summary>
        /// Gets celebrity tag
        /// </summary>
        /// <param name="celebrityTagId">Celebrity tag identifier</param>
        /// <returns>Celebrity tag</returns>
        public virtual CelebrityTag GetCelebrityTagById(int celebrityTagId)
        {
            if (celebrityTagId == 0)
                return null;

            return _celebrityTagRepository.ToCachedGetById(celebrityTagId);
        }

        /// <summary>
        /// Gets celebrity tags
        /// </summary>
        /// <param name="celebrityTagIds">Celebrity tags identifiers</param>
        /// <returns>Celebrity tags</returns>
        public virtual IList<CelebrityTag> GetCelebrityTagsByIds(int[] celebrityTagIds)
        {
            if (celebrityTagIds == null || celebrityTagIds.Length == 0)
                return new List<CelebrityTag>();

            var query = from p in _celebrityTagRepository.Table
                        where celebrityTagIds.Contains(p.Id)
                        select p;

            return query.ToList();
        }

        /// <summary>
        /// Gets celebrity tag by name
        /// </summary>
        /// <param name="name">Celebrity tag name</param>
        /// <returns>Celebrity tag</returns>
        public virtual CelebrityTag GetCelebrityTagByName(string name)
        {
            var query = from pt in _celebrityTagRepository.Table
                        where pt.Name == name
                        select pt;

            var celebrityTag = query.FirstOrDefault();
            return celebrityTag;
        }

        /// <summary>
        /// Inserts a celebrity-celebrity tag mapping
        /// </summary>
        /// <param name="tagMapping">Celebrity-Celebrity tag mapping</param>
        public virtual void InsertCelebrityCelebrityTagMapping(CelebrityCelebrityTagMapping tagMapping)
        {
            if (tagMapping is null)
                throw new ArgumentNullException(nameof(tagMapping));

            _celebrityCelebrityTagMappingRepository.Insert(tagMapping);

            //event notification
            _eventPublisher.EntityInserted(tagMapping);
        }

        /// <summary>
        /// Inserts a celebrity tag
        /// </summary>
        /// <param name="celebrityTag">Celebrity tag</param>
        public virtual void InsertCelebrityTag(CelebrityTag celebrityTag)
        {
            if (celebrityTag == null)
                throw new ArgumentNullException(nameof(celebrityTag));

            _celebrityTagRepository.Insert(celebrityTag);
            
            //event notification
            _eventPublisher.EntityInserted(celebrityTag);
        }

        /// <summary>
        /// Indicates whether a celebrity tag exists
        /// </summary>
        /// <param name="celebrity">Celebrity</param>
        /// <param name="celebrityTagId">Celebrity tag identifier</param>
        /// <returns>Result</returns>
        public virtual bool CelebrityTagExists(Celebrity celebrity, int celebrityTagId)
        {
            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            return _celebrityCelebrityTagMappingRepository.Table.Any(pptm => pptm.CelebrityId == celebrity.Id && pptm.CelebrityTagId == celebrityTagId);
        }

        /// <summary>
        /// Updates the celebrity tag
        /// </summary>
        /// <param name="celebrityTag">Celebrity tag</param>
        public virtual void UpdateCelebrityTag(CelebrityTag celebrityTag)
        {
            if (celebrityTag == null)
                throw new ArgumentNullException(nameof(celebrityTag));

            _celebrityTagRepository.Update(celebrityTag);

            var seName = _urlRecordService.ValidateSeName(celebrityTag, string.Empty, celebrityTag.Name, true);
            _urlRecordService.SaveSlug(celebrityTag, seName, 0);
            
            //event notification
            _eventPublisher.EntityUpdated(celebrityTag);
        }

        /// <summary>
        /// Get number of celebrities
        /// </summary>
        /// <param name="celebrityTagId">Celebrity tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Number of celebrities</returns>
        public virtual int GetCelebrityCount(int celebrityTagId, int storeId, bool showHidden = false)
        {
            var dictionary = GetCelebrityCount(storeId, showHidden);
            if (dictionary.ContainsKey(celebrityTagId))
                return dictionary[celebrityTagId];

            return 0;
        }

        /// <summary>
        /// Update celebrity tags
        /// </summary>
        /// <param name="celebrity">Celebrity for update</param>
        /// <param name="celebrityTags">Celebrity tags</param>
        public virtual void UpdateCelebrityTags(Celebrity celebrity, string[] celebrityTags)
        {
            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            //celebrity tags
            var existingCelebrityTags = GetAllCelebrityTagsByCelebrityId(celebrity.Id);
            var celebrityTagsToRemove = new List<CelebrityTag>();
            foreach (var existingCelebrityTag in existingCelebrityTags)
            {
                var found = false;
                foreach (var newCelebrityTag in celebrityTags)
                {
                    if (!existingCelebrityTag.Name.Equals(newCelebrityTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                {
                    celebrityTagsToRemove.Add(existingCelebrityTag);
                }
            }

            foreach (var celebrityTag in celebrityTagsToRemove)
            {
                DeleteCelebrityCelebrityTagMapping(celebrity.Id, celebrityTag.Id);
            }

            foreach (var celebrityTagName in celebrityTags)
            {
                CelebrityTag celebrityTag;
                var celebrityTag2 = GetCelebrityTagByName(celebrityTagName);
                if (celebrityTag2 == null)
                {
                    //add new celebrity tag
                    celebrityTag = new CelebrityTag
                    {
                        Name = celebrityTagName
                    };
                    InsertCelebrityTag(celebrityTag);
                }
                else
                {
                    celebrityTag = celebrityTag2;
                }

                if (!CelebrityTagExists(celebrity, celebrityTag.Id))
                {
                    InsertCelebrityCelebrityTagMapping(new CelebrityCelebrityTagMapping { CelebrityTagId = celebrityTag.Id, CelebrityId = celebrity.Id });
                }

                var seName = _urlRecordService.ValidateSeName(celebrityTag, string.Empty, celebrityTag.Name, true);
                _urlRecordService.SaveSlug(celebrityTag, seName, 0);
            }

            //cache
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.CelebrityTagPrefixCacheKey);
        }

        #endregion

        #region MyRegion

        protected partial class CelebrityTagWithCount
        {
            /// <summary>
            /// Gets or sets the entity identifier
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the celebrity tag ID
            /// </summary>
            public int CelebrityTagId { get; set; }

            /// <summary>
            /// Gets or sets the count
            /// </summary>
            public int CelebrityCount { get; set; }
        }

        #endregion
    }
}