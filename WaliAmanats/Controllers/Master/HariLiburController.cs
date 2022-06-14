using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data.SqlClient;
using WaliAmanats.Models;
using System.Configuration;
using System.Threading.Tasks;
using WaliAmanats.Models.Master;

namespace WaliAmanats.Controllers
{
    public class HariLiburController : Controller
    {
        // GET: HariLiburs
        private SqlConnection _con;
        private ApplicationDbContext _context;
        public HariLiburController()
        {
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            //var result = await _con.QueryAsync<HariLibur>("Select * From HariLiburs");
            var result = _context.HariLibur.ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int Id)
        {
            var result = _context.HariLibur.SingleOrDefault(i => i.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(HariLibur harilibur)
        {
            if (harilibur.Id == 0)
            {
                _context.HariLibur.Add(harilibur);
            }
            else
            {
                var hariliburDb = _context.HariLibur.Single(p => p.Id == harilibur.Id);
                hariliburDb.Nama = harilibur.Nama;
                hariliburDb.TanggalLibur = harilibur.TanggalLibur;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int Id)
        {
            bool result = false;
            var b = _context.HariLibur.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.HariLibur.Remove(b);
                _context.SaveChanges();
                result = true;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}