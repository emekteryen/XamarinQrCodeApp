using MySqlConnector;
using QRiyerXamarin.ViewModels;
using QRiyerXamarin.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using static Android.App.Assist.AssistStructure;

namespace QRiyerXamarin
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("UserUpd", typeof(UserUpd));
            Routing.RegisterRoute("EtkinUpd", typeof(EtkinUpd));
            Routing.RegisterRoute("BarcodeGen", typeof(BarcodeGen));
            Appearing += OnAppShellAppearing;
            BindingContext = new ShellViewModel();
        }
        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            if (Shell.Current.BindingContext is ShellViewModel viewModel)
            {
                viewModel.IsAdmin = false;
                
            }
            await Shell.Current.GoToAsync("//LoginPage");
        }
        private void OnAppShellAppearing(object sender, System.EventArgs e)
        {
            if (BindingContext is ShellViewModel viewModel)
            {
                foreach (var item in Items)
                {
                    if (item is FlyoutItem flyoutItem)
                    {
                        switch (flyoutItem.Title)
                        {
                            case "Barkod Oluşturucu":
                            case "Veri Kaydetme":
                            case "Etkinlik Güncelleme":
                                flyoutItem.IsVisible = viewModel.IsAdmin;
                                break;
                        }
                    }
                }
            }
        }
    }
}
