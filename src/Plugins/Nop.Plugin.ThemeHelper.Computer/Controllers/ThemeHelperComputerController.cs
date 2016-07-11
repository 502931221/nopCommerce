using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Plugin.ThemeHelper.Computer.Infrastructure.Cache;
using Nop.Plugin.ThemeHelper.Computer.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.UI;

namespace Nop.Plugin.ThemeHelper.Computer.Controllers
{
    public class ThemeHelperComputerController : BasePluginController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IForumService _forumService;
        private readonly ITopicService _topicService;

        private readonly CustomerSettings _customerSettings;
        private readonly ComputerSettings _computerSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly BlogSettings _blogSettings;

        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly IPluginFinder _pluginFinder;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ThemeHelperComputerController(
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ICategoryService categoryService,
            IPictureService pictureService,
            IProductService productService,
            ISettingService settingService,
            IForumService forumService,
            IStoreService storeService,
            ITopicService topicService,

            ComputerSettings computerSettings,
            CustomerSettings customerSettings,
            CatalogSettings catalogSettings,
            ForumSettings forumSettings,
            BlogSettings blogSettings,

            IStoreContext storeContext,
            ICacheManager cacheManager,
            IPluginFinder pluginFinder,
            IWorkContext workContext,
            ILogger logger)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._storeService = storeService;
            this._computerSettings = computerSettings;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._forumService = forumService;
            this._customerSettings = customerSettings;
            this._forumSettings = forumSettings;
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
            this._categoryService = categoryService;
            this._pictureService = pictureService;
            this._catalogSettings = catalogSettings;
            this._blogSettings = blogSettings;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._productService = productService;
            this._topicService = topicService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <param name="allCategories">All available categories; pass null to load them internally</param>
        /// <returns>Category models</returns>
        [NonAction]
        protected virtual IList<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            if (allCategories == null)
            {
                //load categories if null passed
                //we implemeneted it this way for performance optimization - recursive iterations (below)
                //this way all categories are loaded only once
                allCategories = _categoryService.GetAllCategories();
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //product number for each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    string cacheKey = string.Format(ComputerThemeModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                        _storeContext.CurrentStore.Id,
                        category.Id);
                    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                    {
                        var categoryIds = new List<int>();
                        categoryIds.Add(category.Id);
                        //include subcategories
                        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                            categoryIds.AddRange(GetChildCategoryIds(category.Id));
                        return _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                    });
                }

                //category icon
                var iconId = category.GetAttribute<int>("category-icon");
                if (iconId != 0)
                {
                    categoryModel.PictureUrl = _pictureService.GetPictureUrl(iconId);
                }

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ComputerThemeModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        #endregion

        #region Methods

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var computerSettings = _settingService.LoadSetting<ComputerSettings>(storeScope);

            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeScope,
                DataAlreadyInstalled = computerSettings.DataAlreadyInstalled,
                Slide1Html = computerSettings.Slide1Html,
                Slide2Html = computerSettings.Slide2Html,
                Slide3Html = computerSettings.Slide3Html,
                PromotionInfo = computerSettings.PromotionInfo,
            };
            if (storeScope > 0)
            {
                model.Slide1Html_OverrideForStore = _settingService.SettingExists(computerSettings, x => x.Slide1Html, storeScope);
                model.Slide2Html_OverrideForStore = _settingService.SettingExists(computerSettings, x => x.Slide2Html, storeScope);
                model.Slide3Html_OverrideForStore = _settingService.SettingExists(computerSettings, x => x.Slide3Html, storeScope);
                model.PromotionInfo_OverrideForStore = _settingService.SettingExists(computerSettings, x => x.PromotionInfo, storeScope);
            }
            return View("~/Plugins/ThemeHelper.Computer/Views/ThemeHelperComputer/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        [FormValueRequired("saveconfigure")]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var computerSettings = _settingService.LoadSetting<ComputerSettings>(storeScope);

            computerSettings.Slide1Html = model.Slide1Html;
            computerSettings.Slide2Html = model.Slide2Html;
            computerSettings.Slide3Html = model.Slide3Html;
            computerSettings.PromotionInfo = model.PromotionInfo;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.Slide1Html_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(computerSettings, x => x.Slide1Html, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(computerSettings, x => x.Slide1Html, storeScope);

            if (model.Slide2Html_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(computerSettings, x => x.Slide2Html, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(computerSettings, x => x.Slide2Html, storeScope);

            if (model.Slide3Html_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(computerSettings, x => x.Slide3Html, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(computerSettings, x => x.Slide3Html, storeScope);

            if (model.PromotionInfo_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(computerSettings, x => x.PromotionInfo, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(computerSettings, x => x.PromotionInfo, storeScope);


            //now clear settings cache
            _settingService.ClearCache();

            return Configure();
        }
        //install sample data
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("installsampledata")]
        public ActionResult InstallSampleData()
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("ThemeHelper.Computer");
            if (pluginDescriptor == null)
                throw new Exception("Cannot load the plugin");

            //plugin
            var plugin = pluginDescriptor.Instance() as ComputerPlugin;
            if (plugin == null)
                throw new Exception("Cannot load the plugin");

            //install data
            try
            {
                plugin.InstallSampleData();

                var furnitureSettings = _settingService.LoadSetting<ComputerSettings>();
                furnitureSettings.DataAlreadyInstalled = true;
                _settingService.SaveSetting(furnitureSettings, x => x.DataAlreadyInstalled);

                var message = _localizationService.GetResource("Computer.SampleData.InstallationCompleted");
                AddNotification(NotifyType.Success, message, true);
                _logger.Information(message);
            }
            catch (Exception exception)
            {
                var message = _localizationService.GetResource("Computer.SampleData.InstallationError") + exception;
                AddNotification(NotifyType.Error, message, true);
                _logger.Error(message);
            }

            return Configure();
        }

        //preconfigure
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("preconfigure")]
        public ActionResult Preconfigure()
        {
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("ThemeHelper.Computer");
            if (pluginDescriptor == null)
                throw new Exception("Cannot load the plugin");

            //plugin
            var plugin = pluginDescriptor.Instance() as ComputerPlugin;
            if (plugin == null)
                throw new Exception("Cannot load the plugin");

            //preconfigure
            try
            {
                plugin.Preconfigure();
                var message = _localizationService.GetResource("Computer.PreconfigureCompleted");
                AddNotification(NotifyType.Success, message, true);
                _logger.Information(message);
            }
            catch (Exception exception)
            {
                var message = _localizationService.GetResource("Computer.PreconfigureError") + exception;
                AddNotification(NotifyType.Error, message, true);
                _logger.Error(message);
            }

            return Configure();
        }

        //welcome block
        [ChildActionOnly]
        public ActionResult Welcome()
        {
            var customer = _workContext.CurrentCustomer;
            var model = new WelcomeModel()
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerFirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName)
            };
            return View("~/Themes/Computer/Views/Common/Welcome.cshtml", model);
        }

        //call block
        [ChildActionOnly]
        public ActionResult Call()
        {
            var model = new CallModel()
            {
                PhoneNumber = _storeContext.CurrentStore.CompanyPhoneNumber
            };
            return PartialView("~/Themes/Computer/Views/Common/Call.cshtml", model);
        }

        //shopping cart box
        [ChildActionOnly]
        public ActionResult ShoppingCartBox()
        {
            var customer = _workContext.CurrentCustomer;
            var model = new ShoppingCartBoxModel()
            {
                ItemsCount = customer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                                       .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
                                                       .ToList().GetTotalProducts(),
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
            };
            return View("~/Themes/Computer/Views/Common/ShoppingCartBox.cshtml", model);
        }

        //discount info
        [ChildActionOnly]
        public ActionResult PromotionInfo()
        {
            var model = new PromotionInfoModel()
            {
                Content = _computerSettings.PromotionInfo,
            };
            return View("~/Themes/Computer/Views/Home/PromotionInfo.cshtml", model);
        }

        //slider
        [ChildActionOnly]
        public ActionResult Slider()
        {
            var model = new SliderModel()
            {
                Slide1Html = _computerSettings.Slide1Html,
                Slide2Html = _computerSettings.Slide2Html,
                Slide3Html = _computerSettings.Slide3Html,
            };
            return View("~/Themes/Computer/Views/Home/Slider.cshtml", model);
        }

        public ActionResult CategoryIconAdd(int pictureId, int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new ArgumentException("No category found with the specified id");
            _genericAttributeService.SaveAttribute<int>(category, "category-icon", pictureId);
            _cacheManager.RemoveByPattern(ComputerThemeModelCacheEventConsumer.CATEGORY_MENU_PATTERN_KEY);
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CategoryIconTabContent(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            var model = new CategoryIconModel { IconId = category.GetAttribute<int>("category-icon") };
            return View("~/Plugins/ThemeHelper.Computer/Views/ThemeHelperComputer/CategoryIconTab.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult TopMenu()
        {
            //categories
            string categoryCacheKey = string.Format(ComputerThemeModelCacheEventConsumer.CATEGORY_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var cachedCategoriesModel = _cacheManager.Get(categoryCacheKey, () => PrepareCategorySimpleModels(0));

            //top menu topics
            string topicCacheKey = string.Format(ComputerThemeModelCacheEventConsumer.TOPIC_TOP_MENU_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInTopMenu)
                .Select(t => new TopMenuModel.TopMenuTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName()
                })
                .ToList()
            );
            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                Topics = cachedTopicModel,
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };

            return PartialView("~/Themes/Computer/Views/Catalog/TopMenu.cshtml", model);
        }

        #endregion
    }
}