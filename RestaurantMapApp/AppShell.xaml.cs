using RestaurantMapApp.Services;

namespace RestaurantMapApp
{
    public partial class AppShell : Shell
    {
        public AppShell(IRestaurantService restaurantService)
        {
            InitializeComponent();
            
            // MainPage'i dependency injection ile oluştur
            var mainPage = new Views.MainPage(restaurantService);
            var shellContent = new ShellContent
            {
                Title = "Home",
                Content = mainPage,
                Route = "MainPage"
            };
            
            // Mevcut ShellContent'ı değiştir
            Items.Clear();
            Items.Add(shellContent);
        }
    }
}
