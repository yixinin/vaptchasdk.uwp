using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace VaptchaSdk
{
    public class DispatcherManager
    {
        private CoreDispatcher _dispatcher;
        public CoreDispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
            set
            {
                _dispatcher = value;
            }
        }

        private static DispatcherManager _current;
        public static DispatcherManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DispatcherManager();
                }
                return _current;
            }
        }
    }
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                if (DispatcherManager.Current.Dispatcher == null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    if (DispatcherManager.Current.Dispatcher.HasThreadAccess)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                    else
                    {

                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

                    }
                }
            }
        }
    }
}
