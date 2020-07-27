using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a celebrity tag model
    /// </summary>
    public partial class CelebrityTagModel : BaseNopEntityModel, ILocalizedModel<CelebrityTagLocalizedModel>
    {
        #region Ctor

        public CelebrityTagModel()
        {
            Locales = new List<CelebrityTagLocalizedModel>();
        }
        
        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.CelebrityTags.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.CelebrityTags.Fields.CelebrityCount")]
        public int CelebrityCount { get; set; }

        public IList<CelebrityTagLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class CelebrityTagLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.CelebrityTags.Fields.Name")]
        public string Name { get; set; }
    }
}