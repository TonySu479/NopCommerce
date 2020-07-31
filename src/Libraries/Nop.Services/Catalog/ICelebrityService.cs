using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public interface ICelebrityService
    {
        void DeleteCelebrities(IList<Celebrity> celebrities);
        void DeleteCelebrity(Celebrity celebrity);
        void DeleteCelebrityPicture(CelebrityPicture celebrityPicture);
        IList<Celebrity> GetAllCelebrities(string celebrityName = null);
        IList<Celebrity> GetCelebritiesByIds(int[] celebrityIds);
        IList<Celebrity> GetCelebritiesByName(string name);
        Celebrity GetCelebrityById(int celebrityId);
        CelebrityPicture GetCelebrityPictureById(int celebrityPictureId);
        IList<CelebrityPicture> GetCelebrityPicturesByCelebrityId(int celebrityId);
        void InsertCelebrity(Celebrity celebrity);
        void InsertCelebrityPicture(CelebrityPicture celebrityPicture);
        IPagedList<Celebrity> SearchCelebrities(int pageIndex = 0, int pageSize = int.MaxValue, int storeId = 0, int celebrityTagId = 0, bool searchCelebrityTags = false, int languageId = 0, ProductSortingEnum orderBy = ProductSortingEnum.Position);
        void UpdateCelebrity(Celebrity celebrity);
        void UpdateCelebrityPicture(CelebrityPicture celebrityPicture);
        void UpdateCelebrityStoreMappings(Celebrity celebrity, IList<int> limitedToStoresIds);
    }
}