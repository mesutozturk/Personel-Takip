using PT.Entitiy.IdentityModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PT.Entitiy.Model
{
    [Table("Departments")]
    public class Department : BaseModel
    {
        [Required]
        [StringLength(55, ErrorMessage = "Bu alan Zorunludur", MinimumLength = 5)]
        [Index(IsUnique = true)]
        public string DepartmentName { get; set; }


        public virtual List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
