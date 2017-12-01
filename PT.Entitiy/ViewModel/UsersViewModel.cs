using System;

namespace PT.Entitiy.ViewModel
{
    public class UsersViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public DateTime RegisterDate { get; set; }
        public int MyProperty { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string Country { get; set; }
        public int MyProperty2 { get; set; }
        public string cakisma { get; set; }
        public int ordulu { get; set; }
        public string BulentOzen { get; set; }
        public string CreateNew { get; set; }
    }
}
