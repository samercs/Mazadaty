using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;

namespace Mzayad.Services
{
    public class AvatarService : ServiceBase
    {
        public AvatarService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }
    }
}
