using System;
using System.Collections.Generic;
using Challenge_Base.Models;

namespace Challenge_Base.Services.Interfaces
{
    public interface IPersonService
    {
        void Add(Person newPerson);
        IEnumerable<Person> FindAll();
        void Remove(Person person);
        int CalculateAge(DateTime birthDate);
    }
}
