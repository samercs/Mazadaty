using System;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
{
    public class AddressService : ServiceBase
    {
        public AddressService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<Address> SaveAddress(Address address)
        {
            using (var dc = DataContext())
            {
                if (address.AddressId == default(int))
                {
                    var newAddress = new Address
                    {
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        AddressLine3 = address.AddressLine3,
                        AddressLine4 = address.AddressLine4,
                        CityArea = address.CityArea,
                        StateProvince = address.StateProvince,
                        CountryCode = address.CountryCode,
                        PostalCode = address.PostalCode
                    };

                    dc.Addresses.Add(newAddress);

                    await dc.SaveChangesAsync();

                    return newAddress;
                }
                
                throw new NotImplementedException();
            }
        }
    }
}
