using System.Collections.Generic;

namespace BDcource.Models
{
    public partial class ToolType
    {
        public int ToolTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int InStock { get; set; }
        public int? Allocated { get; set; }

        public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
        public virtual ICollection<OperationToolsUsage> OperationToolsUsages { get; set; } = new List<OperationToolsUsage>();
    }
}