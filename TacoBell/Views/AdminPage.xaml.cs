using System.Windows;
using TacoBell.Services;
using TacoBell.ViewModels;

namespace TacoBell.Views
{
    public partial class AdminPage : Window
    {
        public AdminPage()
        {
            InitializeComponent();
            DataContext = new AdminPageVM(new NavigationService());
        }
    }
}
