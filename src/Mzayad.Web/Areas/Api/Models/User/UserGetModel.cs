﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Web.Areas.Api.Models.User
{
    public class UserGetModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }


    }
}
