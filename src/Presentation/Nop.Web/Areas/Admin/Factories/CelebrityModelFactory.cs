using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
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
        private readonly ICelebrityTagService _celebrityTagService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        #region Ctor
        public CelebrityModelFactory(
            CatalogSettings catalogSettings,
            ICelebrityService celebrityService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IPictureService pictureService,
            ICelebrityTagService celebrityTagService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory
            )
        {
            _catalogSettings = catalogSettings;
            _celebrityService = celebrityService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _pictureService = pictureService;
            _celebrityTagService = celebrityTagService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
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


            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare grid
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
            var celebrities = _celebrityService.SearchCelebrities(storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new CelebrityListModel().PrepareToGrid(searchModel, celebrities, () =>
            {
                return celebrities.Select(celebrity =>
                {
                    //fill in model values from the entity
                    var celebrityModel = celebrity.ToModel<CelebrityModel>();

                    //fill in additional values (not existing in the entity)
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

                model.CelebrityTags = string.Join(", ", _celebrityTagService.GetAllCelebrityTagsByCelebrityId(celebrity.Id).Select(tag => tag.Name));

                //prepare localized models
                if (!excludeProperties)
                    model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                PrepareCelebrityPictureSearchModel(model.CelebrityPictureSearchModel, celebrity);

            }

            //set default values for the new model
            if (celebrity == null)
            {
                
            }

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, celebrity, excludeProperties);

            var celebrityTags = _celebrityTagService.GetAllCelebrityTags();
            var celebrityTagsSb = new StringBuilder();
            celebrityTagsSb.Append("var initialCelebrityTags = [");
            for (var i = 0; i < celebrityTags.Count; i++)
            {
                var tag = celebrityTags[i];
                celebrityTagsSb.Append("'");
                celebrityTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
                celebrityTagsSb.Append("'");
                if (i != celebrityTags.Count - 1)
                    celebrityTagsSb.Append(",");
            }
            celebrityTagsSb.Append("]");

            model.InitialCelebrityTags = celebrityTagsSb.ToString();


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

        /// <summary>
        /// Prepare celebrity tag search model
        /// </summary>
        /// <param name="searchModel">Celebrity tag search model</param>
        /// <returns>Celebrity tag search model</returns>
        public virtual CelebrityTagSearchModel PrepareCelebrityTagSearchModel(CelebrityTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged celebrity tag list model
        /// </summary>
        /// <param name="searchModel">Celebrity tag search model</param>
        /// <returns>Celebrity tag list model</returns>
        public virtual CelebrityTagListModel PrepareCelebrityTagListModel(CelebrityTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get celebrity tags
            var celebrityTags = _celebrityTagService.GetAllCelebrityTags(tagName: searchModel.SearchTagName)
                .OrderByDescending(tag => _celebrityTagService.GetCelebrityCount(tag.Id, storeId: 0)).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new CelebrityTagListModel().PrepareToGrid(searchModel, celebrityTags, () =>
            {
                return celebrityTags.Select(tag =>
                {
                    //fill in model values from the entity
                    var celebrityTagModel = tag.ToModel<CelebrityTagModel>();

                    //fill in additional values (not existing in the entity)
                    celebrityTagModel.CelebrityCount = _celebrityTagService.GetCelebrityCount(tag.Id, storeId: 0);

                    return celebrityTagModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare celebrity tag model
        /// </summary>
        /// <param name="model">Celebrity tag model</param>
        /// <param name="celebrityTag">Celebrity tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Celebrity tag model</returns>
        public virtual CelebrityTagModel PrepareCelebrityTagModel(CelebrityTagModel model, CelebrityTag celebrityTag, bool excludeProperties = false)
        {
            Action<CelebrityTagLocalizedModel, int> localizedModelConfiguration = null;

            if (celebrityTag != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = celebrityTag.ToModel<CelebrityTagModel>();
                }

                model.CelebrityCount = _celebrityTagService.GetCelebrityCount(celebrityTag.Id, storeId: 0);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(celebrityTag, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

    }
}
