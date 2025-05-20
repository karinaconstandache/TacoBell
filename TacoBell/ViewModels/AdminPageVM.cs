using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TacoBell.Helpers;
using TacoBell.Models.BusinessLogicLayer;
using TacoBell.Models.Entities;
using TacoBell.Services;

namespace TacoBell.ViewModels
{
    public enum AdminSection
    {
        Dishes,
        Menus,
        Categories,
        Allergens
    }

    public class AdminPageVM : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly DishBLL _dishBLL = new();
        private readonly MenuBLL _menuBLL = new();
        private readonly CategoryBLL _categoryBLL = new();
        private readonly AllergenBLL _allergenBLL = new();

        public AdminPageVM(NavigationService navigationService)
        {
            _navigationService = navigationService;

            OpenAppMenuCommand = new RelayCommand(_ => IsAppMenuVisible = !IsAppMenuVisible);
            LogoutCommand = new RelayCommand(_ => Logout());
            ShowManageMenuCommand = new RelayCommand(_ => ToggleSection(true));
            ShowManageOrdersCommand = new RelayCommand(_ => ToggleSection(false));

            ShowDishesCommand = new RelayCommand(_ => CurrentSection = AdminSection.Dishes);
            ShowMenusCommand = new RelayCommand(_ => CurrentSection = AdminSection.Menus);
            ShowCategoriesCommand = new RelayCommand(_ => CurrentSection = AdminSection.Categories);
            ShowAllergensCommand = new RelayCommand(_ => CurrentSection = AdminSection.Allergens);

            LoadDishes();
            LoadMenus();
            LoadCategories();
            LoadAllergens();
            LoadAvailableImages();

            NewDish = new Dish();
            NewMenu = new Menu();
            NewCategory = new Category();
            NewAllergen = new Allergen();

            AvailableAllergens = new ObservableCollection<Allergen>(_allergenBLL.GetAllAllergens());
        }

        public string CurrentUserName => UserSessionService.CurrentUser?.FirstName ?? "";

        public ICommand OpenAppMenuCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ShowManageMenuCommand { get; }
        public ICommand ShowManageOrdersCommand { get; }
        public ICommand ShowDishesCommand { get; }
        public ICommand ShowMenusCommand { get; }
        public ICommand ShowCategoriesCommand { get; }
        public ICommand ShowAllergensCommand { get; }

        private bool _isAppMenuVisible;
        public bool IsAppMenuVisible
        {
            get => _isAppMenuVisible;
            set { _isAppMenuVisible = value; OnPropertyChanged(); }
        }

