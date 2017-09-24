using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mazadaty.Data;
using Mazadaty.Models;

namespace Mazadaty.Services
{
    public class AddressService : ServiceBase
    {
        public AddressService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
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
                        PostalCode = address.PostalCode,
                        Floor = address.Floor,
                        FlatNumber = address.FlatNumber
                    };

                    dc.Addresses.Add(newAddress);

                    await dc.SaveChangesAsync();

                    return newAddress;
                }

                throw new NotImplementedException();
            }
        }

        public async Task<Address> GetAddress(int? addressId)
        {
            if (!addressId.HasValue)
            {
                return null;
            }

            using (var dc = DataContext())
            {
                return await dc.Addresses.SingleOrDefaultAsync(i => i.AddressId == addressId);
            }
        }

        public async Task<Address> Update(Address address)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(address);
                await dc.SaveChangesAsync();
                return address;
            }
        }
    }
}
