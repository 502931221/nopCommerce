using Nop.Core.Configuration;

namespace Nop.Plugin.ThemeHelper.Computer
{
    public class ComputerSettings : ISettings
    {
        public bool DataAlreadyInstalled { get; set; }
        public string Slide1Html { get; set; }
        public string Slide2Html { get; set; }
        public string Slide3Html { get; set; }
        public string PromotionInfo { get; set; }
    }
}