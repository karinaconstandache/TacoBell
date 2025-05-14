using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Models.BusinessLogicLayer;
using TacoBell.Models.Entities;
using TacoBell.Models.Enums;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public class LoginPageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly UserSessionService _userSessionService;
        private readonly UserBLL _userBLL = new();

        public LoginPageVM(NavigationService navigationService, UserSessionService userSessionService)
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;

            NavigateToLoginCommand = new RelayCommand(_ => SwitchMode(true));
            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            NavigateToHomeCommand = new RelayCommand(_ => _navigationService.NavigateTo("HomePage"));
            NavigateToMenuCommand = new RelayCommand(_ => _navigationService.NavigateTo("MenuPage"));
            NavigateToAccountCommand = new RelayCommand(_ => NavigateToAccount());

            SubmitCommand = new RelayCommand(_ => Submit());
            SwitchToRegisterCommand = new RelayCommand(_ => SwitchMode(false));
        }

        private void SwitchMode(bool login)
        {
            IsLoginMode = login;
            ErrorMessage = "";
        }

        public ICommand NavigateToLoginCommand { get; }
        public ICommand OpenAppMenuCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMenuCommand { get; }
        public ICommand NavigateToAccountCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        private bool _isLoginMode = true;
        public bool IsLoginMode
        {
            get => _isLoginMode;
            set { _isLoginMode = value; OnPropertyChanged(); }
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryAddress { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private void NavigateToAccount()
        {
            if (_userSessionService.IsUserLoggedIn)
                _navigationService.NavigateTo("AccountPage");
            else
                _navigationService.NavigateTo("LoginPage");
        }

        private void Submit()
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return;
            }

            if (IsLoginMode)
            {
                var user = _userBLL.Login(Email, Password);
                if (user == null)
                {
                    ErrorMessage = "Incorrect email or password.";
                    return;
                }

                _userSessionService.SetUser(user);
                _navigationService.NavigateTo("HomePage");
            }
            else
            {
                if (_userBLL.IsEmailTaken(Email))
                {
                    ErrorMessage = "Email already registered.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(FirstName) ||
                    string.IsNullOrWhiteSpace(LastName) ||
                    string.IsNullOrWhiteSpace(PhoneNumber) ||
                    string.IsNullOrWhiteSpace(DeliveryAddress))
                {
                    ErrorMessage = "All fields are required.";
                    return;
                }

                var user = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    PhoneNumber = PhoneNumber,
                    DeliveryAddress = DeliveryAddress,
                    Role = UserRole.USER
                };

                _userBLL.Register(user);
                _userSessionService.SetUser(user);
                _navigationService.NavigateTo("HomePage");
            }
        }
    }
}
