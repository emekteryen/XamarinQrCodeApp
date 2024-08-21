using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Essentials;
using System.Text;

namespace QRiyerXamarin.ViewModels
{
    public class PopItems : INotifyPropertyChanged
    {
        public ObservableCollection<string> PickerItems { get; set; }
        string myValue;
        public PopItems()
        {
            PickerItems = new ObservableCollection<string>();
            PopulatePicker();
        }

        public void PopulatePicker()
        {
            PickerItems.Clear();
            myValue = Preferences.Get("logid", "default_value");
            var items = GetItemsFromDatabase();
            foreach (var item in items)
            {
                PickerItems.Add(item);
            }
        }

        private IEnumerable<string> GetItemsFromDatabase()
        {
            var items = new List<string>();

            string connectionString = "server=your_server_address;port=your_port;database=your_database;user=your_username;password=your_password";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM yenietkinlik where admintel='" + myValue + "'";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string id = reader["etkinlikid"].ToString();
                                string name = reader["etkinlikad"].ToString();
                                items.Add(id + "|" + name);
                                //items.Add(reader.GetString("etkinlikid")); // Replace "ItemName" with your column name
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching data: " + ex.Message);
                }
            }

            return items;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
