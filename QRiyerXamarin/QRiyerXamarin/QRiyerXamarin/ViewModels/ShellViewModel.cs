using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QRiyerXamarin.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        private bool isAdmin;
        //Proje danışmanı ile düzenlenen yerler bulunmakta
        public bool IsAdmin
        {
            get { return isAdmin; }
            set
            {
                if (isAdmin != value)
                {
                    isAdmin = value;
                    OnPropertyChanged(nameof(IsAdmin));
                    OnPropertyChanged(nameof(BarkodOluşturucuVisible));
                    OnPropertyChanged(nameof(VeriKaydetmeVisible));
                    OnPropertyChanged(nameof(EtkinlikGuncellemeVisible));
                }
            }
        }
        public bool BarkodOluşturucuVisible => IsAdmin;
        public bool VeriKaydetmeVisible => IsAdmin;
        public bool EtkinlikGuncellemeVisible => IsAdmin;
        public ShellViewModel()
        {
            IsAdmin = Preferences.Get("admin", false);
            MessagingCenter.Subscribe<object, bool>(this, "AdminPreferenceChanged", OnAdminPreferenceChanged);
        }
        private void OnAdminPreferenceChanged(object sender, bool newAdminValue)
        {
            IsAdmin = newAdminValue;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}