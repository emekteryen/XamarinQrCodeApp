using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserUpd : ContentPage
    {        
        public UserUpd()
        {
            InitializeComponent();
            this.Appearing += UsrAppearing;
        }
        string loginId;
        private void UsrAppearing(object sender, System.EventArgs e)
        {
            loginId = Preferences.Get("logid", "default_value");
            Eskino.Text=loginId.ToString();
            LoadUserData();
        }
            private async void LoadUserData()
        {
            //var loginId = Preferences.Get("logid", string.Empty);
            //Eskino.Text=loginId.ToString();
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ad, soyad, tc_no, mail FROM user WHERE telno = @telno";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@telno", loginId);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        telnoText.Text=loginId.ToString();
                        adText.Text = reader.GetString(0);
                        soyadText.Text = reader.GetString(1);
                        long tcNo = reader.GetInt64(2);
                        tcnoText.Text = tcNo.ToString();
                        mailText.Text = reader.GetString(3);
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
            }           
        }
        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(telnoText.Text) ||
                string.IsNullOrEmpty(adText.Text) ||
                string.IsNullOrEmpty(soyadText.Text) ||
                string.IsNullOrEmpty(tcnoText.Text) ||
                string.IsNullOrEmpty(mailText.Text))
            {
                await DisplayAlert("Uyarı", "Tüm alanları doldurun!", "Tamam");
                return;
            }
            if (!telkontrol(telnoText.Text))
            {
                await DisplayAlert("Uyarı", "Telefon numarası 11 haneli olmalıdır!", "Tamam");
                return;
            }
            var connectionString2 = new MySqlConnection(Properties.Resources.db_con2);
            using (MySqlConnection connection2 = connectionString2)
            {
                try
                {
                    connection2.Open();

                    string query = "UPDATE user SET telno=@telno, ad=@ad, soyad=@soyad, mail=@mail WHERE telno='"+Eskino.Text+"'";

                    MySqlCommand cmd = new MySqlCommand(query, connection2);
                    cmd.Parameters.AddWithValue("@telno", telnoText.Text);
                    
                    cmd.Parameters.AddWithValue("@ad", adText.Text);
                    cmd.Parameters.AddWithValue("@soyad", soyadText.Text);
                    cmd.Parameters.AddWithValue("@mail", mailText.Text);

                    cmd.ExecuteNonQuery();
                    connection2.Close();
                }
                catch (Exception ex)
                {
                    dbupdate.Text = ex.Message;
                    return;
                }
            }
            var connectionString3 = new MySqlConnection(Properties.Resources.db_con2);
            using (MySqlConnection connection3 = connectionString3)
            {
                try
                {
                    connection3.Open();
                    string query = "UPDATE etkinlik SET telno=@telno WHERE telno='" + Eskino.Text + "'";
                    MySqlCommand cmd = new MySqlCommand(query, connection3);
                    cmd.Parameters.AddWithValue("@telno", telnoText.Text);

                    cmd.ExecuteNonQuery();
                    connection3.Close();
                }
                catch (Exception ex)
                {
                    dbupdate.Text = ex.Message;
                    return;
                }
            }
            var connectionString4 = new MySqlConnection(Properties.Resources.db_con2);
            using (MySqlConnection connection4 = connectionString4)
            {
                try
                {
                    connection4.Open();
                    string query = "UPDATE yenietkinlik SET admintel=@admintel WHERE admintel='" + Eskino.Text + "'";
                    MySqlCommand cmd = new MySqlCommand(query, connection4);
                    cmd.Parameters.AddWithValue("@admintel", telnoText.Text);
                    cmd.ExecuteNonQuery();
                    connection4.Close();
                }
                catch (Exception ex)
                {
                    dbupdate.Text = ex.Message;
                    return;
                }
            }
            Preferences.Set("logid", telnoText.Text);
            Eskino.Text= telnoText.Text;
            await DisplayAlert("Bilgi", "Kullanıcı bilgileri güncellendi", "Tamam");
            await Shell.Current.GoToAsync("//UserProfile");
        }
        private bool telkontrol(string telno)
        {
            return telno.Length == 11 && Regex.IsMatch(telno, @"^\d{11}$");
        }
    }
}