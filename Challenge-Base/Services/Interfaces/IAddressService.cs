using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Challenge_Base.Models;

namespace Challenge_Base.Services.Interfaces
{
    public interface IAddressService
    {
        public void Add(Person person, Address newAddress);
    }
}
