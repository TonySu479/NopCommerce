using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a celebrity-celebrity tag mapping  cache event consumer
    /// </summary>
    public partial class CelebrityCelebrityTagMappingCacheEventConsumer : CacheEventConsumer<CelebrityCelebrityTagMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(CelebrityCelebrityTagMapping entity)
        {
            Remove(_cacheKeyService.PrepareKey(NopCatalogDefaults.CelebrityTagAllByCelebrityIdCacheKey, entity.CelebrityId));
        }
    }
}