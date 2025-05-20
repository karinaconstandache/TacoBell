using System.Windows;
using TacoBell.Views;

namespace TacoBell.Services
{
    public class NavigationService
    {
        public void NavigateTo(string viewName)
        {
            Window window = viewName switch
            {
                "HomePage" => new HomePage(),
                "LoginPage" => new LoginPage(),
                "MenuPage" => new MenuPage(),
                "AccountPage" => new AccountPage(),
                "AdminPage" => new AdminPage(),
                _ => null
            };

            window?.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w != window)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}

