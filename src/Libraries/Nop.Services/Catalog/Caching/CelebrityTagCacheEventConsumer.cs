using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a celebrity tag cache event consumer
    /// </summary>
    public partial class CelebrityTagCacheEventConsumer : CacheEventConsumer<CelebrityTag>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CelebrityTag entity)
        {
            RemoveByPrefix(NopCatalogDefaults.CelebrityTagPrefixCacheKey);
        }
    }
}
