using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Challenge_Base.Data.Repositories.Interfaces;
using Challenge_Base.Models;

namespace Challenge_Base.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Person> GetAll()
        {
            return _context.Persons.Include(p => p.Addresses).ToList();
        }

        public void Save(Person person)
        {
            _context.Persons.Add(person);
            _context.SaveChanges();
        }

        public void Update(Person person)
        {
            var existingPerson = _context.Persons.Include(p => p.Addresses).FirstOrDefault(p => p.Id == person.Id);
            if (existingPerson != null)
            {
                existingPerson.Firstname = person.Firstname;
                existingPerson.Lastname = person.Lastname;

                foreach (var address in person.Addresses)
                {
                    var existingAddress = existingPerson.Addresses.FirstOrDefault(a => a.Id == address.Id);
                    if (existingAddress != null)
                    {
                        existingAddress.Street = address.Street;
                        existingAddress.City = address.City;
                        existingAddress.PostalCode = address.PostalCode;
                    }
                    else
                    {
                        existingPerson.Addresses.Add(address);
                    }
                }

                foreach (var address in existingPerson.Addresses.ToList())
                {
                    if (!person.Addresses.Any(a => a.Id == address.Id))
                    {
                        _context.Addresses.Remove(address);
                    }
                }

                _context.Persons.Update(existingPerson);
                _context.SaveChanges();
            }
        }

        public void Delete(Person person)
        {
            _context.Persons.Remove(person);
            _context.SaveChanges();
        }
    }
}
