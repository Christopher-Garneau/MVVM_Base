using Challenge_Base.Data.Repositories.Interfaces;
using Challenge_Base.Models;
using Challenge_Base.Services.Interfaces;
using Challenge_Base.Utils;
using Challenge_Base.Utils.Commands;
using System.Windows.Input;

namespace Challenge_Base.ViewModels
{
    class NewAddressViewModel : BaseViewModel, INavigationParameterReceiver
    {
        private readonly INavigationService _navigationService;
        private readonly IAddressService _addressService;
        private Person _selectedPerson;

        public NewAddressViewModel(INavigationService navigationService, IAddressService addressService)
        {
            _navigationService = navigationService;
            _addressService = addressService;
            SaveCommand = new RelayCommand(Save, CanSave);
        }

        public void ApplyNavigationParameters(params object[] parameters)
        {
            if (parameters?.Length > 0)
            {
                _selectedPerson = parameters[0] as Person;
            }
        }

        private string _street;
        public string Street
        {
            get => _street;
            set
            {
                if (_street != value)
                {
                    _street = value;
                    OnPropertyChanged(nameof(Street));
                    ValidateProperty(nameof(Street), value);
                }
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged(nameof(City));
                    ValidateProperty(nameof(City), value);
                }
            }
        }

        private string _postalCode;
        public string PostalCode
        {
            get => _postalCode;
            set
            {
                if (_postalCode != value)
                {
                    _postalCode = value;
                    OnPropertyChanged(nameof(PostalCode));
                    ValidateProperty(nameof(PostalCode), value);
                }
            }
        }

        public ICommand SaveCommand { get; set; }
        private void Save()
        {
            var newAddress = new Address
            {
                Street = Street,
                City = City,
                PostalCode = PostalCode,
                PersonId = _selectedPerson.Id
            };

            _addressService.Add(_selectedPerson, newAddress);
            _navigationService.NavigateTo<PersonsViewModel>();
        }

        private bool CanSave()
        {
            return !string.IsNullOrEmpty(Street) && !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(PostalCode);
        }

        private void ValidateProperty(string propertyName, string value)
        {
            ClearErrors(propertyName);
            if (string.IsNullOrEmpty(value))
            {
                AddError(propertyName, $"{propertyName} est requis.");
            }
            OnPropertyChanged(nameof(ErrorMessages));
        }

    }
}
