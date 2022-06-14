using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaliAmanats.ViewModel
{
    public class UserVM
    {
        public string UserId { get; set; }
        public string Nama { get; set; }
        public string Npp { get; set; }
        public string Role { get; set; }
    }
    public class UserEditVM
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}