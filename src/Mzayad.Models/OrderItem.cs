using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class OrderItem : ModelBase
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public virtual Product Product { get; set; }
    }
}
