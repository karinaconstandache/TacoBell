using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public class HomePageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly UserSessionService _userSessionService;

        public HomePageVM(NavigationService navigationService, UserSessionService userSessionService)
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;

            NavigateToLoginCommand = new RelayCommand(_ => NavigateToLogin());
            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            NavigateToHomeCommand = new RelayCommand(_ => _navigationService.NavigateTo("HomePage"));
            NavigateToMenuCommand = new RelayCommand(_ => _navigationService.NavigateTo("MenuPage"));
            NavigateToAccountCommand = new RelayCommand(_ => NavigateToAccount());
        }

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        public ICommand NavigateToLoginCommand { get; }
        public ICommand OpenAppMenuCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMenuCommand { get; }
        public ICommand NavigateToAccountCommand { get; }

        private void NavigateToLogin() => _navigationService.NavigateTo("LoginPage");

        private void NavigateToAccount()
        {
            if (_userSessionService.IsUserLoggedIn)
                _navigationService.NavigateTo("AccountPage");
            else
                _navigationService.NavigateTo("LoginPage");
        }
    }
}

