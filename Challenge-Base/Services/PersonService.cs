using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Challenge_Base.Data.Repositories.Interfaces;
using Challenge_Base.Models;
using Challenge_Base.Services.Interfaces;

namespace Challenge_Base.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public void Add(Person newPerson)
        {
            ValidatePerson(newPerson);
            _personRepository.Save(newPerson);
        }

        public IEnumerable<Person> FindAll()
        {
            return _personRepository.GetAll();
        }

        public void Remove(Person person)
        {
            _personRepository.Delete(person);
        }

        public int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private void ValidatePerson(Person person)
        {
            var nameRegex = new Regex("^[a-zA-Z]{2,}$");
            if (!nameRegex.IsMatch(person.Firstname))
            {
                throw new ArgumentException("Le prénom doit contenir au moins 2 caractères alphabétiques.");
            }
            if (!nameRegex.IsMatch(person.Lastname))
            {
                throw new ArgumentException("Le nom doit contenir au moins 2 caractères alphabétiques.");
            }
        }
    }
}
