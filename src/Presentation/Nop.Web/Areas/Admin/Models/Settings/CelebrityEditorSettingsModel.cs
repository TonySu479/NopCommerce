using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a celebrity editor settings model
    /// </summary>
    public partial class CelebrityEditorSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.CelebrityEditor.CelebrityTags")]
        public bool CelebrityTags { get; set; }

        #endregion
    }
}