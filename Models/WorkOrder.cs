using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDcource.Models
{
    public partial class WorkOrder
    {
        [Column("WorkOrderID")]
        public int WorkOrderId { get; set; }

        [Column("ProductID")]
        public int ProductId { get; set; }

        public DateOnly RegistrationDate { get; set; }
        public DateOnly DueDate { get; set; }
        public int RequiredQuantity { get; set; }
        public bool Completed { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<MaterialIssuance> MaterialIssuances { get; set; } = new List<MaterialIssuance>();
        public virtual ICollection<ToolIssuance> ToolIssuances { get; set; } = new List<ToolIssuance>();
        public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
        public string DisplayName => $"{Product?.Name} ({RegistrationDate:dd.MM.yyyy}, {RequiredQuantity} шт)";
    }
}