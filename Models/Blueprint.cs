using System.Collections.Generic;

namespace BDcource.Models
{
    public partial class Blueprint
    {
        public string BlueprintNumber { get; set; } = null!;
        public string TechnicalRequirements { get; set; } = null!;

        public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();
    }
}