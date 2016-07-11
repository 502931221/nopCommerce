using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.ThemeHelper.Computer.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        public bool DataAlreadyInstalled { get; set; }

        [NopResourceDisplayName("Computer.Slide1Html")]
        [AllowHtml]
        public string Slide1Html { get; set; }
        public bool Slide1Html_OverrideForStore { get; set; }

        [NopResourceDisplayName("Computer.Slide2Html")]
        [AllowHtml]
        public string Slide2Html { get; set; }
        public bool Slide2Html_OverrideForStore { get; set; }

        [NopResourceDisplayName("Computer.Slide3Html")]
        [AllowHtml]
        public string Slide3Html { get; set; }
        public bool Slide3Html_OverrideForStore { get; set; }

        [NopResourceDisplayName("Computer.PromotionInfo")]
        [AllowHtml]
        public string PromotionInfo { get; set; }
        public bool PromotionInfo_OverrideForStore { get; set; }
    }
}