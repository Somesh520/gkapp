using System.Collections.ObjectModel;
using System.Windows.Input;
using GKFashionApp.Models;
using GKFashionApp.Services;

namespace GKFashionApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly DatabaseService _dbService;

        public ObservableCollection<FashionItem> ClothingItems { get; set; } = new();

        // Tabs
        private bool _isHomeTab = true;
        public bool IsHomeTab { get => _isHomeTab; set { _isHomeTab = value; OnPropertyChanged(); } }

        private bool _isAdminTab = false;
        public bool IsAdminTab { get => _isAdminTab; set { _isAdminTab = value; OnPropertyChanged(); } }

        private bool _isDetailsVisible = false;
        public bool IsDetailsVisible { get => _isDetailsVisible; set { _isDetailsVisible = value; OnPropertyChanged(); } }

        private FashionItem _selectedProduct;
        public FashionItem SelectedProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(); } }

        // Inputs
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string _size;
        public string Size { get => _size; set { _size = value; OnPropertyChanged(); } }

        private double _price;
        public double Price { get => _price; set { _price = value; OnPropertyChanged(); } }

        private string _imageUrl;
        public string ImageUrl { get => _imageUrl; set { _imageUrl = value; OnPropertyChanged(); } }

        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        // Commands
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SwitchToHomeCommand { get; }
        public ICommand SwitchToAdminCommand { get; }
        public ICommand OpenDetailsCommand { get; }
        public ICommand CloseDetailsCommand { get; }

        public HomeViewModel()
        {
            _dbService = new DatabaseService();

            AddCommand = new Command(async () => await AddItem());
            DeleteCommand = new Command<FashionItem>(async item => await DeleteItem(item));

            SwitchToHomeCommand = new Command(() =>
            {
                IsHomeTab = true;
                IsAdminTab = false;
                IsDetailsVisible = false;
            });

            SwitchToAdminCommand = new Command(() =>
            {
                IsHomeTab = false;
                IsAdminTab = true;
                IsDetailsVisible = false;
            });

            OpenDetailsCommand = new Command<FashionItem>(item =>
            {
                SelectedProduct = item;
                IsDetailsVisible = true;
            });

            CloseDetailsCommand = new Command(() =>
            {
                IsDetailsVisible = false;
                SelectedProduct = null;
            });

            Task.Run(async () => await LoadData());
        }

        private async Task LoadData()
        {
            try
            {
                var items = await _dbService.GetItemsAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ClothingItems.Clear();
                    if (items != null)
                    {
                        foreach (var item in items)
                            ClothingItems.Add(item);
                    }
                });
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ClothingItems.Clear();
                });
            }
        }

        private async Task AddItem()
        {
            if (string.IsNullOrWhiteSpace(Name) || Price <= 0)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Error",
                        "Name aur Price required",
                        "OK");
                });
                return;
            }

            var newItem = new FashionItem
            {
                Name = Name,
                Size = Size,
                Price = Price,
                ImageUrl = string.IsNullOrWhiteSpace(ImageUrl)
                    ? "https://placehold.co/400x500/png?text=GK+Fashion"
                    : ImageUrl,
                Description = string.IsNullOrWhiteSpace(Description)
                    ? "Premium Quality"
                    : Description
            };

            bool success = await _dbService.SaveItemAsync(newItem);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (success)
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Success",
                        "✅ Item saved successfully",
                        "OK");

                    Name = "";
                    Size = "";
                    Price = 0;
                    ImageUrl = "";
                    Description = "";

                    await LoadData();
                    IsHomeTab = true;
                    IsAdminTab = false;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Failed",
                        "❌ Failed to save item",
                        "OK");
                }
            });
        }

        private async Task DeleteItem(FashionItem item)
        {
            bool confirm = await App.Current.MainPage.DisplayAlert(
                "Delete Item",
                $"Remove {item.Name}?",
                "Yes",
                "No");

            if (!confirm) return;

            await _dbService.DeleteItemAsync(item);
            await LoadData();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await App.Current.MainPage.DisplayAlert(
                    "Deleted",
                    "🗑️ Item deleted",
                    "OK");
            });
        }
    }
}
