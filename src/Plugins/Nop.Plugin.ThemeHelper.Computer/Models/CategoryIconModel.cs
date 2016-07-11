using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.ThemeHelper.Computer.Models
{
    public class CategoryIconModel : BaseNopModel
    {
        [UIHint("Picture")]
        [NopResourceDisplayName("Computer.Category.MenuIcon")]
        public int IconId { get; set; }
    }
}