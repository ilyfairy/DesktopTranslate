using Microsoft.Web.WebView2.Wpf;
using System.Text;
using System.Threading.Tasks;

namespace Ilyfairy.Tools.DesktopTranslate;

public static class WebViewEx
{
    public static async Task GoogleTranslateBeautify(this WebView2 web)
    {
        //顶栏空白
        await web.ExecuteScriptAsync("document.querySelector('body > div > header').parentElement.remove();");
        //顶栏
        await web.ExecuteScriptAsync("var tmp = document.querySelector('body > c-wiz > div > div');if(tmp.innerHTML == '') tmp.remove();");
        //去除 文字,文档 翻译
        await web.ExecuteScriptAsync("document.querySelector('body > c-wiz > div > div > c-wiz > div > nav').parentElement.remove();");
        //去除帮助
        await web.ExecuteScriptAsync("var tmp = document.querySelector('body > c-wiz > div > div > c-wiz > div:nth-child(3)');if(tmp.children.length == 0) tmp.remove();");
        //去除语音
        await web.ExecuteScriptAsync("document.querySelector('body > c-wiz > div > div > c-wiz > div > c-wiz > div > div > div > c-wiz > div > div > c-wiz > span > div > div > span > button > div').parentElement.parentElement.parentElement.remove();");
    }

    public static async Task GoogleTranslateFocusInputBox(this WebView2 web)
    {
        await web.ExecuteScriptAsync("document.querySelector('body > c-wiz > div > div > c-wiz > div > c-wiz > div > div > div > c-wiz > span > span > div > textarea').focus();");
    }
    
    public static async Task GoogleTranslateInput(this WebView2 web,string text)
    {
        char[] cs = text.ToCharArray();
        StringBuilder s = new();
        for (int i = 0; i < cs.Length; i++)
        {
            s.AppendFormat("\\u{0:x4}", (int)cs[i]);
        }
        await web.ExecuteScriptAsync($@"
var input = document.querySelector('body > c-wiz > div > div > c-wiz > div > c-wiz > div > div > div > c-wiz > span > span > div > textarea');
var inputEvent = document.createEvent('HTMLEvents');
inputEvent.initEvent('input', true, true);
input.value = '{s}';
input.dispatchEvent(inputEvent)
");
    }
}
