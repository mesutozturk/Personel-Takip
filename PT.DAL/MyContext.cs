using Microsoft.AspNet.Identity.EntityFramework;
using PT.Entitiy.IdentityModel;
using PT.Entitiy.Model;
using System.Data.Entity;

namespace PT.DAL
{
    public class MyContext : IdentityDbContext<ApplicationUser>
    {
        public MyContext()
            : base("name=MyCon")
        { }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<LaborLog> LaborLogs { get; set; }
        public virtual DbSet<SalaryLog> SalaryLogs { get; set; }
    }
}
