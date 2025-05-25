using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Models.Entities;
using TacoBell.Services;
using TacoBell.Models.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TacoBell.ViewModels
{
    public class MenuPageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly CategoryService _categoryService = new();
        private readonly DishService _dishService = new();
        private readonly MenuService _menuService = new();

        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<IDisplayItem> FilteredItems { get; set; } = new();
        public ObservableCollection<CartItem> CartItems { get; set; } = new();

        private bool _isCartVisible;
        public bool IsCartVisible
        {
            get => _isCartVisible;
            set { _isCartVisible = value; OnPropertyChanged(); }
        }

        public decimal Subtotal => CartItems.Sum(c => c.TotalPrice);
        public decimal ShippingFee => Subtotal < 50 ? 10 : 0;
        public bool HasDiscount => Subtotal > 100;
        public decimal Discount => HasDiscount ? Subtotal * 0.10m : 0;
        public decimal Total => Subtotal + ShippingFee - Discount;

        public ICommand SelectCategoryCommand { get; }
        public ICommand ShowAllergensCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ToggleCartCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }

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
            AddToCartCommand = new RelayCommand(OnAddToCart);
            ToggleCartCommand = new RelayCommand(_ => IsCartVisible = !IsCartVisible);
            PlaceOrderCommand = new RelayCommand(async _ => await OnPlaceOrder());
            IncreaseQuantityCommand = new RelayCommand(OnIncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(OnDecreaseQuantity);

            CartItems.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(ShippingFee));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(Total));
            };

            LoadCategories();
        }

        private async Task OnPlaceOrder()
        {
            // To be implemented in next step.
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

            var dishes = await _dishService.GetByCategoryIdAsync(category.CategoryId);
            var menus = await _menuService.GetByCategoryIdAsync(category.CategoryId);

            foreach (var d in dishes)
                FilteredItems.Add(d);

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

        private void OnAddToCart(object item)
        {
            if (!UserSessionService.IsUserLoggedIn)
            {
                MessageBox.Show("Trebuie să fii autentificat pentru a adăuga produse în coș.", "Autentificare necesară");
                _navigationService.NavigateTo("LoginPage");
                return;
            }

            if (item is DishDisplayDTO dish)
            {
                var existing = CartItems.FirstOrDefault(c => c.DishId == dish.DishId);
                if (existing != null)
                    existing.Quantity++;
                else
                    CartItems.Add(new CartItem
                    {
                        Name = dish.Name,
                        DishId = dish.DishId,
                        Price = dish.Price,
                        Quantity = 1
                    });
            }
            else if (item is MenuDisplayDTO menu)
            {
                var existing = CartItems.FirstOrDefault(c => c.MenuId == menu.MenuId);
                if (existing != null)
                    existing.Quantity++;
                else
                    CartItems.Add(new CartItem
                    {
                        Name = menu.Name,
                        MenuId = menu.MenuId,
                        Price = menu.Price,
                        Quantity = 1
                    });
            }

            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(ShippingFee));
            OnPropertyChanged(nameof(Discount));
            OnPropertyChanged(nameof(Total));
            MessageBox.Show("Produs adăugat în coș.", "Confirmare");
        }

        private void OnIncreaseQuantity(object param)
        {
            if (param is CartItem item)
            {
                item.Quantity++;
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(ShippingFee));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(Total));
            }
        }

        private void OnDecreaseQuantity(object param)
        {
            if (param is CartItem item)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    CartItems.Remove(item);

                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(ShippingFee));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(Total));
            }
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
