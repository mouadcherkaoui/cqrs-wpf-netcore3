using CQRSExample.Commands;
using CQRSExample.DataAccessLayer;
using CQRSExample.Models;
using CQRSExample.Queries;
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
            var results = _mediator.Send<AddCustomer, object>(new AddCustomer()
            {
                ID = Guid.NewGuid(),
                Firstname = "test",
                Lastname = "testtesttest"
            });
            
            //Application.Current.Shutdown();
        }
    }
}
