using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Challenge_Base.ViewModels;
using Challenge_Base.Utils;
using Challenge_Base.Data.Repositories;
using Challenge_Base.Data.Repositories.Interfaces;
using Challenge_Base.Services;
using Challenge_Base.Services.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
namespace Challenge_Base
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            IConfigurationService configurationService = new ConfigurationService();

            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configurationService.GetLogConfigPath());
            LogManager.Configuration.Variables["logFilePath"] = Environment.ExpandEnvironmentVariables(configurationService.GetLogFilePath());
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            });

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<PersonsViewModel>();
            services.AddSingleton<NewPersonViewModel>();
            services.AddSingleton<NewAddressViewModel>();

            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
            {
                BaseViewModel ViewModelFactory(Type viewModelType)
                {
                    return (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
                }
                return ViewModelFactory;
            });

            services.AddDbContext<ApplicationDbContext>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (dbContext.Database.EnsureCreated())
                {
                    dbContext.SeedData();
                }
            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
