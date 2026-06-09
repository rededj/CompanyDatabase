using System.ComponentModel.DataAnnotations.Schema;

namespace BDcource.Models
{
    [Table("ProductsOperations")]
    public partial class ProductsOperation
    {
        [Column("ProductID")]
        public int ProductId { get; set; }

        [Column("OperationID")]
        public int OperationId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Operation Operation { get; set; } = null!;
    }
}