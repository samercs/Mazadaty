using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazadaty.Web.Areas.admin.Models.Avatar
{
    public class AvatarAddViewModel
    {
        public Mazadaty.Models.Avatar Avatar { get; set; }

        public AvatarAddViewModel Init()
        {
            Avatar=new Mazadaty.Models.Avatar();
            return this;
        }
    }
}
