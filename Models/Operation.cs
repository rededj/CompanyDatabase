using System.Collections.Generic;

namespace BDcource.Models
{
    public partial class Operation
    {
        public int OperationId { get; set; }
        public int WorkshopId { get; set; }
        public string Description { get; set; } = null!;
        public TimeOnly AverageDuration { get; set; }
        public string BlueprintNumber { get; set; } = null!;

        public virtual Blueprint BlueprintNumberNavigation { get; set; } = null!;
        public virtual Workshop Workshop { get; set; } = null!;

        public virtual ICollection<MaterialIssuance> MaterialIssuances { get; set; } = new List<MaterialIssuance>();
        public virtual ICollection<OperationMaterialsUsage> OperationMaterialsUsages { get; set; } = new List<OperationMaterialsUsage>();
        public virtual ICollection<OperationToolsUsage> OperationToolsUsages { get; set; } = new List<OperationToolsUsage>();
        public virtual ICollection<ToolIssuance> ToolIssuances { get; set; } = new List<ToolIssuance>();
        public virtual ICollection<ProductsOperation> ProductsOperations { get; set; } = new List<ProductsOperation>();
    }
}