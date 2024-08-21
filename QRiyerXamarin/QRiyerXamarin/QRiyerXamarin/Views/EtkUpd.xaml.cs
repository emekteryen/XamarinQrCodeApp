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

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EtkUpd : ContentPage
    {
        public EtkUpd()
        {            
            InitializeComponent();
            this.Appearing += etkupd_Appearing;
        }
        string myValue;
        private void etkupd_Appearing(object sender, System.EventArgs e)
        {
            etkPicker.Items.Clear();
            myValue = Preferences.Get("logid", "default_value");
            etkgetir();
            etkinlikAdtext.Text = "";
            türtext.Text = "";
        }
        private async void etkgetir()
        {
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT etkinlikid FROM yenietkinlik where admintel='"+myValue+"'";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        etkPicker.Items.Add(id.ToString());
                    }
                    connection.Close();                   
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
            }
        }
        private async void etkPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (etkPicker.SelectedIndex == -1) return;
            string etkid = etkPicker.SelectedItem.ToString();
            int etkinlikID = int.Parse(etkid);
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT etkinlikad, tür, tarih FROM yenietkinlik WHERE etkinlikid = @etkinlikid";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@etkinlikid", etkinlikID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        etkinlikAdtext.Text = reader.GetString(0);
                        türtext.Text = reader.GetString(1);
                        DateTime tarih = reader.GetDateTime(2);
                        TarihAl.Date = tarih.Date;
                        SaatAl.Time = tarih.TimeOfDay;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı",ex.Message,"Tamam");
                }
            }
        }
        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            string etkinlikad = etkinlikAdtext.Text;
            string tür = türtext.Text;
            string selectedEtkinlikId = etkPicker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(etkinlikad) || string.IsNullOrEmpty(tür) || string.IsNullOrEmpty(selectedEtkinlikId))
            {
                await DisplayAlert("Uyarı", "Lütfen tüm bilgileri doldurun", "Tamam");
                return;
            }
            if (DateTime.Now > TarihAl.Date + SaatAl.Time)
            {
                await DisplayAlert("Uyarı", "Tarih Hatalı", "Tamam");
                return;
            }


            int etkinlikID = int.Parse(selectedEtkinlikId);
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE yenietkinlik SET etkinlikad=@value2, tür=@value3, tarih=@value4 WHERE etkinlikid=@value1";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //mysqlconnector ile datetime komutu düzenlendi {9}
                    DateTime chosenDateTime = new DateTime(TarihAl.Date.Year, TarihAl.Date.Month, TarihAl.Date.Day,
                                        SaatAl.Time.Hours, SaatAl.Time.Minutes, SaatAl.Time.Seconds);
                    string tarihsaat = chosenDateTime.ToString("yyyy-MM-dd HH:mm");
                    cmd.Parameters.AddWithValue("@value1", etkinlikID);
                    cmd.Parameters.AddWithValue("@value2", etkinlikad);
                    cmd.Parameters.AddWithValue("@value3", tür);
                    cmd.Parameters.AddWithValue("@value4", tarihsaat);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    await DisplayAlert("Bilgi", "Etkinlik Güncellemesi Başarılı", "Tamam");
                    türtext.Text = "";
                    etkinlikAdtext.Text = "";
                    etkPicker.SelectedItem = null;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
  
            }
        }
    }

}
