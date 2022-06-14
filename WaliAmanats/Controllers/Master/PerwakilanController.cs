using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Master;
using Dapper;
using WaliAmanats.ViewModel;

namespace WaliAmanats.Controllers
{
    public class PerwakilanController : Controller
    {
        // GET: Perwakillan
        private SqlConnection _con;
        private ApplicationDbContext _context;
        public PerwakilanController()
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
            //var result = await _con.QueryAsync<PerwakilanVM>("Select pp.Id, pp.Nama, p.Nama as Perusahaan From PerwakilanPerusahaan pp join Perusahaan p on Perusahaan_Id = p.Id");
            var result = _context.Perwakilan.Include("Perusahaan").Include("Jabatan").ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetById(int Id)
        {
            var result = _context.Perwakilan.SingleOrDefault(i => i.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Save(PerwakilanPerusahaan perwakilan)
        {
            if (perwakilan.Id == 0)
            {
                _context.Perwakilan.Add(perwakilan);
            }
            else
            {
                var perwakilanDb = _context.Perwakilan.Single(p => p.Id == perwakilan.Id);
                perwakilanDb.Nama = perwakilan.Nama;
                perwakilanDb.Gender = perwakilan.Gender;
                perwakilanDb.Perusahaan_Id = perwakilan.Perusahaan_Id;
                perwakilanDb.Jabatan_Id = perwakilan.Jabatan_Id;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveJabatan(Jabatan jabatan)
        {
            _context.Jabatan.Add(jabatan);
            _context.SaveChanges();
            var result = _context.Jabatan.SingleOrDefault(j => j.Id == jabatan.Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int Id)
        {
            bool result = false;
            var b = _context.Perwakilan.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.Perwakilan.Remove(b);
                _context.SaveChanges();
                result = true;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPerusahaan()
        {
            var result = _context.Perusahaan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJabatan()
        {
            var result = _context.Jabatan.ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}