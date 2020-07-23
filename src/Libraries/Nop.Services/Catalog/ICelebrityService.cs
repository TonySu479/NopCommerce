using System.Collections.Generic;
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
        void UpdateCelebrity(Celebrity celebrity);
        void UpdateCelebrityPicture(CelebrityPicture celebrityPicture);
    }
}