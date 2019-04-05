using CQRSExample.Commands;
using CQRSExample.DataAccessLayer;
using CQRSExample.Infrastructure;
using CQRSExample.Models;
using CQRSExample.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.DataAnnotations;

namespace CQRSExample.FrontEnd.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            base.OnStartup(e);
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DapperMediator>();
            services.AddScoped<MainWindow>();
            services.AddScoped<IQueryHandler<AddCustomer, object>, AddCustomerHandler>();
            services.AddScoped<IQueryHandler<GetCustomers, Customer[]>, GetCustomersHandler>();
        }
    }
}
