using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.Models.Transaksi;
using WaliAmanats.Models.Master;
using WaliAmanats.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System.Globalization;
using System.Configuration;
using System.Data.SqlClient; 
using Dapper;

namespace WaliAmanats.Controllers.Transactions
{
    public class TransaksiJaminansController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        private SqlConnection _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());

        // GET: TransaksiJaminans
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CetakJaminan()
        {
            return View();
        }
        public ActionResult ViewJaminan(Int64 Id)
        {
            var res = _context.TransaksiJaminan.Include(x => x.Jaminan)
                .Include(x => x.Perusahaan)
                .Include(x => x.StatusCetak).Where(x => x.Id == Id).FirstOrDefault();
            JaminanVM result = new JaminanVM();
            result.TransaksiJaminan = res;
            return View(result);
        }

        public ActionResult MasterJaminan()
        {
            return View();
        }
        public JsonResult Savemaster(Jaminan jaminan)
        {
            if(jaminan.Id == 0)
            {
                Jaminan rat = new Jaminan();
                rat.Nama = jaminan.Nama;
                rat.IsDelete = false;
                _context.Jaminan.Add(rat);
                _context.SaveChanges();
            }else
            {
                var jamin = _context.Jaminan.Where(x => x.Id == jaminan.Id).FirstOrDefault();
                jamin.Id = jaminan.Id;
                jamin.Nama = jaminan.Nama;
                _context.Entry(jamin).State = EntityState.Modified;
            }
            
          
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
   
        public JsonResult GetDataMaster()
        {
            var result = _context.Jaminan.Where(x => x.IsDelete == false).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetById(int id)
        {
            var result = _context.Jaminan.Where(x => x.Id == id && x.IsDelete == false).FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAll()
        {
            var result = _context.TransaksiJaminan.Include(x => x.Jaminan).Include(x => x.Perusahaan)
              .Include(x => x.StatusCetak).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetPerusahaan(Int64 Id)
        {
            var result = _context.TransaksiJaminan.Include(x => x.Jaminan).Include(x => x.Perusahaan)
            .SingleOrDefault(x => x.Id == Id);

            var JsonResult = Json(result , JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetCetakJaminan()
        {
            var result = _context.TransaksiJaminan.Include(x => x.Jaminan).Include(x => x.Perusahaan)
              .Include(x => x.StatusCetak).Where(x => x.TanggalCetak.Year == DateTime.Now.Year
              && x.TanggalCetak.Month == DateTime.Now.Month && x.TanggalCetak.Day == DateTime.Now.Day).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetCetakJaminanAll()
        {
            var result = _context.TransaksiJaminan.Include(x => x.Jaminan).Include(x => x.Perusahaan)
              .Include(x => x.StatusCetak).ToList();

            var JsonResult = Json(new { data = result }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }
        public JsonResult GetByJaminan(Int64 Id)
        {
            var result = _context.TransaksiJaminan.Include(x => x.Perusahaan).Include(x => x.Jaminan).Include(x => x.StatusCetak)
               .SingleOrDefault(x => x.Id == Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJaminan()
        {
            var result = _context.Jaminan.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru, int interval)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(interval))
                yield return day;
        }
        public JsonResult Edit(TransaksiJaminan jaminan)
        {
            var jaminans = _context.TransaksiJaminan.Where(x => x.Id == jaminan.Id).FirstOrDefault();
            jaminans.Jaminan_Id = jaminan.Jaminan_Id;
            jaminans.TanggalInput = jaminan.TanggalInput;

            var hasil = jaminan.Nominal * (jaminan.Persentase / 100);
           

            jaminans.Nominal = hasil;
            jaminans.Persentase = jaminan.Persentase;


            jaminans.Perusahaan_Id = jaminan.Perusahaan_Id;
            _context.Entry(jaminans).State = EntityState.Modified;
            _context.SaveChanges();
            var result = true;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubmitFile(string Id, HttpPostedFileBase file)
        {
            int idDetail;
            bool Detail = Int32.TryParse(Id, out idDetail);

            bool result = false;
            if (Detail == true)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (file != null)
                {
                    if (file.FileName.EndsWith("pdf") || file.FileName.EndsWith("PDF"))
                    {
                        var detail = _context.TransaksiJaminan.Include(x => x.Perusahaan).Include(x => x.Jaminan)
                            .SingleOrDefault(x => x.Id == idDetail);
                        if (detail.Path != null)
                        {
                            string paths = Server.MapPath("~/Files/Jaminan/" + detail.Path);
                            System.IO.File.Delete(paths);
                        }

                        var ext = Path.GetExtension(file.FileName);

                        var pathfile = detail.Id + "-" + detail.Perusahaan.Nama + "-" + detail.Jaminan.Nama + "" + ext;
                        string path = Server.MapPath("~//Files/Jaminan/" + pathfile);
                        file.SaveAs(path);

                        detail.Path = pathfile;
                        detail.ModifiedDate = DateTime.Now;
                        _context.Entry(detail).State = EntityState.Modified;

                        _context.SaveChanges();
                    }

                    result = true;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(TransaksiJaminan jaminan)
        {
            if (jaminan.Id == 0)
            {
                TransaksiJaminan trans = new TransaksiJaminan();
                trans.Perusahaan_Id = jaminan.Perusahaan_Id;
                trans.Jaminan_Id = jaminan.Jaminan_Id;
                trans.Nominal = jaminan.Nominal;
                trans.Persentase = jaminan.Persentase;
                var hasil = trans.Nominal * (trans.Persentase / 100);
                trans.Nominal = trans.Nominal - hasil;
                trans.TanggalInput = jaminan.TanggalInput;
                

                trans.TanggalCetak = GetMinusNewDate(jaminan.TanggalInput, 15);
                trans.ModifiedDate = DateTime.Now;
                trans.Status = false;
                trans.StatusCetak_Id = 2;
                _context.TransaksiJaminan.Add(trans);
                _context.SaveChanges();
            }
            var result = _context.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Done(Int64 Id)
        {
            bool result = false;
            //foreach (var Ids in Id)
            //{
            var data = _context.TransaksiJaminan.SingleOrDefault(x => x.Id == Id);
            data.Status = true;
            _context.Entry(data).State = EntityState.Modified;
            _context.SaveChanges();
            result = true;
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(Int64 Id)
        {
            bool result = false;
            var delete = _context.TransaksiJaminan.SingleOrDefault(x => x.Id == Id);
            _context.TransaksiJaminan.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMaster(Int64 Id)
        {
            bool result = false;
            var delete = _context.Jaminan.SingleOrDefault(x => x.Id == Id);
            _context.Jaminan.Remove(delete);
            _context.SaveChanges();
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private DateTime GetMinusNewDate(DateTime startdate, int bil)
        {
            var nextFewJobDays = new List<DateTime>();
            var endDate = startdate.Date;

            while (nextFewJobDays.Count() < bil)
            {                
                if (!IsHoliday(endDate) && !IsWeekEnd(endDate))
                {
                    nextFewJobDays.Add(endDate);
                }

                endDate = endDate.AddDays(-1);
            }

            if (bil == 0)
            {
                return startdate;
            }
            else
            {
                return nextFewJobDays.Last();
            }
        }
        private bool IsHoliday(DateTime date)
        {
            var holiday = _context.HariLibur.Select(x => x.TanggalLibur).Contains(date);
            return holiday;
        }

        private bool IsWeekEnd(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
                || date.DayOfWeek == DayOfWeek.Sunday;
        }
        //CETAK LAPORANJAMINAN
        public void CetakLaporanJaminan(string TanggalCetak, Int64 Idc)
        {

            var result = _con.Query<LaporanJaminanVM>(
             "EXEC SP_GetLaporanFidusia @TanggalCetak,@Idc",
              new
              { 
                  TanggalCetak = TanggalCetak,
                  Idc = Idc,
              }).ToList();

            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "Template_LJ"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi file 
            var trsCetak = _context.TransaksiJaminan.Single(x => x.Id == Idc);
            trsCetak.StatusCetak_Id = 5;
            //_context.Entry(Lap).State = EntityState.Modified;
            _context.Entry(trsCetak).State = EntityState.Modified;
            _context.SaveChanges();

            string Gedung = "";
 
            foreach (var item in result)
            {
                if (item.Gedung != null)
                {
                    Gedung = item.Gedung;

                }

                document.Replace("%%Perusahaan%%", item.Nama_Perusahaan, false, true);
                document.Replace("%%Gedung%%", Gedung, false, true);
                document.Replace("%%Jalan%%", item.Jalan, false, true);
                document.Replace("%%Kota%%", item.Kota, false, true);

                document.Replace("%%Gender%%", item.Gender, false, true);
                document.Replace("%%NamaPerwakilan%%", item.NamaPerwakilan, false, true);
                document.Replace("%%Jabatan%%", item.NamaJabatan, false, true);

                document.Replace("%%mtn%%", item.Initial, false, true);

                document.Replace("%%JenisTugas%%", item.NamaAgen, false, true);
            }




            IWTextRange textRange;
            WSection section = document.Sections[0];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            var no = 1;

            foreach (var item in result)
            {
                //Adds row baru
                row = table.AddRow(true, false);
                //Specifies the row height
                row.Height = 18;
                //Specifies the row height type
                row.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(' ' + no.ToString()).CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no++;

                //Column perusahaan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Initial + ' ' + item.NamaProduk);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;

            }
            document.Save("LaporanJaminan.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();

        }
    }
}