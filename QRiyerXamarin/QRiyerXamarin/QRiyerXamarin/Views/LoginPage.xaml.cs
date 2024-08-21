using QRiyerXamarin.ViewModels;
using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }
        private async void Login_Clicked(object sender, EventArgs e)
        {
            using (var con = new MySqlConnection(Properties.Resources.db_con2))
            {
                con.Open();
                string sql = "SELECT * FROM user WHERE telno='" + telno.Text + "' and sifre='" + sifre.Text + "'";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    Preferences.Set("logid", telno.Text);
                    MessagingCenter.Send<object, long>(this, "adminid", Convert.ToInt64(telno.Text));
                    con.Close();
                    await DisplayAlert("Bilgi", "Giriş Başarılı", "Tamam");
                    await Shell.Current.GoToAsync("//AboutPage");
                }
                else
                {
                    await DisplayAlert("Dikkat", "Telefon numarası veya şifre yanlış", "Tamam");
                }
            }
            using (var con1 = new MySqlConnection(Properties.Resources.db_con2))
            {
                con1.Open();
                string sql1 = "SELECT admin FROM user WHERE telno='"+telno.Text+"'";
                using (var cmd1 = new MySqlCommand(sql1, con1))
                {
                    cmd1.Parameters.AddWithValue("@telno", telno.Text);
                    using (var rd1 = cmd1.ExecuteReader())
                    {
                        if (rd1.Read())
                        {
                            bool isAdmin = rd1.GetBoolean("admin");
                            MessagingCenter.Send<object, bool>(this, "AdminPreferenceChanged", isAdmin);
                            Preferences.Set("admin", isAdmin);
                        }
                    }
                }
            }
        }
        private async void Signup_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Bilgi", "Kayıt ekranına yönlendiriliyorsunuz", "Tamam");
            await Shell.Current.GoToAsync("//SignupPage");
        }
    }
}