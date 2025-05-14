using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Challenge_PixelWaves.ViewModels;
using Challenge_PixelWaves.Utils;
using Challenge_PixelWaves.Services;
using Challenge_PixelWaves.Services.Interfaces;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using Microsoft.EntityFrameworkCore;
namespace Challenge_PixelWaves
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
            services.AddSingleton<PixelGeneratorViewModel>();

            services.AddScoped<IPixelGeneratorService, PixelGeneratorService>();
            services.AddScoped<IPixelGeneratorComputationalParamsService, PixelGeneratorComputationalParamsService>();
            services.AddScoped<IPixelGeneratorPhysicParamsService, PixelGeneratorPhysicParamsService>();
            services.AddScoped<IPixelGeneratorService, PixelGeneratorService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
            {
                BaseViewModel ViewModelFactory(Type viewModelType)
                {
                    return (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
                }
                return ViewModelFactory;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                string rawDbPath = configurationService.GetDbPath();
                string dbPath = Environment.ExpandEnvironmentVariables(rawDbPath);
                var connectionString = $"Data Source={dbPath}";
                options.UseSqlite(connectionString);
            });

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

