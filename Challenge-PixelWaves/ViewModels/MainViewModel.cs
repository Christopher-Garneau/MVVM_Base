using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Challenge_PixelWaves.Utils;
using Challenge_PixelWaves.Utils.Commands;
using Challenge_PixelWaves.Services.Interfaces;


namespace Challenge_PixelWaves.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private INavigationService _navigationService;

        public INavigationService NavigationService
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigationService.NavigateTo<PixelGeneratorViewModel>();
            System.Diagnostics.Debug.WriteLine("CurrentView: " + NavigationService.CurrentView?.GetType().FullName);
        }
    }
}


