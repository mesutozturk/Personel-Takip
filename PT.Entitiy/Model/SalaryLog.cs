using PT.Entitiy.IdentityModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PT.Entitiy.Model
{
    [Table("SalaryLogs")]
    public class SalaryLog:BaseModel
    {
        public decimal Salary { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
