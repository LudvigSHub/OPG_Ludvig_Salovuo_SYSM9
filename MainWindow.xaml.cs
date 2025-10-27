using CookMaster.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CookMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Metoden triggas varje gång användaren skriver i PasswordBox (eventet PasswordChanged)
        //
        private void Pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //DataContext is MainViewModel vm → hämtar din ViewModel
            //sender is PasswordBox pb → hämtar PasswordBox som utlöste eventet
            if (DataContext is MainViewModel vm && sender is PasswordBox pb)
                //vm.Password = pb.Password; → sätter ViewModel-egenskapen till det som finns i PasswordBox
                vm.Password = pb.Password;
        }
    }
}