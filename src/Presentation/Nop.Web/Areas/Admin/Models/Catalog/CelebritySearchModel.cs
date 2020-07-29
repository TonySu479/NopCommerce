using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Celebrity.List.SearchCelebrityName")]
        public string SearchCelebrityName { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public bool HideStoresList { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Celebrities.List.SearchStore")]
        public int SearchStoreId { get; set; }
        #endregion
    }
}
