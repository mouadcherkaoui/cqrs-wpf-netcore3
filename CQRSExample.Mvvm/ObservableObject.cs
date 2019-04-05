using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CQRSExample.Mvvm
{
    public class ObservableObject<TType> : INotifyPropertyChanged
    {
        public ObservableObject(TType instance)
        {
            var propDict = typeof(TType).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(p => p, p => p.GetValue(instance));
            typeof(TType).GetProperties().ToDictionary(p => p.Name, p => new { Getter = p.GetGetMethod(), Setter = p.GetSetMethod() });
        }
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