        private bool _isManageMenuVisible = true;
        public bool IsManageMenuVisible
        {
            get => _isManageMenuVisible;
            set { _isManageMenuVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentManagementTitle)); }
        }

        private bool _isManageOrdersVisible;
        public bool IsManageOrdersVisible
        {
            get => _isManageOrdersVisible;
            set { _isManageOrdersVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentManagementTitle)); }
        }

        public string CurrentManagementTitle => IsManageMenuVisible ? "Menu Management" : "Order Management";

        private void ToggleSection(bool showMenu)
        {
            IsManageMenuVisible = showMenu;
            IsManageOrdersVisible = !showMenu;
        }

        private AdminSection _currentSection = AdminSection.Dishes;
        public AdminSection CurrentSection
        {
            get => _currentSection;
            set
            {
                _currentSection = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDishesVisible));
                OnPropertyChanged(nameof(IsMenusVisible));
                OnPropertyChanged(nameof(IsCategoriesVisible));
                OnPropertyChanged(nameof(IsAllergensVisible));
            }
        }

        public bool IsDishesVisible => CurrentSection == AdminSection.Dishes;
        public bool IsMenusVisible => CurrentSection == AdminSection.Menus;
        public bool IsCategoriesVisible => CurrentSection == AdminSection.Categories;
        public bool IsAllergensVisible => CurrentSection == AdminSection.Allergens;

        public ObservableCollection<Dish> DishList { get; set; } = new();
        public ObservableCollection<Menu> MenuList { get; set; } = new();
        public ObservableCollection<Category> CategoryList { get; set; } = new();
        public ObservableCollection<Allergen> AllergenList { get; set; } = new();
        public ObservableCollection<Allergen> AvailableAllergens { get; set; } = new();
        public List<string> AvailableImages { get; set; }
        public string SelectedImagePath { get; set; }

        public ICommand EditDishCommand => new RelayCommand(d => { });
        public ICommand DeleteDishCommand => new RelayCommand(d => { });

        public ICommand EditCategoryCommand => new RelayCommand(c => { });
        public ICommand DeleteCategoryCommand => new RelayCommand(c => { });

        public ICommand EditAllergenCommand => new RelayCommand(a => { });
        public ICommand DeleteAllergenCommand => new RelayCommand(a => { });

        public Dish NewDish { get; set; }
        public Category SelectedCategoryForNewDish { get; set; }
        public ICommand AddDishCommand => new RelayCommand(_ => AddDish());

        private void AddDish()
        {
            if (SelectedCategoryForNewDish != null &&
                !string.IsNullOrWhiteSpace(NewDish.Name) &&
                NewDish.Price > 0 && NewDish.TotalQuantity >= 0)
            {
                NewDish.CategoryId = SelectedCategoryForNewDish.CategoryId;
                _dishBLL.AddDish(NewDish);

                if (!string.IsNullOrWhiteSpace(SelectedImagePath))
                {
                    _dishBLL.AddDishImage(NewDish.DishId, SelectedImagePath);
                }

                var selectedAllergenIds = AvailableAllergens
                    .Where(a => a.IsSelected)
                    .Select(a => a.AllergenId).ToList();

                if (selectedAllergenIds.Count > 0)
                {
                    _dishBLL.AddDishAllergens(NewDish.DishId, selectedAllergenIds);
                }

                NewDish = new Dish();
                SelectedCategoryForNewDish = null;
                SelectedImagePath = null;
                foreach (var allergen in AvailableAllergens)
                    allergen.IsSelected = false;

                OnPropertyChanged(nameof(NewDish));
                OnPropertyChanged(nameof(SelectedCategoryForNewDish));
                OnPropertyChanged(nameof(SelectedImagePath));
                LoadDishes();
            }
        }

        public Menu NewMenu { get; set; }
        public Category SelectedCategoryForNewMenu { get; set; }
        public ICommand AddMenuCommand => new RelayCommand(_ => AddMenu());
        public ICommand EditMenuCommand => new RelayCommand(m => EditMenu((Menu)m));
        public ICommand DeleteMenuCommand => new RelayCommand(m => DeleteMenu((Menu)m));

        private void AddMenu()
        {
            if (SelectedCategoryForNewMenu != null && !string.IsNullOrWhiteSpace(NewMenu.Name))
            {
                NewMenu.CategoryId = SelectedCategoryForNewMenu.CategoryId;
                _menuBLL.AddMenu(NewMenu);
                NewMenu = new Menu();
                SelectedCategoryForNewMenu = null;
                OnPropertyChanged(nameof(NewMenu));
                OnPropertyChanged(nameof(SelectedCategoryForNewMenu));
                LoadMenus();
            }
        }

        private void EditMenu(Menu menu)
        {
            if (!string.IsNullOrWhiteSpace(menu.Name) && menu.CategoryId > 0)
            {
                _menuBLL.UpdateMenu(menu);
                LoadMenus();
            }
        }

        private void DeleteMenu(Menu menu)
        {
            _menuBLL.DeleteMenu(menu.MenuId);
            LoadMenus();
        }

        public Category NewCategory { get; set; }
        public ICommand AddCategoryCommand => new RelayCommand(_ => AddCategory());

        private void AddCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewCategory.Name))
            {
                _categoryBLL.AddCategory(NewCategory);
                NewCategory = new Category();
                OnPropertyChanged(nameof(NewCategory));
                LoadCategories();
            }
        }

        public Allergen NewAllergen { get; set; }
        public ICommand AddAllergenCommand => new RelayCommand(_ => AddAllergen());

        private void AddAllergen()
        {
            if (!string.IsNullOrWhiteSpace(NewAllergen.Name))
            {
                _allergenBLL.AddAllergen(NewAllergen);
                NewAllergen = new Allergen();
                OnPropertyChanged(nameof(NewAllergen));
                LoadAllergens();
                RefreshAvailableAllergens(); // <== aici
            }
        }


        private void LoadDishes()
        {
            DishList = new ObservableCollection<Dish>(_dishBLL.GetAllDishes());
            OnPropertyChanged(nameof(DishList));
        }

        private void LoadMenus()
        {
            MenuList = new ObservableCollection<Menu>(_menuBLL.GetAllMenus());
            OnPropertyChanged(nameof(MenuList));
        }

        private void LoadCategories()
        {
            CategoryList = new ObservableCollection<Category>(_categoryBLL.GetAllCategories());
            OnPropertyChanged(nameof(CategoryList));
        }

        private void LoadAllergens()
        {
            AllergenList = new ObservableCollection<Allergen>(_allergenBLL.GetAllAllergens());
            OnPropertyChanged(nameof(AllergenList));
        }

        private void LoadAvailableImages()
        {
            var imageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images");
            if (Directory.Exists(imageDir))
            {
                AvailableImages = Directory.GetFiles(imageDir)
                    .Select(p => Path.Combine("Assets", "Images", Path.GetFileName(p))).ToList();
            }
            else
            {
                AvailableImages = new();
            }
            OnPropertyChanged(nameof(AvailableImages));
        }

        private void RefreshAvailableAllergens()
        {
            AvailableAllergens.Clear();
            foreach (var allergen in _allergenBLL.GetAllAllergens())
            {
                AvailableAllergens.Add(allergen);
            }
            OnPropertyChanged(nameof(AvailableAllergens));
        }

        private void Logout()
        {
            UserSessionService.Logout();
            _navigationService.NavigateTo("LoginPage");
        }
    }
}
