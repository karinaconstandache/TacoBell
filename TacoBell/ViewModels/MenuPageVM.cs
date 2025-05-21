using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Models.Entities;
using TacoBell.Services;
using TacoBell.Models.DTOs;
using TacoBell.Views;

namespace TacoBell.ViewModels
{
    public class MenuPageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly CategoryService _categoryService = new();
        private readonly ProductService _productService = new();
        private readonly MenuService _menuService = new();

        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<IDisplayItem> FilteredItems { get; set; } = new();

        public CartViewModel CartVM { get; } = new();

        public ICommand SelectCategoryCommand { get; }
        public ICommand ShowAllergensCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ToggleCartCommand { get; }

        private bool _isCartVisible = false;
        public bool IsCartVisible
        {
            get => _isCartVisible;
            set { _isCartVisible = value; OnPropertyChanged(); }
        }

        public MenuPageVM(NavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToLoginCommand = new RelayCommand(_ => _navigationService.NavigateTo("LoginPage"));
            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            NavigateToHomeCommand = new RelayCommand(_ => _navigationService.NavigateTo("HomePage"));
            NavigateToMenuCommand = new RelayCommand(_ => _navigationService.NavigateTo("MenuPage"));
            NavigateToAccountCommand = new RelayCommand(_ => NavigateToAccount());
            LogoutCommand = new RelayCommand(_ => Logout());

            SelectCategoryCommand = new RelayCommand(OnCategorySelected);
            ShowAllergensCommand = new RelayCommand(OnShowAllergens);
            AddToCartCommand = new RelayCommand(AddToCart);
            ToggleCartCommand = new RelayCommand(_ => IsCartVisible = !IsCartVisible);

            LoadCategories();
        }

        private async void LoadCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<Category>(categories);
            OnPropertyChanged(nameof(Categories));
        }

        private async void OnCategorySelected(object categoryObj)
        {
            if (categoryObj is not Category category) return;

            FilteredItems.Clear();
            var products = await _productService.GetByCategoryIdAsync(category.CategoryId);
            var menus = await _menuService.GetByCategoryIdAsync(category.CategoryId);

            foreach (var p in products)
                FilteredItems.Add(p);
            foreach (var m in menus)
                FilteredItems.Add(m);

            OnPropertyChanged(nameof(FilteredItems));
        }

        private void OnShowAllergens(object item)
        {
            if (item is IDisplayItem displayItem)
            {
                string allergens = string.Join(", ", displayItem.Allergens);
                MessageBox.Show(string.IsNullOrWhiteSpace(allergens) ? "Fără alergeni" : $"Alergeni: {allergens}", "Informații");
            }
        }

        private void AddToCart(object parameter)
        {
            if (!UserSessionService.IsUserLoggedIn)
            {
                _navigationService.NavigateTo("LoginPage");
                return;
            }

            if (parameter is ProductDisplayDTO product)
                CartVM.AddProduct(product);
            else if (parameter is MenuDisplayDTO menu)
                CartVM.AddMenu(menu);
        }

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        public string CurrentUserName => UserSessionService.CurrentUser?.FirstName ?? "";
        public bool IsUserLoggedIn => UserSessionService.IsUserLoggedIn;

        public ICommand NavigateToLoginCommand { get; }
        public ICommand OpenAppMenuCommand { get; }
        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToMenuCommand { get; }
        public ICommand NavigateToAccountCommand { get; }
        public ICommand LogoutCommand { get; }

        private void NavigateToAccount()
        {
            if (UserSessionService.IsUserLoggedIn)
                _navigationService.NavigateTo("AccountPage");
            else
                _navigationService.NavigateTo("LoginPage");
        }

        private void Logout()
        {
            UserSessionService.Logout();
            _navigationService.NavigateTo("LoginPage");
            OnPropertyChanged(nameof(CurrentUserName));
            OnPropertyChanged(nameof(IsUserLoggedIn));
        }
    }
}