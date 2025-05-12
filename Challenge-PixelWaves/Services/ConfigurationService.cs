using Challenge_PixelWaves.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge_PixelWaves.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetDbPath()
        {
            return ConfigurationManager.AppSettings["DbPath"];
        }
        public string GetLogConfigPath()
        {
            return ConfigurationManager.AppSettings["LogConfigPath"];
        }
        public string GetLogFilePath()
        {
            return ConfigurationManager.AppSettings["LogFilePath"];
        }

        public string GetDefaultAdminUserName()
        {
            return ConfigurationManager.AppSettings["DefaultAdmin"];
        }

        public string GetDefaultAdminPassword()
        {
            return ConfigurationManager.AppSettings["DefaultPassword"];
        }
    }
}

