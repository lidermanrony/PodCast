using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PodCast.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private bool isWaiting = false;
        private string username;
        private string password;

        public string Password
        {
            get { return password; }
            set { this.SetProperty(ref this.password, value); }
        }

        public string Username
        {
            get { return username; }
            set { this.SetProperty(ref this.username, value); }
        }

        public bool IsWaiting {
            get { return isWaiting; }
            set { this.SetProperty<bool>(ref this.isWaiting, value); } 
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
