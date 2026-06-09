using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDcource.Models
{
    public partial class MaterialIssuance
    {
        [Column("IssuanceID")]
        public int IssuanceId { get; set; }

        [Column("WorkOrderID")]
        public int WorkOrderId { get; set; }

        [Column("OperationID")]
        public int OperationId { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [Column("MaterialID")]
        public int MaterialId { get; set; }

        public int ActualQuantity { get; set; }
        public DateTime IssueDateTime { get; set; }

        public virtual WorkOrder WorkOrder { get; set; } = null!;
        public virtual Operation Operation { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}