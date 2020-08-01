using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Catalog
{

    /// <summary>
    /// Represents a celebrity
    /// </summary>
    public partial class Celebrity : BaseEntity, ILocalizedEntity, ISlugSupported, IStoreMappingSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string  Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }
        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string FullDescription { get; set; }
    }
}
