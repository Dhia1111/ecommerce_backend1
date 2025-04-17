using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsLocation
    {

        public static async Task<string?> GetCountryCode(string CountryName) {

            return await ConnectionLayer.clsLocation.GetCountryCode(CountryName);
        }

        public async static Task<List<string>?> GetAllCountries()
        {
            return await ConnectionLayer.clsLocation.GetAllCountries();
        }
    }
}
