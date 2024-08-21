using System;
using MySqlConnector;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Android.Content;

namespace QRiyerXamarin.Views
{
    public partial class AboutPage : ContentPage
    {
        string myValue;
        /*
         * geolocation api ekle
         * arayüzü daha iyi hale getir
         * ana sayfaya yaklaşan etkinlikler ekle
         * sertifikaları image olarak türet
         * imageelri collection view e ekle
         */
        public AboutPage()
        {
            InitializeComponent();
            this.Appearing += about_Appearing;
        }
        private void about_Appearing(object sender, System.EventArgs e)
        {
            myValue = Preferences.Get("logid", "default_value");
            LoadUserData();
        }
        private async void LoadUserData()
        {
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ad, soyad FROM user WHERE telno = @telno";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@telno",myValue);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string ad = reader.GetString(0);
                        string soyad = reader.GetString(1);
                        deneme.Text= ad+" "+soyad;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
            }
        }
    }
}