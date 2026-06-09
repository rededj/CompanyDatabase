using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDcource.Models
{
    public partial class User
    {
        [Column("UserID")]
        public int UserId { get; set; }
        public string Login { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string? WorkshopName { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<MaterialIssuance> MaterialIssuances { get; set; } = new List<MaterialIssuance>();
        public virtual ICollection<ToolIssuance> ToolIssuances { get; set; } = new List<ToolIssuance>();
    }
}