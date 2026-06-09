using System.Collections.Generic;

namespace BDcource.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Cost { get; set; }
        public byte OperationsRequired { get; set; }

        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public virtual ICollection<ProductsOperation> ProductsOperations { get; set; } = new List<ProductsOperation>();
    }
}