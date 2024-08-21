using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using static Android.Net.Http.SslCertificate;
using static System.Net.Mime.MediaTypeNames;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeGen : ContentPage
    {
        //api ile yapıldı {5}
        //https://github.com/Redth/ZXing.Net.Mobile
        //https://github.com/Redth/ZXing.Net.Mobile/blob/master/Samples/Sample.Forms/Sample.Forms/BarcodePage.cs
        ZXingBarcodeImageView barcode;
        Xamarin.Forms.Picker picker;
        Label infotxt;
        Label etkadtxt;
        Label etktartxt;
        Label etkturtxt;
        Button guncelle;
        string myValue;
        StackLayout stackLayout;

        public BarcodeGen()
        {
            Title = "QR Oluşturucu";
            this.Appearing += BarcodeGen_Appearing;
            string id;
            stackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            barcode = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                AutomationId = "zxingBarcodeImageView",
            };
            barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            barcode.BarcodeOptions.Width = 300;
            barcode.BarcodeOptions.Height = 300;
            barcode.BarcodeOptions.Margin = 10;
            barcode.BarcodeValue = "ZXing.Net.Mobile";
            picker = new Xamarin.Forms.Picker
            {
                BackgroundColor = Color.Gray,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;
            infotxt = new Label
            {
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Lütfen yukarıdaki açılır listeden etkinlik seçimi yapınız."
            };
            guncelle = new Button
            {
                Text = "Güncelle"
            };
            guncelle.Clicked += guncelle_Clicked;

            etkadtxt = new Label
            {
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Etkinlik Adı: "
            };
            etkturtxt = new Label
            {
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Etkinlik Türü: "
            };
            etktartxt = new Label
            {
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Etkinlik Tarihi: "
            };


            stackLayout.Children.Add(picker);
            stackLayout.Children.Add(infotxt);
            stackLayout.Children.Add(barcode);
            stackLayout.Children.Add(etkadtxt);
            stackLayout.Children.Add(etkturtxt);
            stackLayout.Children.Add(etktartxt);
            stackLayout.Children.Add(guncelle);

            Content = stackLayout;
        }


        private void BarcodeGen_Appearing(object sender, System.EventArgs e)
        {

            barcode.BarcodeValue = "Seçim Yapılmadı";
            infotxt.Text = "Lütfen yukarıdaki açılır listeden etkinlik seçimi yapınız.";
            if (picker.Items.Count > 0)
            {
                picker.SelectedItem = null;
                picker.Items.Clear();
            }
            myValue = Preferences.Get("logid", "default_value");
            PopulatePicker();
            barcode.IsVisible = false;
            etkadtxt.IsVisible = false;
            etkturtxt.IsVisible = false;
            etktartxt.IsVisible = false;
            guncelle.IsVisible = false;
            etkadtxt.Text = "Etkinlik Adı: ";
            etkturtxt.Text = "Etkinlik Türü ";
            etktartxt.Text = "Etkinlik Tarihi ";
        }

        private void PopulatePicker()
        {
            try
            {
                string connectionString = Properties.Resources.db_con2;
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM yenietkinlik where admintel='" + myValue + "' and tarih > @currentDateTime";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@currentDateTime", DateTime.Now);
                    using (var dataReader = command.ExecuteReader())
                    {

                        while (dataReader.Read())
                        {
                            string id = dataReader["etkinlikid"].ToString();
                            string name = dataReader["etkinlikad"].ToString();
                            picker.Items.Add(id + "|" + name);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                infotxt.Text = "Hata: " + ex.Message;
            }
        }
        void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker.SelectedItem != null)
            {
                etkadtxt.Text = "Etkinlik Adı: ";
                etkturtxt.Text = "Etkinlik Türü ";
                etktartxt.Text = "Etkinlik Tarihi ";

                barcode.IsVisible= true;
                etkadtxt.IsVisible = true;
                etkturtxt.IsVisible = true;
                etktartxt.IsVisible = true;
                guncelle.IsVisible = true;
                string selectedItem = picker.SelectedItem.ToString();
                string[] parts = selectedItem.Split('|');
                string id = parts[0];
                string name = parts[1];
                barcode.BarcodeValue = id;
                infotxt.Text = "Seçilen Etkinlik Id: " + id + " - Adı: " + name;
                Preferences.Set("etkid", id);
                string connectionString = Properties.Resources.db_con;
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM yenietkinlik where etkinlikid='" + id + "'";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@currentDateTime", DateTime.Now);
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            etkadtxt.Text = etkadtxt.Text + dataReader["etkinlikad"].ToString();
                            etkturtxt.Text = etkturtxt.Text + dataReader["tür"].ToString();
                            DateTime tarih = (DateTime)dataReader["tarih"];
                            etktartxt.Text = etktartxt.Text + tarih.ToString();
                        }
                    }

                    connection.Close();
                }


            }
            else
            {
                //barcode.BarcodeValue = "Seçim Yapılmadı";
            }
        }
        private async void guncelle_Clicked(object sender, EventArgs e)
        {
            bool cevap = await DisplayAlert("UYARI", "Güncelleme işlemi yapmak istiyor musunuz?", "Evet", "Hayır");
            if (cevap) {

                await Shell.Current.GoToAsync("EtkinUpd"); }
            
        }

    }
}