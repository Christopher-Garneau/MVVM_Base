using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Challenge_Base.Data.Repositories.Interfaces;
using Challenge_Base.Models;
using Challenge_Base.Services.Interfaces;

namespace Challenge_Base.Services
{
    public class AddressService : IAddressService
    {
        private readonly IPersonRepository _personRepository;

        public AddressService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public void Add(Person person, Address newAddress)
        {
            person.Addresses.Add(newAddress);
            _personRepository.Update(person);
        }
    }
}
