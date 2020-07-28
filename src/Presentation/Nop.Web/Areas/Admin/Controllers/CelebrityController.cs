using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class CelebrityController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly ICelebrityModelFactory _celebrityModelFactory;
        private readonly ICelebrityService _celebrityService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly ICelebrityTagService _celebrityTagService;
        private readonly IUrlRecordService _urlRecordService;

        public CelebrityController(IPermissionService permissionService, ICelebrityModelFactory celebrityModelFactory,
          ICelebrityService celebrityService,
          ICustomerActivityService customerActivityService,
          ILocalizationService localizationService,
          INotificationService notificationService,
          ILocalizedEntityService localizedEntityService,
          IPictureService pictureService,
          ICelebrityTagService celebrityTagService,
          IUrlRecordService urlRecordService
          )
        {
            _permissionService = permissionService;
            _celebrityModelFactory = celebrityModelFactory;
            _celebrityService = celebrityService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _localizedEntityService = localizedEntityService;
            _pictureService = pictureService;
            _celebrityTagService = celebrityTagService;
            _urlRecordService = urlRecordService;
        }

        protected virtual void UpdateLocales(Celebrity celebrity,
            CelebrityModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(celebrity,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(CelebrityTag celebrityTag, CelebrityTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(celebrityTag,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                var seName = _urlRecordService.ValidateSeName(celebrityTag, string.Empty, localized.Name, false);
                _urlRecordService.SaveSlug(celebrityTag, seName, localized.LanguageId);
            }
        }

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            //prepare model
            var model = _celebrityModelFactory.PrepareCelebritySearchModel(new CelebritySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CelebritySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _celebrityModelFactory.PrepareCelebrityListModel(searchModel);


            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();


            //prepare model
            var model = _celebrityModelFactory.PrepareCelebrityModel(new CelebrityModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CelebrityModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //celebrity
                var celebrity = model.ToEntity<Celebrity>();
                _celebrityService.InsertCelebrity(celebrity);

                //locales
                UpdateLocales(celebrity, model);

                //tags
                _celebrityTagService.UpdateCelebrityTags(celebrity, ParseCelebrityTags(model.CelebrityTags));

                //activity log
                _customerActivityService.InsertActivity("AddNewCelebrity",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCelebrity"), celebrity.Name), celebrity);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Celebrities.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = celebrity.Id });
            }

            //prepare model
            model = _celebrityModelFactory.PrepareCelebrityModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            //try to get a celebrity with the specified id
            var celebrity = _celebrityService.GetCelebrityById(id);
            if (celebrity == null)
                return RedirectToAction("List");

            //prepare model
            var model = _celebrityModelFactory.PrepareCelebrityModel(null, celebrity);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CelebrityModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            //try to get a celebrity with the specified id
            var celebrity = _celebrityService.GetCelebrityById(model.Id);
            if (celebrity == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //locales
                UpdateLocales(celebrity, model);

                //tags
                _celebrityTagService.UpdateCelebrityTags(celebrity, ParseCelebrityTags(model.CelebrityTags));

                //celebrity
                celebrity = model.ToEntity(celebrity);
                _celebrityService.UpdateCelebrity(celebrity);

                //activity log
                _customerActivityService.InsertActivity("EditCelebrity",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCelebrity"), celebrity.Name), celebrity);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Celebrity.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = celebrity.Id });
            }

            //prepare model
            model = _celebrityModelFactory.PrepareCelebrityModel(model, celebrity, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            //try to get a celebrity with the specified id
            var celebrity  = _celebrityService.GetCelebrityById(id);
            if (celebrity == null)
                return RedirectToAction("List");

            _celebrityService.DeleteCelebrity(celebrity);
            
            //activity log
            _customerActivityService.InsertActivity("DeleteCelebrity",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCelebrity"), celebrity.Name), celebrity);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Celebrities.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _celebrityService.DeleteCelebrities(_celebrityService.GetCelebritiesByIds(selectedIds.ToArray()).ToList());
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Celebrities.Deleted"));

            return Json(new { Result = true });
        }

        public virtual IActionResult CelebrityPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, int celebrityId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a celebrity with the specified id
            var celebrity = _celebrityService.GetCelebrityById(celebrityId)
                ?? throw new ArgumentException("No celebrity found with the specified id");


            if (_celebrityService.GetCelebrityPicturesByCelebrityId(celebrityId).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(celebrity.Name));

            _celebrityService.InsertCelebrityPicture(new CelebrityPicture
            {
                PictureId = pictureId,
                CelebrityId = celebrityId,
                DisplayOrder = displayOrder
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult CelebrityPictureList(CelebrityPictureSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedDataTablesJson();

            //try to get a celebrity with the specified id
            var celebrity = _celebrityService.GetCelebrityById(searchModel.CelebrityId)
                ?? throw new ArgumentException("No celebrity found with the specified id");

            //prepare model
            var model = _celebrityModelFactory.PrepareCelebrityPictureListModel(searchModel, celebrity);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CelebrityPictureUpdate(CelebrityPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            //try to get a Celebrity picture with the specified id
            var celebrityPicture = _celebrityService.GetCelebrityPictureById(model.Id)
                ?? throw new ArgumentException("No Celebrity picture found with the specified id");

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(celebrityPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            celebrityPicture.DisplayOrder = model.DisplayOrder;
            _celebrityService.UpdateCelebrityPicture(celebrityPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CelebrityPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrities))
                return AccessDeniedView();

            //try to get a celebrity picture with the specified id
            var celebrityPicture = _celebrityService.GetCelebrityPictureById(id)
                ?? throw new ArgumentException("No Celebrity picture found with the specified id");

            var pictureId = celebrityPicture.PictureId;
            _celebrityService.DeleteCelebrityPicture(celebrityPicture);

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        protected virtual string[] ParseCelebrityTags(string celebrityTags)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(celebrityTags))
                return result.ToArray();

            var values = celebrityTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var val in values)
                if (!string.IsNullOrEmpty(val.Trim()))
                    result.Add(val.Trim());

            return result.ToArray();
        }

        #region Celebrity tags

        public virtual IActionResult CelebrityTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrityTags))
                return AccessDeniedView();

            //prepare model
            var model = _celebrityModelFactory.PrepareCelebrityTagSearchModel(new CelebrityTagSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CelebrityTags(CelebrityTagSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrityTags))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _celebrityModelFactory.PrepareCelebrityTagListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CelebrityTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrityTags))
                return AccessDeniedView();

            //try to get a celebrity tag with the specified id
            var tag = _celebrityTagService.GetCelebrityTagById(id)
                ?? throw new ArgumentException("No celebrity tag found with the specified id");

            _celebrityTagService.DeleteCelebrityTag(tag);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.CelebrityTags.Deleted"));

            return RedirectToAction("CelebrityTags");
        }

        [HttpPost]
        public virtual IActionResult CelebrityTagsDelete(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrityTags))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var tags = _celebrityTagService.GetCelebrityTagsByIds(selectedIds.ToArray());
                _celebrityTagService.DeleteCelebrityTags(tags);
            }

            return Json(new { Result = true });
        }

        public virtual IActionResult EditCelebrityTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrityTags))
                return AccessDeniedView();

            //try to get a celebrity tag with the specified id
            var celebrityTag = _celebrityTagService.GetCelebrityTagById(id);
            if (celebrityTag == null)
                return RedirectToAction("List");

            //prepare tag model
            var model = _celebrityModelFactory.PrepareCelebrityTagModel(null, celebrityTag);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditCelebrityTag(CelebrityTagModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCelebrityTags))
                return AccessDeniedView();

            //try to get a celebrity tag with the specified id
            var celebrityTag = _celebrityTagService.GetCelebrityTagById(model.Id);
            if (celebrityTag == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                celebrityTag.Name = model.Name;
                _celebrityTagService.UpdateCelebrityTag(celebrityTag);

                //locales
                UpdateLocales(celebrityTag, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.CelebrityTags.Updated"));

                return continueEditing ? RedirectToAction("EditCelebrityTag", new { id = celebrityTag.Id }) : RedirectToAction("CelebrityTags");
            }

            //prepare model
            model = _celebrityModelFactory.PrepareCelebrityTagModel(model, celebrityTag, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion
    }
}
