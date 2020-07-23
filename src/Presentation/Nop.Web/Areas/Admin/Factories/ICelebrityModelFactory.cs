using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public interface ICelebrityModelFactory
    {
        CelebrityListModel PrepareCelebrityListModel(CelebritySearchModel searchModel);
        CelebrityModel PrepareCelebrityModel(CelebrityModel model, Celebrity celebrity, bool excludeProperties = false);
        CelebrityPictureListModel PrepareCelebrityPictureListModel(CelebrityPictureSearchModel searchModel, Celebrity celebrity);
        CelebritySearchModel PrepareCelebritySearchModel(CelebritySearchModel searchModel);

    }
}