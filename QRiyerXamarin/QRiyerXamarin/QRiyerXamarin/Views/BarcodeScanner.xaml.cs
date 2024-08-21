using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using Xamarin.Essentials;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeScanner : ContentPage
    {
        //api ile yapıldı {6}
        //https://github.com/Redth/ZXing.Net.Mobile
        //https://github.com/Redth/ZXing.Net.Mobile/blob/master/Samples/Sample.Forms/Sample.Forms/CustomScanPage.cs 
        ZXingScannerView zxing;
        ZXingDefaultOverlay overlay;
        public BarcodeScanner() : base()
        {
            Title= "QR Tarayıcı";
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingScannerView",
            };
            zxing.OnScanResult += (result) =>
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var myValue = Preferences.Get("logid", "default_value");
                    zxing.IsAnalyzing = false;
                    await DisplayAlert("Okutulan Sertifika ID", result.Text, "Tamam");
                    var connectionString = new MySqlConnection(Properties.Resources.db_con2);
                    using (MySqlConnection connection = connectionString)
                    {
                        connection.Open();
                        string kontrol = "SELECT COUNT(*) FROM yenietkinlik WHERE etkinlikid = @etkinlikid";
                        MySqlCommand checkCmd = new MySqlCommand(kontrol, connection);
                        checkCmd.Parameters.AddWithValue("@etkinlikid", result.Text);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            await DisplayAlert("Uyarı", "Okutmaya çalıştığınız etkinlik ile alakalı bilgi bulunamadı. Lütfen tekrar deneyiniz!", "Tamam");
                            return;
                        }
                        string varmi = "SELECT COUNT(*) FROM etkinlik WHERE etkinlikid = @etkinlikid AND telno='"+myValue+"'";
                        MySqlCommand Commandcheck = new MySqlCommand(varmi, connection);
                        Commandcheck.Parameters.AddWithValue("@etkinlikid", result.Text);
                        int count2 = Convert.ToInt32(Commandcheck.ExecuteScalar());
                        if (count2 != 0)
                        {
                            await DisplayAlert("Uyarı", "Okutmaya çalıştığınız etkinliğe zaten katılmışsınız!", "Tamam");
                            return;
                        }
                        string query = "insert into etkinlik(telno,etkinlikid) values (@value1,@value2)";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        string telno = myValue;
                        string etkinlikid = result.Text;
                        cmd.Parameters.AddWithValue("@value1", telno.ToString()); ;
                        cmd.Parameters.AddWithValue("@value2", etkinlikid.ToString()); ;
                        cmd.ExecuteNonQuery();
                        await Shell.Current.GoToAsync("//DataRead");

                    }
                });
            overlay = new ZXingDefaultOverlay
            {
                TopText = "Telefonunuzu okutacağınız QR'a doğru tutun",
                BottomText = "Tarama işlemi otomatik olarak gerçekleşecek",
                ShowFlashButton = zxing.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };
            overlay.FlashButtonClicked += (sender, e) =>
            {
                zxing.IsTorchOn = !zxing.IsTorchOn;
            };
            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(zxing);
            grid.Children.Add(overlay);            
            Content = grid;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();
        }
        private void DisplayAlertclik(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//DataRead");
        }
    }
}