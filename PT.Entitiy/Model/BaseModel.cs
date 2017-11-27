using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PT.Entitiy.Model
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;
        [Column(TypeName = "smalldatetime")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
