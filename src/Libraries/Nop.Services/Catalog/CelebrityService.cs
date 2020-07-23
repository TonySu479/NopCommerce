using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    public class CelebrityService : ICelebrityService
    {
        private readonly IRepository<Celebrity> _celebrityRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IRepository<CelebrityPicture> _celebrityPictureRepository;
        #region Ctor

        public CelebrityService(IRepository<Celebrity> celebrityRepository, IEventPublisher eventPublisher, ICacheKeyService cacheKeyService,
          IRepository<CelebrityPicture> celebrityPictureRepository
)
        {
            _celebrityRepository = celebrityRepository;
            _eventPublisher = eventPublisher;
            _cacheKeyService = cacheKeyService;
            _celebrityPictureRepository = celebrityPictureRepository;
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

            var allCelebrities = query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagAllCacheKey));

            if (!string.IsNullOrEmpty(celebrityName))
            {
                allCelebrities = allCelebrities.Where(tag => tag.Name.Contains(celebrityName)).ToList();
            }

            return allCelebrities;
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

        #endregion
    }

}
