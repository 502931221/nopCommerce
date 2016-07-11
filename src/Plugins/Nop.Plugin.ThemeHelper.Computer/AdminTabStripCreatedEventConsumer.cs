using System;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.ThemeHelper.Computer
{
    public class AdminTabStripCreatedEventConsumer : IConsumer<AdminTabStripCreated>
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;

        public AdminTabStripCreatedEventConsumer(
            IPluginFinder pluginFinder,
            ILocalizationService localizationService)
        {
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
        }

        public void HandleEvent(AdminTabStripCreated eventMessage)
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("ThemeHelper.Computer");
            if (pluginDescriptor == null)
                return;

            var plugin = pluginDescriptor.Instance() as ComputerPlugin;
            if (plugin == null)
                return;

            if (eventMessage.TabStripName == "category-edit")
            {
                int categoryId = Convert.ToInt32(System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["Id"]);

                var actionName = "CategoryIconTabContent";
                var controllerName = "ThemeHelperComputer";
                var routeValues = new RouteValueDictionary()
                                  {
                                      {"Namespaces", "Nop.Plugin.ThemeHelper.Computer.Controllers"},
                                      {"area", null},
                                      {"categoryId", categoryId}
                                  };
                var urlHelper = new UrlHelper(eventMessage.Helper.ViewContext.RequestContext).Action(actionName, controllerName, routeValues);

                eventMessage.BlocksToRender.Add(new MvcHtmlString("<script>" +
                                                                  "$(document).ready(function() {" +
                                                                  "$('#category-edit').data('kendoTabStrip').append(" +
                                                                  "[{" +
                                                                  "text: '" + _localizationService.GetResource("Computer.Category.MenuIconTab") + "'," +
                                                                  "contentUrl: '" + urlHelper + "'" +
                                                                  "}]);" +
                                                                  "});" +
                                                                  "</script>"));

            }
        }
    }
}