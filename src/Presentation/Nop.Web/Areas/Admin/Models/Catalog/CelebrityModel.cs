using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class CelebrityModel : BaseNopEntityModel, ILocalizedModel<CelebrityLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public CelebrityModel()
        {
            CelebrityPictureModels = new List<CelebrityPictureModel>();
            Locales = new List<CelebrityLocalizedModel>();
            AddPictureModel = new CelebrityPictureModel();
            CelebrityPictureSearchModel = new CelebrityPictureSearchModel();
            CelebrityEditorSettingsModel = new CelebrityEditorSettingsModel();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.FullDescription")]
        public string FullDescription { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.CelebrityTags")]

        public string CelebrityTags { get; set; }
        public string InitialCelebrityTags { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.ShowOnHomepage")]
        public bool ShowOnHomepage { get; set; }


        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<CelebrityLocalizedModel> Locales { get; set; }

        //pictures
        public CelebrityPictureModel AddPictureModel { get; set; }
        public IList<CelebrityPictureModel> CelebrityPictureModels { get; set; }
        public CelebrityPictureSearchModel CelebrityPictureSearchModel { get; set; }
        public CelebrityEditorSettingsModel CelebrityEditorSettingsModel { get; set; }
        #endregion
    }

    public partial class CelebrityLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.ShortDescription")]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.FullDescription")]
        public string FullDescription { get; set; }
    }
}
