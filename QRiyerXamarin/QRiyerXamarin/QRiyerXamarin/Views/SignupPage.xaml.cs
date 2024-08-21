using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        public SignupPage()
        {
            InitializeComponent();
        }
        private async void DbButton_Clicked(object sender, EventArgs e)
        {
            if(sifreText.Text!=sifretkrText.Text)
            {
                await DisplayAlert("Uyarı", "Şifreler Eşleşmeli!", "Tamam");
                return;
            }
            if (string.IsNullOrEmpty(telnoText.Text) ||
                string.IsNullOrEmpty(sifreText.Text) ||
                string.IsNullOrEmpty(adText.Text) ||
                string.IsNullOrEmpty(soyadText.Text) ||
                string.IsNullOrEmpty(tcnoText.Text) ||
                string.IsNullOrEmpty(sifretkrText.Text) ||
                string.IsNullOrEmpty(mailText.Text))
            {
                await DisplayAlert("Uyarı", "Kayıt işlemi için tüm alanlar doldurulmalı!", "Tamam");
                return;
            }
            if (!telkontrol(telnoText.Text) || !tckontrol(tcnoText.Text))
            {
                await DisplayAlert("Uyarı", "Telefon numarası veya Tc No 11 haneli olmalıdır!", "Tamam");
                return;
            }
            if (IsDuplicate(telnoText.Text, tcnoText.Text))
            {
                await DisplayAlert("Uyarı", "Aynı telefon numarası veya T.C. Numarası kayıtlı!", "Tamam");
                return;
            }

            var connectionString = new MySqlConnection(Properties.Resources.db_con2);
            using (MySqlConnection connection = connectionString)
            {
                try
                {
                    connection.Open();
                    string query = "insert into user(telno,sifre,ad,soyad,tc_no,mail,admin) values (@value1,@value2,@value3,@value4,@value5,@value6,@value7)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    string no = telnoText.Text;
                    string sifre = sifreText.Text;
                    string ad = adText.Text;
                    string soyad = soyadText.Text;
                    string tc_no = tcnoText.Text;
                    string mail = mailText.Text;
                    //bool admin = false;
                    bool admin = adminkontrol.IsToggled;
                    cmd.Parameters.AddWithValue("@value1", no); ;
                    cmd.Parameters.AddWithValue("@value2", sifre.ToString()); ;
                    cmd.Parameters.AddWithValue("@value3", ad.ToString()); ;
                    cmd.Parameters.AddWithValue("@value4", soyad.ToString()); ;
                    cmd.Parameters.AddWithValue("@value5", tc_no); ;
                    cmd.Parameters.AddWithValue("@value6", mail.ToString()); ;
                    cmd.Parameters.AddWithValue("@value7", admin ? 1 : 0); ;
                    Preferences.Set("logid", telnoText.Text);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    dbadd.Text = ex.Message;
                }
            }
            await DisplayAlert("Bilgi", "Kayıt Başarılı", "Tamam");
            await Shell.Current.GoToAsync("//AboutPage");
        }
        private bool telkontrol(string tel_no)
        {
            return tel_no.Length == 11 && Regex.IsMatch(tel_no, @"^\d{11}$");
        }
        private bool tckontrol(string tc_no)
        {
            return tc_no.Length == 11 && Regex.IsMatch(tc_no, @"^\d{11}$");
        }
        private bool IsDuplicate(string telnoText, string tcnoText)
        {
            string query = "SELECT COUNT(*) FROM user WHERE telno = '"+telnoText+"' OR tc_no = '"+tcnoText+"'";

            using (var connection2 = new MySqlConnection(Properties.Resources.db_con2))
            {
                connection2.Open();

                using (var cmd = new MySqlCommand(query, connection2))
                {
                    cmd.Parameters.AddWithValue("@telno", telnoText);
                    cmd.Parameters.AddWithValue("@tc_no", tcnoText);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;                   
                }
                connection2.Close();
            }

        }
        private void OnSwitchToggled(object switchSender, ToggledEventArgs c)
        {
            bool isAdmin = c.Value;
        }
        private async void geri_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}