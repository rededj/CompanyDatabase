using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDcource.Models
{
    public partial class Tool
    {
        public string SerialNumber { get; set; } = null!;
        public int ToolTypeId { get; set; }
        public DateOnly ArrivalDate { get; set; }
        public int? CurrentWorkOrderId { get; set; }

        public virtual ToolType ToolType { get; set; } = null!;
        public virtual WorkOrder? CurrentWorkOrder { get; set; }
        public virtual ICollection<ToolIssuance> ToolIssuances { get; set; } = new List<ToolIssuance>();
    }
}