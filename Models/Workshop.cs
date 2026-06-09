using System.Collections.Generic;

namespace BDcource.Models
{
    public partial class Workshop
    {
        public int WorkshopId { get; set; }
        public string WorkshopName { get; set; } = null!;
        public string Adress { get; set; } = null!;

        public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();
        public virtual ICollection<ToolIssuance> ToolIssuances { get; set; } = new List<ToolIssuance>();
    }
}