using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilyfairy.Tools.DesktopTranslate.Model
{
    public class MainContext
    {
        public HotKeyManager HotKey { get; set; }
        public string LastInputText { get; set; } = string.Empty;
    }
}
