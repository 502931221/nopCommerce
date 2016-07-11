using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.ThemeHelper.Computer.Models
{
    public class WelcomeModel : BaseNopModel
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerFirstName { get; set; }
    }
}