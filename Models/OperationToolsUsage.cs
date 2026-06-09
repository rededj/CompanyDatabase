namespace BDcource.Models
{
    public partial class OperationToolsUsage
    {
        public int OperationId { get; set; }
        public int ToolTypeId { get; set; }
        public short QuantityInUse { get; set; }

        public virtual Operation Operation { get; set; } = null!;
        public virtual ToolType ToolType { get; set; } = null!;
    }
}