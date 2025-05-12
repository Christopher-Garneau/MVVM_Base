using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Challenge_Base.Models;

namespace Challenge_Base.Data.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        public IList<Person> GetAll();
        public void Save(Person person);
        public void Update(Person person);
        public void Delete(Person person);
    }
}
