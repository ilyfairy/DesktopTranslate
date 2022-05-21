using Ilyfairy.Tools.DesktopTranslate.Model;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Ilyfairy.Tools.DesktopTranslate;

public partial class MainWindow : Window
{
    public MainContext Context { get; }
    public MainWindow()
    {
        InitializeComponent();
        Context = (DataContext as MainContext)!;

        SourceInitialized += MainWindow_SourceInitialized;

        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => { Hide(); }));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => { SystemCommands.MinimizeWindow(this); }));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => { SystemCommands.MaximizeWindow(this); }));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => { SystemCommands.RestoreWindow(this); }));
        CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
        web.NavigationStarting += Web_NavigationStarting;
        web.NavigationCompleted += Web_NavigationCompleted;
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e)
    {
        if (PresentationSource.FromVisual(this) is HwndSource hwnd)
        {
            Context.HotKey = new HotKeyManager(hwnd.Handle);
            bool isReg = Context.HotKey.Add("show", ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows, Keys.Z, async () =>
            {
                this.Show();
                this.Activate();
                web.Focus();
                await web.GoogleTranslateFocusInputBox();
                string text = Clipboard.GetText();
                if (text != Context.LastInputText && !string.IsNullOrWhiteSpace(text))
                {
                    Context.LastInputText = text;
                    await web.GoogleTranslateInput(text);
                }
            });
            if (!isReg) MessageBox.Show("热键注册失败");
            hwnd.AddHook(HwndHook);
        }
        else
        {
            MessageBox.Show("初始化失败");
        }
        
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == 0x312)
        {
            Console.WriteLine($"热键: {wParam} {lParam}");
            Context.HotKey.Loop((int)wParam);
        }
        return IntPtr.Zero;
    }
    private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement element) return;

        Point position = (WindowState == WindowState.Maximized) ? 
            (new Point(0, element.ActualHeight)) : 
            (new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top));
        position = element.TransformToAncestor(this).Transform(position);
        SystemCommands.ShowSystemMenu(this, position);
    }
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await web.EnsureCoreWebView2Async();
        web.NavigateToString("<span>loading...<span>");
        Console.WriteLine(web.Source);
    }
    private async void Web_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        switch (web.Source.Host)
        {
            case "translate.google.cn":
                for (int i = 0; i < 20; i++)
                {
                    await web.GoogleTranslateBeautify();
                    await Task.Delay(100);
                }
                web.Stop();
                break;
        }
    }
    private void Web_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (web.Source.AbsoluteUri == "about:blank")
        {
            web.Source = new Uri("https://translate.google.cn/");
        }
    }
}
