using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using Dapper;
using WaliAmanats.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace WaliAmanats.Controllers
{
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        private ApplicationDbContext _context;
        private SqlConnection _con;

        public UserManagementController()
        {
            _context = new ApplicationDbContext();
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
        }
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            var result = _con.Query<UserVM>("select ur.UserId, u.Nama as Nama, u.NPP as Npp, r.Name as Role from AspNetUserRoles ur join AspNetUsers u on UserId = u.Id join AspNetRoles r on RoleId = r.Id");
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(string Id)
        {
            //var result = _context.Users.SingleOrDefault(x => x.Id == Id);
            var result = _con.Query<UserVM>("select ur.UserId, u.Nama as Nama, u.NPP as Npp, r.Id as Role from AspNetUserRoles ur join AspNetUsers u on UserId = u.Id join AspNetRoles r on RoleId = r.Id where UserId = @Id",new {Id = Id});
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(UserVM user, string UserId, string RoleId)
        {
            var db = _context.Users.Single(p => p.Id == UserId);
            db.Nama = user.Nama;
            db.NPP = user.Npp;
            db.UserName = db.NPP;
            _context.SaveChanges();
            var result = _con.Query<UserEditVM>("update AspNetUserRoles Set RoleId=@RoleId where UserId=@UserId", new { UserId= UserId, RoleId = RoleId });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PasswordReset(string Id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == Id);
            var hash = new Microsoft.AspNet.Identity.PasswordHasher();
            string password = "BNI" + user.NPP;
            string passwordHash = hash.HashPassword(password);
            user.PasswordHash = passwordHash;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string Id)
        {
            var user = _context.Users.Where(u => u.Id == Id).SingleOrDefault();
            _context.Users.Remove(user);
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRole()
        {
            var result = _con.Query<IdentityRole>("select * from AspNetRoles").ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}