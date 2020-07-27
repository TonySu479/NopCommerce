using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a celebrity tag search model
    /// </summary>
    public partial class CelebrityTagSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Catalog.CelebrityTags.Fields.SearchTagName")]
        public string SearchTagName { get; set; }
    }
}