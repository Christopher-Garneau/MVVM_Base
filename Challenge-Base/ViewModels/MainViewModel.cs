using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Challenge_Base.Models;
using Challenge_Base.Data.Repositories;
using Challenge_Base.Utils;
using Challenge_Base.Utils.Commands;
using Challenge_Base.Services.Interfaces;


namespace Challenge_Base.ViewModels
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

        public ICommand NavigateToPersonsViewCommand { get; set; }
        public ICommand NavigateToNewPersonViewCommand { get; set; }

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateToPersonsViewCommand = new RelayCommand(() => NavigationService.NavigateTo<PersonsViewModel>());
            NavigateToNewPersonViewCommand = new RelayCommand(() => NavigationService.NavigateTo<NewPersonViewModel>());
            NavigationService.NavigateTo<PersonsViewModel>();
        }
    }
}

