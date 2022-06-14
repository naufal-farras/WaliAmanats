using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Data.SqlClient;
using WaliAmanats.Models;
using System.Configuration;
using System.Data.Entity;
using System.Threading.Tasks;
using WaliAmanats.Models.Master;
using WaliAmanats.ViewModel;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.Controllers
{
    public class PerusahaanController : Controller
    {
        // GET: Perusahaans
        private SqlConnection _con;
        private ApplicationDbContext _context;
        public PerusahaanController()
        {
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
            _context = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TambahRatio(Int64 Id)
        {
            var result = (from data in _context.Perusahaan.ToList()
                          where data.Id == Id
                          select new RatioVM
                          {
                              IdPerusahaan = data.Id,
                              Setting = _context.SettingRatio.Include(x => x.Perusahaan).Include(x => x.Ratio).Include(x => x.Measurement).Where(x => x.Perusahaan_Id == data.Id).ToList()
        
                          }
                          ).FirstOrDefault();
            return View(result);
        }
        public ActionResult DeleteRatio(Int64 Id)
        {
            bool result = false;
            var b = _context.SettingRatio.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.SettingRatio.Remove(b);
                _context.SaveChanges();
                result = true;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAll()
        {
            var result = _context.Perusahaan.Include(x => x.Rating).ToList();
            //var result = _con.Query<Perusahaan>("Select * From Perusahaan");
            return Json(new { data = result } , JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(Int64 Id)
        {
            var result = _context.Perusahaan.Include(x => x.Rating).SingleOrDefault(i => i.Id == Id);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDetail(Int64 Id)
        {
            var result = _context.SettingRatio.Include(x => x.Perusahaan).Include(x => x.Ratio).Include(x => x.Measurement).Where(x => x.Id == Id).FirstOrDefault();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRating()
        {
            var result = _context.Rating.ToList().OrderByDescending(x =>x.Nilai);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMeasure()
        {
            var result = _context.Measurement.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNilai(Int64 Rating)
        {
            Rating result = new Rating();
            var data = _context.Rating.FirstOrDefault(x => x.Id == Rating);
            if (data != null)
            {
                result = data;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(Perusahaan perusahaan)
        {
            if(perusahaan.Id == 0)
            {
                _context.Perusahaan.Add(perusahaan);
            }   
            else
            {
                var perusahaanDb = _context.Perusahaan.Single(p => p.Id == perusahaan.Id);
                perusahaanDb.Nama = perusahaan.Nama;
                perusahaanDb.Kota = perusahaan.Kota;
                perusahaanDb.Gedung = perusahaan.Gedung;
                perusahaanDb.Jalan = perusahaan.Jalan;
                perusahaanDb.Email = perusahaan.Email;
                perusahaanDb.NoTelp = perusahaan.NoTelp;
                perusahaanDb.PersentaseKredit = perusahaan.PersentaseKredit;
                perusahaanDb.Rating_Id = perusahaan.Rating_Id;
                perusahaanDb.Nilai = perusahaan.Nilai;
            }

            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

      

        public JsonResult AddRatio(SettingRatio ratio)
        {



            if (ratio.Id == 0)
            {
                SettingRatio rat = new SettingRatio();
                rat.Perusahaan_Id = ratio.Perusahaan_Id;
                rat.Ratio_Id = ratio.Ratio_Id;
                rat.Measurement_Id = ratio.Measurement_Id;
                rat.Target = ratio.Target;
                rat.ModifiedDate = ratio.ModifiedDate;

                // rat.TransaksiLaporan_Id = ratio.TransaksiLaporan_Id;

                _context.SettingRatio.Add(rat);
                _context.SaveChanges();
            }
            else
            {
                var rate = _context.SettingRatio.FirstOrDefault(x => x.Id == ratio.Id);
                rate.Id = ratio.Id;
                rate.Perusahaan_Id = ratio.Perusahaan_Id;
                rate.Ratio_Id = ratio.Ratio_Id;
                rate.Measurement_Id = ratio.Measurement_Id;
                rate.Target = ratio.Target;
                rate.ModifiedDate = ratio.ModifiedDate;
                _context.Entry(rate).State = EntityState.Modified;
                _context.SaveChanges();
            }
            var result = _context.SaveChanges();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult InputRatio()
        //{
        //    return View();
        //}
        public ActionResult Delete(Int64 Id)
        {
            bool result = false;
            var b = _context.Perusahaan.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.Perusahaan.Remove(b);
                _context.SaveChanges();
                result = true;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetNotes(Int64 Id)
        //{
        //    var result = _context.DetailPerusahaan.Include(x => x.Ratio).Include(x => x.Measurement).Where(x => x.Perusahaan_Id == Id).ToList();
        //    return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult AddNote(EmitenNotes EN)
        {
            _context.EmitenNotes.Add(EN);
            var affectedRow = _context.SaveChanges();
            return Json(affectedRow, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveNote(Int64 Id)
        {
            bool result = false;
            var b = _context.EmitenNotes.Where(x => x.Id == Id).FirstOrDefault();
            if (b != null)
            {
                _context.EmitenNotes.Remove(b);
                _context.SaveChanges();
                result = true;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
   
       
    }
}