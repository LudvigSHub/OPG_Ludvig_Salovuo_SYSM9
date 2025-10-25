using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CookMaster.Infrastructure
{
    public class NavigationService
    {
        private Window? _currentWindow;


        // anropas när man visar ett nytt Huvudfönster
        public void SetCurrentWindow(Window window)
        {
            _currentWindow = window;
        }


        // navigerar till ett nytt fönster av angiven typ med angiven ViewModel
        // stänger det gamla fönstret
        public void NavigateTo<TWindow>(object viewModel) where TWindow : Window, new()
        {
            // skapa nytt fönster och sätt DataContext
            var newWindow = new TWindow { DataContext = viewModel };
            newWindow.Show();

            // stäng gammalt fönster om det finns
            _currentWindow.Close();
            // sätt nya fönstret som aktuellt
            _currentWindow = newWindow;
        }
    }
}
