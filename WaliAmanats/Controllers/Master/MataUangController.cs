using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers.Master
{
    public class MataUangController : Controller
    {
        // GET: MataUang
        ApplicationDbContext _context;
        public MataUangController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            var result = _context.MataUang.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(Int64 Id)
        {
            var result = _context.MataUang.SingleOrDefault(m => m.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(MataUang mataUang)
        {
            if (mataUang.Id == 0)
            {
                _context.MataUang.Add(mataUang);
            }
            else
            {
                var mataUangDb = _context.MataUang.Single(m => m.Id == mataUang.Id);
                mataUangDb.Nama = mataUang.Nama;
                mataUangDb.Initial = mataUang.Initial;
                mataUangDb.Satuan = mataUang.Satuan;
            }
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.MataUang.Where(m => m.Id == Id).FirstOrDefault();
            if (delete != null)
            {
                _context.MataUang.Remove(delete);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}