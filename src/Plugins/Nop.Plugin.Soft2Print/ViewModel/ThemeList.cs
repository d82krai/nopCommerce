using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print.ViewModel
{
    public class ThemeList
    {
        public bool Show { get; set; }
        public IEnumerable<Theme> Themes { get; set; }
    }
}
