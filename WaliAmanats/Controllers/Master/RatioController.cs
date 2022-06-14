using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers.Master
{
    public class RatioController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Ratio
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(Ratio ratio)
        {
            Ratio rat = new Ratio();
            rat.Nama = ratio.Nama;
 
            rat.Initial = ratio.Initial;
            rat.IsDelete = false;
            _context.Ratio.Add(rat);
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.Ratio.SingleOrDefault(x => x.Id == Id);
            _context.Ratio.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Edit(Ratio edit)
        {
            var ratio = _context.Ratio.Where(x => x.Id == edit.Id).FirstOrDefault();
            ratio.Id = edit.Id;
            ratio.Nama = edit.Nama;
            ratio.Initial = edit.Initial;
          
            _context.Entry(ratio).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetData()
        {
            var result = _context.Ratio.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(int id)
        {
            var result = _context.Ratio.Where(x => x.Id == id && x.IsDelete == false).FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}