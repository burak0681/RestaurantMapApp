using RestaurantMapApp.Services;
using System.Text;

namespace RestaurantMapApp
{
    public partial class App : Application
    {
        public App()
        {
            // UTF-8 encoding desteği
            Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("test"));
            
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Service provider'ı al
            var serviceProvider = MauiProgram.Services;
            var restaurantService = serviceProvider?.GetService<IRestaurantService>();
            
            if (restaurantService == null)
            {
                throw new InvalidOperationException("RestaurantService bulunamadı");
            }
            
            return new Window(new AppShell(restaurantService));
        }
    }
}