using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a celebrity cache event consumer
    /// </summary>
    public partial class CelebrityCacheEventConsumer : CacheEventConsumer<Celebrity>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Celebrity entity)
        {
            RemoveByPrefix(NopCatalogDefaults.ProductTagPrefixCacheKey);
        }
    }
}
