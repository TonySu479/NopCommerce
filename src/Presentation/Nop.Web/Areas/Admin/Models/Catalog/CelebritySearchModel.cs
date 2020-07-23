using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;


namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a celebrity search model
    /// </summary>
    public partial class CelebritySearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Catalog.Celebrities.Fields.SearchTagName")]
        public string SearchCelebrityName { get; set; }
    }
}
