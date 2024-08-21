using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QRiyerXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataOrder : ContentPage
    {
        public DataOrder()
        {
            InitializeComponent();
            this.Appearing += DataAppearing;
        }
        private async void DataAppearing(object sender, System.EventArgs e)
        {
            infotxt.IsVisible = false;
            var myValue = Preferences.Get("logid", "default_value");
            string connectionString = Properties.Resources.db_con2;
            var databaseManager = new DatabaseMan(connectionString);
            string telno = myValue;
            List<int> etkinlikIds = databaseManager.etkinlikAl(telno);
            List<Etkinlik> etkinlikData = await Task.Run(() => databaseManager.etkAl(etkinlikIds));

            if (etkinlikData.Count == 0)
            {
                infotxt.IsVisible = true;
                infotxt.Text = "Kayıtlı Bilgi Bulunamadı";
                return;
            }

            //List<Etkinlik> etkinlikData = databaseManager.etkAl(etkinlikIds);
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