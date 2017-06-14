using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Web.Areas.Api.Models.User
{
    public class UserPostModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberCountryCode { get; set; }
        public string CountryCode { get; set; }
        public string Area { get; set; }
        public string Block { get; set; }
        public string Street { get; set; }
        public string Jadda { get; set; }
        public string Building { get; set; }
        

    }
}
