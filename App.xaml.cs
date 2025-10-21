using CookMaster.Services;
using CookMaster.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CookMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1) Tjänster
            var userManager = new UserManager();

            // 2) ViewModel
            var mainVm = new MainViewModel (userManager);

            // 3) View
            var window = new MainWindow { DataContext = mainVm };
            window.Show();
        }
    }

}
