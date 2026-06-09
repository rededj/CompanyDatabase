namespace BDcource.Models
{
    public partial class OperationMaterialsUsage
    {
        public int OperationId { get; set; }
        public int MaterialId { get; set; }
        public short RequiredQuantity { get; set; }

        public virtual Operation Operation { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}