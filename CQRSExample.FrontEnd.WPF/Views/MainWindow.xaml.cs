using CQRSExample.Commands;
using CQRSExample.DataAccessLayer;
using CQRSExample.Models;
using CQRSExample.Mvvm;
using CQRSExample.Queries;
using ExtensionsAndHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CSharp.RuntimeBinder;
using RBinder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace CQRSExample.FrontEnd.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DapperMediator _mediator;
        private string _name = "";
        public MainWindow(DapperMediator mediator)
        {
            _mediator = mediator;
            InitializeComponent();

            dynamic myObj = new ExpandoObject();
            var binder = RBinder.SetMember(CSharpBinderFlags.None, "Name",
                typeof(object),
                new List<CSharpArgumentInfo>() {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                });
            var callsite = CallSite<Func<CallSite, object, object, object>>.Create(binder);
            callsite.Target(callsite, myObj, "value");

        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {

            var propDict = typeof(AddCustomer).GetProperties().ToDictionary(p => p.Name, p => p);
            //var test = typeof(AddCustomer).BuildDynamicTypeWithProperties(propDict);
            //var testInstance = Activator.CreateInstance(test);
            var customer = new AddCustomer()
            {
                ID = Guid.NewGuid(),
                Firstname = "testt",
                Lastname = "testt"
            };
            var wrapped = customer.Wrap();
            var test = wrapped["ID"];
            wrapped["ID"] = Guid.NewGuid();

            Console.WriteLine(test);
            //dynamic expando = new expandable<AddCustomer>(ref customer);
            //expando.Firstname = "test13";
            //var t = expando.Firstname;
            //foreach (var item in expando.GetType().GetProperties())
            //{
            //    Console.WriteLine(item.Name);
            //}
            //foreach (var property in test.GetProperties())
            //{
            //    property.SetValue(testInstance, typeof(AddCustomer).GetProperty(property.Name).GetValue(customer));
            //}
            //var observable = new ObservableObject<AddCustomer>(customer);
            //var results = _mediator.Send<AddCustomer, object>(customer);
            
            //Application.Current.Shutdown();
        }

        private class expandable<TType> : DynamicObject, INotifyPropertyChanged //IDynamicMetaObjectProvider
        {
            Dictionary<string, PropertyInfo> properties;
            TType _target;
            public event PropertyChangedEventHandler PropertyChanged;
            private void RaisePropertyChanged([CallerMemberName] string propertyName = "") 
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public expandable(ref TType target)
            {
                _target = target;
                properties = typeof(TType).GetProperties().ToDictionary(p => p.Name, p => p);
            }
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                // binder.Bind()
                properties[binder.Name].SetValue(_target, value);
                // var resultToReturn = base.TrySetMember(binder, value);
                //if (resultToReturn)
                    RaisePropertyChanged();
                return true;
            }
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var containsProperty = properties.ContainsKey(binder.Name);
                result = containsProperty ? properties[binder.Name].GetValue(_target) : null;
                return containsProperty;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                result = typeof(TType).GetMethod(binder.Name)?.Invoke(_target, args);
                return true; // base.TryInvokeMember(binder, args, out result);
            }

            public new Type GetType() => _target.GetType();
            //public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
            //{
            //    throw new NotImplementedException();
            //}
        }
    }
}
