using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using CookMaster.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;


namespace CookMaster
{
    // Är startpunkten för WPF-applikationen.
    public partial class App : Application
    {

        // OnStartup körs när applikationen startar.
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var navigation = new NavigationService();
            
            // skapa och konfigurera tjänster som används i appen
            // Skapa UserManager som hanterar användare och inloggning
            var userManager = new UserManager();

            // Skapa RecipeManager som hanterar recept
            var recipeMgr = new RecipeManager(userManager);




            // 
            var mainVm = new MainViewModel (userManager, navigation, recipeMgr);

            
            var window = new MainWindow { DataContext = mainVm };
            window.Show();

            navigation.SetCurrentWindow(window);
        }
    }

}
