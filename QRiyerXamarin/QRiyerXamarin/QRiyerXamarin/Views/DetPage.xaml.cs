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
    public partial class DetPage : ContentPage
    {
        string myValue;
        //long ogrid;
        string ogrid;
        long id;
        public DetPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            DbData.ItemsSource = null;
            infotxt.Text = "Lütfen yukarıdaki açılır listeden etkinlik seçimi yapınız.";
            if (picker.Items.Count > 0)
            {
                picker.SelectedItem = null;
                picker.Items.Clear();
            }
            myValue = Preferences.Get("logid", "default_value");
            kayit.IsVisible = false;
            PopulatePicker();
        }
        private void PopulatePicker()
        {
            try
            {
                string connectionString = Properties.Resources.db_con;
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM yenietkinlik where admintel='" + myValue + "'";
                    Console.WriteLine($"Executing SQL query: {query}");
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var dataReader = cmd.ExecuteReader())
                        {
                            if (!dataReader.HasRows)
                            {
                                infotxt.Text = "Kayıtlı Bilgi bulunamadı";
                                return;
                            }
                                while (dataReader.Read())
                            {
                                string id = dataReader["etkinlikid"].ToString();
                                string name = dataReader["etkinlikad"].ToString();
                                picker.Items.Add(id + "|" + name);
                            }
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
        private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker.SelectedItem != null)
            {
                string selectedItem = picker.SelectedItem.ToString();
                string[] parts = selectedItem.Split('|');
                id = int.Parse(parts[0]);
                string name = parts[1];
                infotxt.Text = "Seçilen Etkinlik Id: " + id + " - Adı: " + name;
                kayit.IsVisible = false;
                string connectionString2 = Properties.Resources.db_con2;
                using (MySqlConnection connection2 = new MySqlConnection(connectionString2))
                {
                    try
                    {
                        connection2.Open();
                        string query2 = "SELECT COUNT(*) FROM etkinlik WHERE etkinlikid ='"+id+"'";
                        MySqlCommand cmd2 = new MySqlCommand(query2, connection2);
                        int count = Convert.ToInt32(cmd2.ExecuteScalar());
                        if (count==0)
                        {
                            DbData.ItemsSource= null;
                            kayit.IsVisible = true;
                            kayit.Text = "Kayıtlı öğrenci bulunamadı";
                            return;
                        }
                        connection2.Close();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Uyarı", ex.Message, "Tamam");
                    }
                }


                string connectionString = Properties.Resources.db_con2;
                var databaseManager = new DatabaseMan(connectionString);

                List<long> idtel = databaseManager.userAl(id);
                List<Etkinlik> etkinlikData = await Task.Run(() => databaseManager.userTopla(idtel));
                Random random = new Random(); //{8}
                foreach (var etkinlik in etkinlikData)
                {
                    Color randomColor = Color.FromRgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                    etkinlik.RandomColor = randomColor;
                }
                DbData.ItemsSource = etkinlikData;


            }
            else
            {
                infotxt.Text = "Seçim Yapılmadı";
            }
        }
    }
}