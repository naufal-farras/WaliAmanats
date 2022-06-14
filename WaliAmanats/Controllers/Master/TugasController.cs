using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers.Master
{
    public class TugasController : Controller
    {
        // GET: Tugas
        private ApplicationDbContext _context;

        public TugasController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            var result = _context.JenisTugas.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(Int64 Id)
        {
            var result = _context.JenisTugas.SingleOrDefault(t => t.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(JenisTugas tugas)
        {
            if(tugas.Id == 0)
            {
                _context.JenisTugas.Add(tugas);
            }
            else
            {
                var tugasDb = _context.JenisTugas.Single(t => t.Id == tugas.Id);
                tugasDb.Nama = tugas.Nama;
            }
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.JenisTugas.Where(t => t.Id == Id).FirstOrDefault();
            if (delete != null)
            {
                _context.JenisTugas.Remove(delete);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}