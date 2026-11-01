using System.Windows;

namespace GachaLife;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += (s, e) =>
        {
            MessageBox.Show(e.Exception.ToString(), "Ошибка");
            e.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "Критическая ошибка");
        };
    }
}