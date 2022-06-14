using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers.Master
{
    public class ProdukController : Controller
    {
        // GET: Produk
        private ApplicationDbContext _context;
        public ProdukController()
        {
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            var result = _context.Produk.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(Int64 Id)
        {
            var result = _context.Produk.SingleOrDefault(p => p.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(Produk produk)
        {
            if (produk.Id == 0)
            {
                _context.Produk.Add(produk);
            }
            else
            {
                var ProdukDb = _context.Produk.Single(p => p.Id == produk.Id);
                ProdukDb.Nama = produk.Nama;
            }
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.Produk.Where(x => x.Id == Id).FirstOrDefault();
            if (delete != null)
            {
                _context.Produk.Remove(delete);
                _context.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}