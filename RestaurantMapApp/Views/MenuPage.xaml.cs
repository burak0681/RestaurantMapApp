using RestaurantMapApp.Models;

namespace RestaurantMapApp.Views;

public partial class MenuPage : ContentPage
{
    private Restaurant? _restaurant;

    public MenuPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        // Shell navigation'dan gelen parametreleri al
        if (Shell.Current?.CurrentPage?.BindingContext is Dictionary<string, object> parameters && 
            parameters.ContainsKey("Restaurant") && 
            parameters["Restaurant"] is Restaurant restaurant)
        {
            _restaurant = restaurant;
            LoadRestaurantData();
        }
    }

    private void LoadRestaurantData()
    {
        if (_restaurant == null) return;
        
        RestaurantNameLabel.Text = _restaurant.Name;
        AddressLabel.Text = _restaurant.Address;
        DescriptionLabel.Text = string.IsNullOrEmpty(_restaurant.Description) ? "Açıklama bulunmuyor" : _restaurant.Description;
        
        RatingLabel.Text = $"⭐ {_restaurant.Rating:F1}";
        PriceRangeLabel.Text = $"💰 {new string('$', _restaurant.PriceRange)}";
        
        PhoneLabel.Text = string.IsNullOrEmpty(_restaurant.Phone) ? "Telefon bulunmuyor" : _restaurant.Phone;
        WebsiteLabel.Text = string.IsNullOrEmpty(_restaurant.Website) ? "Website bulunmuyor" : _restaurant.Website;
        
        CuisineTypesCollection.ItemsSource = _restaurant.CuisineTypes.Any() ? _restaurant.CuisineTypes : new List<string> { "Belirtilmemiş" };
        
        // Menü listesini yükle
        if (_restaurant.MenuItems.Any())
        {
            MenuItemsCollection.ItemsSource = _restaurant.MenuItems;
        }
        else
        {
            MenuItemsCollection.ItemsSource = new List<RestaurantMapApp.Models.MenuItem> { new RestaurantMapApp.Models.MenuItem { Name = "Menü bilgisi bulunmuyor", Description = "Bu restoran için henüz menü eklenmemiş.", Price = 0, Category = "Bilgi" } };
        }
    }

    private async void MenuButton_Clicked(object sender, EventArgs e)
    {
        if (_restaurant == null) return;
        
        if (!string.IsNullOrEmpty(_restaurant.MenuUrl))
        {
            try
            {
                await Launcher.OpenAsync(new Uri(_restaurant.MenuUrl));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Menü açılamadı: {ex.Message}", "Tamam");
            }
        }
        else
        {
            await DisplayAlert("Bilgi", "Bu restoran için menü linki bulunmuyor.", "Tamam");
        }
    }
} 