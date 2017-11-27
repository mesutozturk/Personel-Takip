using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PT.Entitiy.IdentityModel;

namespace PT.Entitiy.Model
{
    [Table("LaborLogs")]
    public class LaborLog : BaseModel
    {
        public DateTime StartShift { get; set; } = DateTime.Now;
        public DateTime? EndShift { get; set; }
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
