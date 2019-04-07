using CQRSExample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace CQRSExample.FrontEnd.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand GetCustomersCommand { get; private set; }
        public ObservableCollection<Customer> Customers { get; set; }
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
