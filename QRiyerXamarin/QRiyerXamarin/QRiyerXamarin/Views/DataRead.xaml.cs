using Java.Lang;
using Java.Net;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Net.Http.SslCertificate;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataRead : ContentPage
    {
        public DataRead()
        {
            InitializeComponent();
            this.Appearing += DataAppearing; //{7}
        }
        //proje danışmanı ile yapılan yerler mevcut
        private void DataAppearing(object sender, System.EventArgs e)
        {
            infotxt.IsVisible = false;
            var myValue = Preferences.Get("logid", "default_value");
            string connectionString = Properties.Resources.db_con2;
            var databaseManager = new DatabaseMan(connectionString);
            string telno = myValue;
            List<int> etkinlikIds = databaseManager.etkinlikAl(telno);
            List<Etkinlik> etkinlikData = databaseManager.dataAl(etkinlikIds);
            if (etkinlikData.Count == 0)
            {
                infotxt.IsVisible = true;
                infotxt.Text = "Kayıtlı Bilgi Bulunamadı";
                return;
            }

            Random random = new Random(); //{8}
            foreach (var etkinlik in etkinlikData)
            {
                Color randomColor = Color.FromRgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
                etkinlik.RandomColor = randomColor;
            }
            DbData.ItemsSource = etkinlikData;
        }
    }
}