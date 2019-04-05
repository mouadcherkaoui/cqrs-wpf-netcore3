using CQRSExample.Commands;
using CQRSExample.DataAccessLayer;
using CQRSExample.Models;
using CQRSExample.Mvvm;
using CQRSExample.Queries;
using ExtensionsAndHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

namespace CQRSExample.FrontEnd.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DapperMediator _mediator;
        public MainWindow(DapperMediator mediator)
        {
            _mediator = mediator;
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            var test = typeof(AddCustomer).BuildDynamicTypeWithProperties();
            var testInstance = Activator.CreateInstance(test);
            var customer = new AddCustomer()
            {
                ID = Guid.NewGuid(),
                Firstname = "testt",
                Lastname = "testt"
            };

            foreach (var property in test.GetProperties())
            {
                property.SetValue(testInstance, property.GetValue(customer));
            }
            var observable = new ObservableObject<AddCustomer>(customer);
            var results = _mediator.Send<AddCustomer, object>(customer);
            
            //Application.Current.Shutdown();
        }
    }
}
