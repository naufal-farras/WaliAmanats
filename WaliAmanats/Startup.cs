using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using WaliAmanats.Models;

[assembly: OwinStartupAttribute(typeof(WaliAmanats.Startup))]
namespace WaliAmanats
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRole();
        }
        private void CreateRole()
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

            //Creating role
                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    role.Name = "Admin";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Asisten"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Asisten";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Pimkel"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Pimkel";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Analis"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Analis";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Pengelola"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Pengelola";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Akuntansi"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Akuntansi";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Penyelia"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Penyelia";
                roleManager.Create(role);
            }
        }
    }
}
