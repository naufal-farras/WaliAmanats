using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers.Master
{
    public class AturanOJKController : Controller
    {
        // GET: AturanOJK
        ApplicationDbContext _context;

        public AturanOJKController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAll()
        {
            var result = _context.AturanOJK.ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int Id)
        {
            var result = _context.AturanOJK.SingleOrDefault(i => i.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(AturanOJK AturanOJK)
        {
            if(AturanOJK.Id == 0)
            {
                _context.AturanOJK.Add(AturanOJK);
            }
            else
            {
                var AturanDb = _context.AturanOJK.Single(x => x.Id == AturanOJK.Id);
                AturanDb.Aturan = AturanOJK.Aturan;
                AturanDb.IsiAturan = AturanOJK.IsiAturan;
            }
            var addrows = _context.SaveChanges();

            return Json(addrows, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var b = _context.AturanOJK.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.AturanOJK.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAturan()
        {
            var result = _context.AturanOJK.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUser()
        {
            var result = _context.Users.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}