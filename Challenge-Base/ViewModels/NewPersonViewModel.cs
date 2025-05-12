using System;
using System.Collections.Generic;
using System.Windows.Input;
using Challenge_Base.Models;
using Challenge_Base.Services.Interfaces;
using Challenge_Base.Utils;
using Challenge_Base.Utils.Commands;

namespace Challenge_Base.ViewModels
{
    public class NewPersonViewModel : BaseViewModel
    {
        private readonly IPersonService _personService;
        private readonly INavigationService _navigationService;

        private readonly Dictionary<string, Action<object, string>> _propertyValidators;

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                    ValidateProperty(nameof(FirstName), value);
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                    ValidateProperty(nameof(LastName), value);
                }
            }
        }

        private DateTime? _dateOfBirth;
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (_dateOfBirth != value)
                {
                    _dateOfBirth = value;
                    OnPropertyChanged(nameof(DateOfBirth));
                    ValidateProperty(nameof(DateOfBirth), value);
                }
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

        public NewPersonViewModel(IPersonService personService, INavigationService navigationService)
        {
            _personService = personService;
            _navigationService = navigationService;
            SaveCommand = new RelayCommand(Save, CanSave);

            _propertyValidators = new Dictionary<string, Action<object, string>>
            {
                { nameof(FirstName), (value, propertyName) => ValidateFirstName(value as string, propertyName) },
                { nameof(LastName), (value, propertyName) => ValidateLastName(value as string, propertyName) },
                { nameof(DateOfBirth), (value, propertyName) => ValidateDateOfBirth(value as DateTime?, propertyName) },
                { nameof(Street), (value, propertyName) => ValidateStreet(value as string, propertyName) },
                { nameof(City), (value, propertyName) => ValidateCity(value as string, propertyName) },
                { nameof(PostalCode), (value, propertyName) => ValidatePostalCode(value as string, propertyName) }
            };
        }

        public ICommand SaveCommand { get; set; }

        private void Save()
        {
            if (!CanSave())
            {
                return;
            }

            var person = new Person
            {
                Firstname = FirstName,
                Lastname = LastName,
                DateOfBirth = DateOfBirth ?? DateTime.MinValue,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        Street = Street,
                        City = City,
                        PostalCode = PostalCode
                    }
                }
            };

            _personService.Add(person);
            _navigationService.NavigateTo<PersonsViewModel>();
        }

        private bool CanSave()
        {
            return !HasErrors && AreRequiredFieldsFilled();
        }

        private bool AreRequiredFieldsFilled()
        {
            return FirstName.NotEmpty() && 
                LastName.NotEmpty() && 
                DateOfBirth.HasValue && 
                Street.NotEmpty() 
                && City.NotEmpty() && 
                PostalCode.NotEmpty();
        }

        private void ValidateProperty(string propertyName, object value)
        {
            ClearErrors(propertyName);
            if (_propertyValidators.TryGetValue(propertyName, out var validator))
            {
                validator(value, propertyName);
            }
            else
            {
                AddUnknownError(propertyName);
            }
            OnPropertyChanged(nameof(ErrorMessages));
        }

        private void ValidateFirstName(string firstName, string propertyName)
        {
            if (firstName.Empty())
            {
                AddError(propertyName, "Le prénom est requis.");
            }
            else if (firstName.Length < 2)
            {
                AddError(propertyName, "Le prénom doit contenir au moins 2 caractères.");
            }
        }

        private void ValidateLastName(string lastName, string propertyName)
        {
            if (lastName.Empty())
            {
                AddError(propertyName, "Le nom est requis.");
            }
            else if (lastName.Length < 2)
            {
                AddError(propertyName, "Le nom doit contenir au moins 2 caractères.");
            }
        }

        private void ValidateDateOfBirth(DateTime? dateOfBirth, string propertyName)
        {
            if (dateOfBirth == DateTime.MinValue)
            {
                AddError(propertyName, "La date de naissance est requise.");
            }
        }

        private void ValidateStreet(string street, string propertyName)
        {
            if (street.Empty())
            {
                AddError(propertyName, "La rue est requise.");
            }
        }

        private void ValidateCity(string city, string propertyName)
        {
            if (city.Empty())
            {
                AddError(propertyName, "La ville est requise.");
            }
        }

        private void ValidatePostalCode(string postalCode, string propertyName)
        {
            if (postalCode.Empty())
            {
                AddError(propertyName, "Le code postal est requis.");
            }
        }

        private void AddUnknownError(string propertyName)
        {
            AddError(propertyName, "Une erreur inconnue est survenue.");
        }
    }
}



