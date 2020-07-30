using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public interface ICelebrityTagService
    {
        void DeleteCelebrityTag(CelebrityTag celebrityTag);
        void DeleteCelebrityTags(IList<CelebrityTag> celebrityTags);
        IList<CelebrityTag> GetAllCelebrityTags(string tagName = null);
        IList<CelebrityTag> GetAllCelebrityTagsByCelebrityId(int celebrityId);
        int GetCelebrityCount(int celebrityTagId, int storeId);
        CelebrityTag GetCelebrityTagById(int celebrityTagId);
        IList<CelebrityTag> GetCelebrityTagsByIds(int[] celebrityTagIds);
        void InsertCelebrityCelebrityTagMapping(CelebrityCelebrityTagMapping tagMapping);
        void UpdateCelebrityTag(CelebrityTag celebrityTag);
        void UpdateCelebrityTags(Celebrity celebrity, string[] celebrityTags);
    }
}