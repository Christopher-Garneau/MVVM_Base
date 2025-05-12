using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge_Base.Services.Interfaces
{
    public interface INavigationParameterReceiver
    {
        void ApplyNavigationParameters(params object[] parameters);
    }
}
