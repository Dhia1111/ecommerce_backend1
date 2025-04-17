using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLayer
{
   internal static class clsConnectionGenral
    {

        public static string ConnectionString = "";
        private static IConfiguration _configuration;
        static clsConnectionGenral()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();

            _configuration = builder.Build();

            var st = _configuration["ConnectionSetting"];
            if (!string.IsNullOrEmpty(st)) ConnectionString = st;
        }
    }
}
