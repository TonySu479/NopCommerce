using System.Collections.Generic;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class CelebrityModel : BaseNopEntityModel, ILocalizedModel<CelebrityLocalizedModel>
    {
        #region Ctor

        public CelebrityModel()
        {
            CelebrityPictureModels = new List<CelebrityPictureModel>();
            Locales = new List<CelebrityLocalizedModel>();
            AddPictureModel = new CelebrityPictureModel();
            CelebrityPictureSearchModel = new CelebrityPictureSearchModel();
            CelebrityEditorSettingsModel = new CelebrityEditorSettingsModel();
        }

        #endregion

        #region Properties

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.CelebrityTags")]
        public string CelebrityTags { get; set; }
        public string InitialCelebrityTags { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.ShowOnHomepage")]
        public bool ShowOnHomepage { get; set; }

        //[NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.ProductCount")]
        //public int ProductCount { get; set; }

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
    }
}
