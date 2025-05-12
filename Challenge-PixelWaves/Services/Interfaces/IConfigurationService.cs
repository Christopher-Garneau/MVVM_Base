using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge_PixelWaves.Services.Interfaces
{
    public interface IConfigurationService
    {
        string GetDbPath();
        string GetLogConfigPath();
        string GetLogFilePath();
        string GetDefaultAdminUserName();

        string GetDefaultAdminPassword();
    }
}

