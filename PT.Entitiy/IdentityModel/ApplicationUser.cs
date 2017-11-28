using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PT.Entitiy.Model;

namespace PT.Entitiy.IdentityModel
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(25)]
        public string Name { get; set; }
        [StringLength(35)]
        public string Surname { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public decimal Salary { get; set; }
        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public virtual List<LaborLog> LaborLogs { get; set; } = new List<LaborLog>();
        public virtual List<SalaryLog> SalaryLogs { get; set; } = new List<SalaryLog>();
    }
}
