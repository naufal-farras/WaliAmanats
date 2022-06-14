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
    public class RatingController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        // GET: Rating
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Save(Rating rating)
        {
            Rating rat = new Rating();
            rat.Nama = rating.Nama;
            rat.Nilai = rating.Nilai;
            rat.IsDelete = false;
            _context.Rating.Add(rat);
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Edit(Rating edit)
        {
            var rating = _context.Rating.Where(x => x.Id == edit.Id).FirstOrDefault();
            rating.Id = edit.Id;
            rating.Nama = edit.Nama;
            rating.Nilai = edit.Nilai;
            _context.Entry(rating).State = EntityState.Modified;
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.Rating.SingleOrDefault(x => x.Id == Id);
            _context.Rating.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetData()
        {
            var result = _context.Rating.Where(x => x.IsDelete == false).OrderByDescending(x=>x.Nilai).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(int id)
        {
            var result = _context.Rating.Where(x => x.Id == id && x.IsDelete == false).FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}