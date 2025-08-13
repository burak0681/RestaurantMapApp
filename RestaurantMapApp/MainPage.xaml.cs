using System.Text.Json;
using RestaurantMapApp.Models;
using RestaurantMapApp.Services;

namespace RestaurantMapApp.Views;

public partial class MainPage : ContentPage
{
    private readonly IRestaurantService _service;
    private List<Restaurant> _allRestaurants = new();
    private List<Restaurant> _filteredRestaurants = new();
    private bool _showOnlyOpen = false;
    private int _priceFilter = 0; // 0 = tümü, 1-4 = fiyat aralığı
    private double _ratingFilter = 0.0; // 0 = tümü, 1-5 = minimum puan

    public MainPage(IRestaurantService restaurantService)
    {
        InitializeComponent();
        _service = restaurantService;
        Loaded += MainPage_Loaded;
    }

    private async void MainPage_Loaded(object? sender, EventArgs e)
    {
        try
    {
        // Leaflet HTML'ini yükle
        using var stream = await FileSystem.OpenAppPackageFileAsync("map.html");
        using var reader = new StreamReader(stream);
        var html = await reader.ReadToEndAsync();
        MapWebView.Source = new HtmlWebViewSource { Html = html };

            // WebView hazır olsun diye kısa bir gecikme bırak
            await Task.Delay(500);

            // Restoran verisini yükle
            await LoadRestaurants();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Harita yüklenirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async Task LoadRestaurants()
    {
        try
        {
            _allRestaurants = await _service.GetRestaurantsAsync();
            // Başlangıçta hiçbir restoran gösterme
            _filteredRestaurants = new List<Restaurant>();
            UpdateRestaurantCount();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Restoran verileri yüklenirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async Task UpdateMap()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(_filteredRestaurants, options);
            
            await DisplayAlert("Debug", $"UpdateMap çağrıldı:\nRestoran sayısı: {_filteredRestaurants.Count}\nJSON uzunluğu: {json.Length}\nJSON: {json.Substring(0, Math.Min(200, json.Length))}...", "Tamam");
            
            await MapWebView.EvaluateJavaScriptAsync($"window.setRestaurantsJson('{json}');");
            
            await DisplayAlert("Debug", "JavaScript fonksiyonu çağrıldı", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Harita güncellenirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private void UpdateRestaurantCount()
    {
        RestaurantCountLabel.Text = $"{_filteredRestaurants.Count} restoran bulundu";
    }

    // Arama işlemleri
    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Arama metni değiştiğinde hemen filtrele
        _ = Task.Run(async () => await ApplyFilters());
    }

    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
        await ApplyFilters();
    }

    private async Task ApplyFilters()
    {
        var searchText = SearchEntry.Text?.ToLowerInvariant() ?? "";
        
        if (!string.IsNullOrEmpty(searchText))
        {
            // Sadece arama metni ile eşleşen restoranları bul
            _filteredRestaurants = _allRestaurants.Where(r => 
                r.SearchText.Contains(searchText)
            ).ToList();
            
            await DisplayAlert("Debug", $"'{searchText}' için {_filteredRestaurants.Count} restoran bulundu", "Tamam");
            
            // Haritayı güncelle
            await UpdateMap();
        }
        else
        {
            // Arama metni yoksa hiçbir restoran gösterme
            _filteredRestaurants = new List<Restaurant>();
            await DisplayAlert("Debug", "Arama metni boş, hiçbir restoran gösterilmiyor", "Tamam");
            
            // Haritayı temizle
            await UpdateMap();
        }
        
        UpdateRestaurantCount();
    }

    // Filtre işlemleri
    private async void OpenFilter_Clicked(object sender, EventArgs e)
    {
        _showOnlyOpen = !_showOnlyOpen;
        
        if (_showOnlyOpen)
        {
            OpenFilterButton.BackgroundColor = Colors.Green;
            OpenFilterButton.TextColor = Colors.White;
        }
        else
        {
            OpenFilterButton.BackgroundColor = Color.FromArgb("#E8F5E8");
            OpenFilterButton.TextColor = Color.FromArgb("#166534");
        }
        
        await ApplyFilters();
    }

    private async void PriceFilter_Clicked(object sender, EventArgs e)
    {
        var options = new[] { "Tümü", "💰 (Ucuz)", "💰💰 (Orta)", "💰💰💰 (Pahalı)", "💰💰💰💰 (Lüks)" };
        var result = await DisplayActionSheet("Fiyat Filtresi", "İptal", null, options);
        
        _priceFilter = result switch
        {
            "💰 (Ucuz)" => 1,
            "💰💰 (Orta)" => 2,
            "💰💰💰 (Pahalı)" => 3,
            "💰💰💰💰 (Lüks)" => 4,
            _ => 0
        };

        if (_priceFilter > 0)
        {
            PriceFilterButton.BackgroundColor = Colors.Blue;
            PriceFilterButton.TextColor = Colors.White;
        }
        else
        {
            PriceFilterButton.BackgroundColor = Color.FromArgb("#E0F2FE");
            PriceFilterButton.TextColor = Color.FromArgb("#0C4A6E");
        }
        
        await ApplyFilters();
    }

    private async void RatingFilter_Clicked(object sender, EventArgs e)
    {
        var options = new[] { "Tümü", "⭐ 4.0+", "⭐ 4.5+", "⭐ 5.0" };
        var result = await DisplayActionSheet("Puan Filtresi", "İptal", null, options);
        
        _ratingFilter = result switch
        {
            "⭐ 4.0+" => 4.0,
            "⭐ 4.5+" => 4.5,
            "⭐ 5.0" => 5.0,
            _ => 0.0
        };

        if (_ratingFilter > 0)
        {
            RatingFilterButton.BackgroundColor = Colors.Yellow;
            RatingFilterButton.TextColor = Colors.Black;
        }
        else
        {
            RatingFilterButton.BackgroundColor = Color.FromArgb("#FEFCE8");
            RatingFilterButton.TextColor = Color.FromArgb("#92400E");
        }
        
        await ApplyFilters();
    }

    // app://menu/{id} URL'lerini yakalayıp MenuPage'e yönlendir
    private async void MapWebView_Navigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("app://menu/", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            var idStr = e.Url.Replace("app://menu/", "");
            if (int.TryParse(idStr, out var id))
            {
                var restaurant = _allRestaurants.FirstOrDefault(x => x.Id == id);
                if (restaurant != null)
                {
                    await Shell.Current.GoToAsync(nameof(MenuPage), new Dictionary<string, object> { { "Restaurant", restaurant } });
                }
            }
        }
    }

    private async void CenterOnIstanbul_Clicked(object sender, EventArgs e)
    {
        try
        {
            await MapWebView.EvaluateJavaScriptAsync("window.centerOnIstanbul();");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Harita merkezlenirken hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void ShowAllRestaurants_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Tüm filtreleri sıfırla
            _showOnlyOpen = false;
            _priceFilter = 0;
            _ratingFilter = 0;
            SearchEntry.Text = "";
            
            // Arama metnini temizle
            SearchEntry.Text = "";
            
            // Buton renklerini sıfırla
            OpenFilterButton.BackgroundColor = Color.FromArgb("#E8F5E8");
            OpenFilterButton.TextColor = Color.FromArgb("#166534");
            PriceFilterButton.BackgroundColor = Color.FromArgb("#E0F2FE");
            PriceFilterButton.TextColor = Color.FromArgb("#0C4A6E");
            RatingFilterButton.BackgroundColor = Color.FromArgb("#FEFCE8");
            RatingFilterButton.TextColor = Color.FromArgb("#92400E");
            
            // Tüm restoranları göster
            _filteredRestaurants = new List<Restaurant>(_allRestaurants);
            UpdateRestaurantCount();
            await UpdateMap();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Restoranlar gösterilirken hata oluştu: {ex.Message}", "Tamam");
        }
    }
}
