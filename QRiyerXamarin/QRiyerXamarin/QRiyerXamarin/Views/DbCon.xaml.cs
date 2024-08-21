using Java.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using Android.Telecom;
using static System.Net.Mime.MediaTypeNames;
using System.Net;

namespace QRiyerXamarin.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DbCon : ContentPage
	{
        public DbCon ()
		{
			InitializeComponent ();
            this.Appearing += etkupd_Appearing;
		}
        string admintel;
         
        private void etkupd_Appearing(object sender, System.EventArgs e)
        {
            admintel = Preferences.Get("logid", "default_value");
        }
        private async void DbButton_Clicked(object sender, EventArgs e)
		{
            string etkinlikad = etkinlikAdtext.Text;
            string tür = türtext.Text;

            if (string.IsNullOrEmpty(etkinlikad) || string.IsNullOrEmpty(tür))
            {
                await DisplayAlert("Uyarı", "Lütfen tüm bilgileri doldurun", "Tamam");
                return;
            }
            if(DateTime.Now>TarihAl.Date+SaatAl.Time)
            {
                await DisplayAlert("Uyarı", "Tarih Hatalı", "Tamam");
                return;
            }
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "insert into yenietkinlik(etkinlikad,tür,tarih,admintel) values (@value2,@value3,@value4,@value5)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    DateTime secilentarih = new DateTime(TarihAl.Date.Year, TarihAl.Date.Month, TarihAl.Date.Day,
                                        SaatAl.Time.Hours, SaatAl.Time.Minutes, SaatAl.Time.Seconds);
                    string tarihsaat = secilentarih.ToString("yyyy-MM-dd HH:mm");
                    cmd.Parameters.AddWithValue("@value2", etkinlikad.ToString()); ;
                    cmd.Parameters.AddWithValue("@value3", tür.ToString()); ;
                    cmd.Parameters.AddWithValue("@value4", tarihsaat); ;
                    cmd.Parameters.AddWithValue("@value5", admintel); ;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    await DisplayAlert("Bilgi", "Etkinlik Kaydı Başarılı", "Tamam");
                    connection.Close();
                    await Shell.Current.GoToAsync("//BarcodeGen");
                }
                catch (Exception ex)
                {
                    dbadd.Text = ex.Message;
                }                
            }
        }       
	}
}