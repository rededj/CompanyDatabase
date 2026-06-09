using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDcource.Models
{
    public partial class ToolIssuance
    {
        [Column("IssuanceID")]
        public int IssuanceId { get; set; }

        [Column("WorkOrderID")]
        public int WorkOrderId { get; set; }

        [Column("OperationID")]
        public int OperationId { get; set; }

        [Column("SerialNumber")]
        public string SerialNumber { get; set; } = null!;

        [Column("WorkshopID")]
        public int WorkshopId { get; set; }

        [Column("UserID")]
        public int UserId { get; set; }

        [Column("IssueDateTime")]
        public DateOnly IssueDateTime { get; set; }

        [Column("ReturnDateTime")]
        public DateOnly ReturnDateTime { get; set; }

        [Column("ActualReturnDate")]
        public DateOnly? ActualReturnDate { get; set; }

        public virtual WorkOrder WorkOrder { get; set; } = null!;
        public virtual Operation Operation { get; set; } = null!;
        public virtual Tool SerialNumberNavigation { get; set; } = null!;
        public virtual Workshop Workshop { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}