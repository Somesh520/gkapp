using GKFashionApp.ViewModels;

namespace GKFashionApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            // 🛑 YE LINE BAHUT ZAROORI HAI 🛑
            // Iske bina Buttons aur Data kaam nahi karenge
            this.BindingContext = new HomeViewModel();
        }
    }
}