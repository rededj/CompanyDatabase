using System.Collections.Generic;

namespace BDcource.Models
{
    public partial class Material
    {
        public int MaterialId { get; set; }
        public string Name { get; set; } = null!;
        public int NumberOf { get; set; }

        public virtual ICollection<MaterialIssuance> MaterialIssuances { get; set; } = new List<MaterialIssuance>();
        public virtual ICollection<OperationMaterialsUsage> OperationMaterialsUsages { get; set; } = new List<OperationMaterialsUsage>();
    }
}