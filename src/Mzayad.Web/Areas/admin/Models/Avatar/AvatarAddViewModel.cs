using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Web.Areas.admin.Models.Avatar
{
    public class AvatarAddViewModel
    {
        public Mzayad.Models.Avatar Avatar { get; set; }

        public AvatarAddViewModel Init()
        {
            Avatar=new Mzayad.Models.Avatar();
            return this;
        }
    }
}
