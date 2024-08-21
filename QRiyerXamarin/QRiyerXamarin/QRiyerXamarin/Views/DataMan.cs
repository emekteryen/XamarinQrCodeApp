using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using static Android.Content.Res.Resources;
using static Android.Graphics.Paint;
using Xamarin.Essentials;
using QRiyerXamarin.Views;
using Xamarin.Forms;

using System.Globalization;
using System.Linq;

namespace QRiyerXamarin.Views
{
    public class Etkinlik : BindableObject
    {
        public int etkinlikid { get; set; }
        public long telefonno { get; set; }
        public string etkinlikad { get; set; }
        public string tür { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string mail { get; set; }
        public string tarih { get; set; }
        public int katilimci { get; set; }

        private Color _randomColor;
        public Color RandomColor
        {
            get { return _randomColor; } //{8}
            set { _randomColor = value; OnPropertyChanged(); }
        }
    }
    //kod danışmanı ile beraber yapılan yerler mevcut
    public class EtkinlikId
    {
        public int Id { get; set; }
        public int Etkinlikid { get; set; }
    }
    public class DatabaseMan
    {
        private readonly string _connectionString;

        public DatabaseMan(string connectionString)
        {
            _connectionString = $"{connectionString};AllowZeroDateTime=True";
        }

        public List<int> etkinlikAl(string telno)
        {
            List<int> etkinlikIds = new List<int>();
            //https://medium.com/a-developer-in-making/how-to-work-with-collectionview-in-xamarin-forms-5dc65c50b419
            //{11}
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT etkinlikid FROM etkinlik WHERE telno = @telno";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@telno", telno);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        etkinlikIds.Add(Convert.ToInt32(reader["etkinlikid"]));
                    }
                }
                connection.Close();
            }
            return etkinlikIds;
        }

        public List<Etkinlik> dataAl(List<int> etkinlikIds)
        {
            List<Etkinlik> etkinlikData = new List<Etkinlik>();
            if (etkinlikIds.Count == 0)
                return etkinlikData;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                //mysqlconnector apisi web sitesinden alınan yerler mevcut
                string query = $"SELECT * FROM yenietkinlik WHERE etkinlikid IN ({string.Join(",", etkinlikIds)}) ORDER BY tarih";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Etkinlik etkinlik = new Etkinlik
                        {
                            etkinlikid = Convert.ToInt32(reader["etkinlikid"]),
                            etkinlikad = reader["etkinlikad"].ToString(),
                            tür = reader["tür"].ToString(),
                        };
                        //https://stackoverflow.com/questions/43368614/date-in-c-mysql
                        //{12}
                        string mysqlDateTime = reader["tarih"].ToString();
                        if (mysqlDateTime == null || mysqlDateTime == "0000-00-00" || mysqlDateTime == "0000-00-00 00:00:00")
                        {
                            etkinlik.tarih = null;
                        }
                        else
                        {
                            DateTime parsedDateTime;
                            if (DateTime.TryParse(mysqlDateTime, out parsedDateTime))
                            {
                                etkinlik.tarih = parsedDateTime.ToString();
                            }
                            else
                            {
                                etkinlik.tarih = null;
                            }
                        }
                        etkinlikData.Add(etkinlik);
                    }
                }
                connection.Close();
            }
            return etkinlikData;
        }
        public List<Etkinlik> etkAl(List<int> etkinlikIds)
        {
            List<Etkinlik> etkinlikData = new List<Etkinlik>();
            if (etkinlikIds.Count == 0)
                return etkinlikData;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM yenietkinlik WHERE etkinlikid IN ({string.Join(",", etkinlikIds)}) AND tarih < @currentDateTime ORDER BY tarih";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@currentDateTime", DateTime.Now);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Etkinlik etkinlik = new Etkinlik
                        {
                            etkinlikid = Convert.ToInt32(reader["etkinlikid"]),
                            etkinlikad = reader["etkinlikad"].ToString(),
                            tür = reader["tür"].ToString(),
                            katilimci = 0
                        };
                        string mysqlDateTime = reader["tarih"].ToString();
                        if (mysqlDateTime == null || mysqlDateTime == "0000-00-00" || mysqlDateTime == "0000-00-00 00:00:00")
                        {
                            etkinlik.tarih = null;
                        }
                        else
                        {
                            DateTime parsedDateTime;
                            if (DateTime.TryParse(mysqlDateTime, out parsedDateTime))
                            {
                                etkinlik.tarih = parsedDateTime.ToString();
                            }
                            else
                            {
                                etkinlik.tarih = null;
                            }
                        }
                        etkinlikData.Add(etkinlik);
                    }
                }
            }
            katilim(etkinlikData);
            return etkinlikData;
        }
        public void katilim(List<Etkinlik> etkinlikData)
        {
            if (etkinlikData == null || etkinlikData.Count == 0)
                return;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                //query için uzmandan yardım alındı
                string ids = string.Join(",", etkinlikData.Select(e => e.etkinlikid));
                string query = $@"
            SELECT etkinlikid, COUNT(*) AS katilimci
            FROM etkinlik
            WHERE etkinlikid IN ({ids})
            GROUP BY etkinlikid";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int etkinlikid = Convert.ToInt32(reader["etkinlikid"]);
                        int katilimci = Convert.ToInt32(reader["katilimci"]);
                        Etkinlik etkinlik = etkinlikData.FirstOrDefault(e => e.etkinlikid == etkinlikid);
                        if (etkinlik != null)
                        {
                            etkinlik.katilimci = katilimci;
                        }
                    }
                }
            }
        }


        public List<long> userAl(long id)
        {
            List<long> telno = new List<long>();
            //https://medium.com/a-developer-in-making/how-to-work-with-collectionview-in-xamarin-forms-5dc65c50b419
            //{11}
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT telno FROM etkinlik WHERE etkinlikid = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        telno.Add(Convert.ToInt64(reader["telno"]));
                    }
                }
                connection.Close();
            }
            return telno;
        }
        public List<Etkinlik> userTopla(List<long> telno)
        {
            List<Etkinlik> etkinlikData = new List<Etkinlik>();
            if (telno.Count == 0)
                return etkinlikData;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                //mysqlconnector apisi web sitesinden alınan yerler mevcut
                string query = $"SELECT * FROM user WHERE telno IN ({string.Join(",", telno)}) ORDER BY ad";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Etkinlik etkinlik = new Etkinlik
                        {
                            telefonno = Convert.ToInt64(reader["telno"]),
                            ad = reader["ad"].ToString(),
                            soyad = reader["soyad"].ToString(),
                            mail = reader["mail"].ToString()
                        };
                       
                        etkinlikData.Add(etkinlik);
                    }
                }
                connection.Close();
            }
            return etkinlikData;
        }

    }
}
