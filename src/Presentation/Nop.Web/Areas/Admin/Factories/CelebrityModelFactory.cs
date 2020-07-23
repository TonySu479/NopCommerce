using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{

    public class CelebrityModelFactory : ICelebrityModelFactory
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICelebrityService _celebrityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IPictureService _pictureService;
        #region Ctor
        public CelebrityModelFactory(
            CatalogSettings catalogSettings,
            ICelebrityService celebrityService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IPictureService pictureService
            )
        {
            _catalogSettings = catalogSettings;
            _celebrityService = celebrityService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _pictureService = pictureService;
        }
        #endregion
        /// <summary>
        /// Prepare Celebrity search model
        /// </summary>
        /// <param name="searchModel">Celebrity search model</param>
        /// <returns>Celebrity search model</returns>
        public virtual CelebritySearchModel PrepareCelebritySearchModel(CelebritySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        /// <summary>
        /// Prepare paged celebrity list model
        /// </summary>
        /// <param name="searchModel">Celebrity search model</param>
        /// <returns>Celebrity list model</returns>
        public virtual CelebrityListModel PrepareCelebrityListModel(CelebritySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get celebrities
            var celebrities = _celebrityService.GetAllCelebrities(celebrityName: searchModel.SearchCelebrityName)
                .ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new CelebrityListModel().PrepareToGrid(searchModel, celebrities, () =>
            {
                return celebrities.Select(celebrity =>
                {
                    //fill in model values from the entity
                    var celebrityModel = celebrity.ToModel<CelebrityModel>();
                    var defaultCelebrityPicture = _pictureService.GetPicturesByCelebrityId(celebrity.Id, 1).FirstOrDefault();
                    celebrityModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(ref defaultCelebrityPicture, 75);

                    return celebrityModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare celebrity picture search model
        /// </summary>
        /// <param name="searchModel">Celebrity picture search model</param>
        /// <param name="celebrity">Celebrity</param>
        /// <returns>Celebrity picture search model</returns>
        protected virtual CelebrityPictureSearchModel PrepareCelebritySearchModel(CelebrityPictureSearchModel searchModel, Celebrity celebrity)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            searchModel.CelebrityId = celebrity.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
        /// <summary>
        /// Prepare celebrity model
        /// </summary>
        /// <param name="model">Celebrity model</param>
        /// <param name="celebrity">Celebrity</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Celebrity model</returns>
        public virtual CelebrityModel PrepareCelebrityModel(CelebrityModel model, Celebrity celebrity, bool excludeProperties = false)
        {
            Action<CelebrityLocalizedModel, int> localizedModelConfiguration = null;

            if (celebrity != null)
            {
                model = celebrity.ToModel<CelebrityModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(celebrity, entity => entity.Name, languageId, false, false);
                };

                //prepare localized models
                if (!excludeProperties)
                    model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                PrepareCelebrityPictureSearchModel(model.CelebrityPictureSearchModel, celebrity);
            }

            //set default values for the new model
            if (celebrity == null)
            {
                
            }

            return model;
        }

        /// <summary>
        /// Prepare paged celebrity picture list model
        /// </summary>
        /// <param name="searchModel">Celebrity picture search model</param>
        /// <param name="celebrity">Celebrity</param>
        /// <returns>Celebrity picture list model</returns>
        public virtual CelebrityPictureListModel PrepareCelebrityPictureListModel(CelebrityPictureSearchModel searchModel, Celebrity celebrity)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            //get celebrity pictures
            var celebrityPictures = _celebrityService.GetCelebrityPicturesByCelebrityId(celebrity.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new CelebrityPictureListModel().PrepareToGrid(searchModel, celebrityPictures, () =>
            {
                return celebrityPictures.Select(celebrityPicture =>
                {
                    //fill in model values from the entity
                    var celebrityPictureModel = celebrityPicture.ToModel<CelebrityPictureModel>();

                    //fill in additional values (not existing in the entity)
                    var picture = _pictureService.GetPictureById(celebrityPicture.PictureId)
                                  ?? throw new Exception("Picture cannot be loaded");

                    celebrityPictureModel.PictureUrl = _pictureService.GetPictureUrl(ref picture);
                    celebrityPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    celebrityPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return celebrityPictureModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare celebrity picture search model
        /// </summary>
        /// <param name="searchModel">Celebrity picture search model</param>
        /// <param name="celebrity">Celebrity</param>
        /// <returns>Celebrity picture search model</returns>
        protected virtual CelebrityPictureSearchModel PrepareCelebrityPictureSearchModel(CelebrityPictureSearchModel searchModel, Celebrity celebrity)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (celebrity == null)
                throw new ArgumentNullException(nameof(celebrity));

            searchModel.CelebrityId = celebrity.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }
    }
}
