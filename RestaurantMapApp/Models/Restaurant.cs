namespace RestaurantMapApp.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string MenuUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public List<string> CuisineTypes { get; set; } = new();
    public double Rating { get; set; }
    public int PriceRange { get; set; } // 1-4 arası
    public bool IsOpen { get; set; } = true;
    
    // Yeni eklenen menü alanları
    public List<MenuItem> MenuItems { get; set; } = new();
    public string SearchText => $"{Name} {string.Join(" ", CuisineTypes)} {Address}".ToLowerInvariant();
}

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty; // Ana Yemek, Tatlı, İçecek, vb.
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
} 