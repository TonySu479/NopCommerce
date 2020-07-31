using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;


namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a celebrity search model
    /// </summary>
    public partial class CelebritySearchModel : BaseSearchModel
    {
        #region Ctor

        public CelebritySearchModel()
        {

        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Celebrity.List.SearchCelebrityName")]
        public string SearchCelebrityName { get; set; }

        #endregion
    }
}