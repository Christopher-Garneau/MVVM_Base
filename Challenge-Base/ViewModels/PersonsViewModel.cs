using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Challenge_Base.Data.Repositories.Interfaces;
using Challenge_Base.Models;
using Challenge_Base.Services.Interfaces;
using Challenge_Base.Utils;
using Challenge_Base.Utils.Commands;

namespace Challenge_Base.ViewModels
{
    class PersonsViewModel : BaseViewModel
    {
        private readonly IPersonService _personService;
        private readonly INavigationService _navigationService;
        public ObservableCollection<Person> Persons
        {
            get => new ObservableCollection<Person>(_personService.FindAll());
            set;
        }

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
                OnPropertyChanged(nameof(AddressCount));
            }
        }

        public int AddressCount => SelectedPerson?.Addresses?.Count ?? 0;

        public ICommand DeleteCommand { get; set; }
        public ICommand NavigateToNewAddressViewCommand { get; set; }

        public PersonsViewModel(IPersonService personService, INavigationService navigationService)
        {
            _personService = personService;
            _navigationService = navigationService;
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            NavigateToNewAddressViewCommand = new RelayCommand(NavigateToNewAddressView, CanNavigateToNewAddressView);
        }

        private void Delete()
        {
            _personService.Remove(SelectedPerson);
            OnPropertyChanged(nameof(Persons));
        }

        private bool CanDelete()
        {
            return SelectedPerson != null;
        }

        private void NavigateToNewAddressView()
        {
            if (CanNavigateToNewAddressView())
            {
                _navigationService.NavigateTo<NewAddressViewModel>(SelectedPerson);
            }
        }

        private bool CanNavigateToNewAddressView()
        {
            return SelectedPerson != null;
        }
    }
}
