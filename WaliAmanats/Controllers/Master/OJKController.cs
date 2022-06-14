using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers.Master
{
    public class OJKController : Controller
    {
        // GET: OJK
        ApplicationDbContext _context;

        public OJKController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAll()
        {
            var result = _context.OJK.ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int Id)
        {
            var result = _context.OJK.SingleOrDefault(i => i.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(OJK OJK)
        {
            if (OJK.Id == 0)
            {
                _context.OJK.Add(OJK);
            }
            else
            {
                var OJKDb = _context.OJK.Single(x => x.Id == OJK.Id);
                OJKDb.Alamat1 = OJK.Alamat1;
                OJKDb.Alamat2 = OJK.Alamat2;
                OJKDb.Alamat3 = OJK.Alamat3;
            }
            var addrows = _context.SaveChanges();

            return Json(addrows, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var b = _context.OJK.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.OJK.Remove(b);
                _context.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}