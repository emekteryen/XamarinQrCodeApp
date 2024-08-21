using Android.Content;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfile : ContentPage
    {
        public UserProfile()
        {
            InitializeComponent();
            this.Appearing += UsrAppearing;
        }
        string loginId;
        private void UsrAppearing(object sender, System.EventArgs e)
        {
            loginId = Preferences.Get("logid", "default_value");
            LoadUserData();
            eskisifre.IsVisible = false;
            eskisifretext.IsVisible = false;
            yenisifre.IsVisible = false;
            yenisifretext.IsVisible = false;
            yenitekrar.IsVisible = false;
            yenitekrartext.IsVisible = false;
            yenile.IsVisible = false;
        }
        private async void LoadUserData()
        {
            //var loginId = Preferences.Get("logid", string.Empty);
            //Eskino.Text=loginId.ToString();
            teltext.Text = loginId;
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
                        teltext.Text = loginId.ToString();
                        adtext.Text = reader.GetString(0);
                        soyadtext.Text = reader.GetString(1);
                        mailtext.Text = reader.GetString(3);
                        long tcNo = reader.GetInt64(2);
                        tctext.Text = tcNo.ToString();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
            }
        }
        private async void UsrUpd_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Bilgi", "Profil Güncelleme ekranına yönlendiriliyorsunuz", "Tamam");
            await Shell.Current.GoToAsync("UserUpd");
        }
        private void sifreyenile_Clicked(object sender, EventArgs e)
        {
            eskisifre.IsVisible= true;
            eskisifretext.IsVisible= true;
            yenisifre.IsVisible= true;
            yenisifretext.IsVisible= true;
            yenitekrar.IsVisible = true;
            yenitekrartext.IsVisible= true;
            yenile.IsVisible= true;

        }
        private async void sifre_Clicked(object sender, EventArgs e)
        {
            if (yenisifretext.Text != yenitekrartext.Text)
            {
                await DisplayAlert("UYARI", "Girdiğiniz şifreler eşleşmiyor", "Tamam");
                return;
            }
            string connectionString2 = Properties.Resources.db_con2;
            using (MySqlConnection connection2 = new MySqlConnection(connectionString2))
            {
                try
                {
                    connection2.Open();
                    string query2 = "SELECT sifre from user WHERE telno = @telno";
                    MySqlCommand cmd2 = new MySqlCommand(query2, connection2);
                    cmd2.Parameters.AddWithValue("@telno", loginId);
                    MySqlDataReader reader = cmd2.ExecuteReader();
                    if (reader.Read())
                    {
                        var sifre = reader.GetString(0);
                        if(sifre!=eskisifretext.Text)
                        {
                            await DisplayAlert("UYARI","Eski Şifrenizi Yanlış Girdiniz","Tamam");
                            return;
                        }
                    }
                    connection2.Close();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
            }
            string connectionString = Properties.Resources.db_con2;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE user SET sifre=@sifre WHERE telno = @telno";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@telno", loginId);
                    var sifre = yenisifretext.Text;
                    cmd.Parameters.AddWithValue("@sifre", yenisifretext.Text);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    eskisifretext.Text = "";
                    yenisifretext.Text = "";
                    yenitekrartext.Text = "";
                    await DisplayAlert("UYARI","Şifreniz Başarıyla Güncellenmiştir","TAMAM");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Uyarı", ex.Message, "Tamam");
                }
            }
            eskisifre.IsVisible = false;
            eskisifretext.IsVisible = false;
            yenisifre.IsVisible = false;
            yenisifretext.IsVisible = false;
            yenitekrar.IsVisible = false;
            yenitekrartext.IsVisible = false;
            yenile.IsVisible = false;
        }
    }
}