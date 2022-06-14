using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WaliAmanats.Models;
using WaliAmanats.ViewModel;
using Syncfusion.DocIO.DLS;
using System.Globalization;
using Syncfusion.DocIO;
using System.IO;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;

namespace WaliAmanats.Controllers.Transactions
{
    public class TransaksiOJKController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SqlConnection _con;

        public TransaksiOJKController()
        {
            _context = new ApplicationDbContext();
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["WaliAmanatApps"].ToString());
        }
        // GET: TramsaksiOJK
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetByRangeDate(string StartDate, string EndDate)
        {
            DateTime sDate = Convert.ToDateTime(StartDate);
            DateTime eDate = Convert.ToDateTime(EndDate);

            var result = (from lap in _context.TransaksiLaporan
                          .Include(x => x.Perusahaan)
                          .Include(x => x.Produk)
                          .ToList()
                          select new LaporanOJKVM
                          {
                              Id = lap.Id,
                              Perusahaan = lap.Perusahaan.Nama,
                              Produk = lap.Produk.Nama,
                              NamaProduk = lap.NamaProduk,
                              Details = (from det in _context.TransaksiDetail.ToList()
                                         where det.Trans_Id == lap.Id
                                         select new DetailOJKVM
                                         {
                                             Id = det.Id,
                                             TglTerbit = det.TglTerbit,
                                             TglJatuhTempo = det.TglJatuhTempo
                                         }).ToList()
                          }).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetById(Int64 Id, DateTime StartDate, DateTime EndDate)
        {
            var result = (from tgl in _context.TransaksiTanggal.ToList()
                          where tgl.Detail_Id == Id
                          && (tgl.TglSuratBunga >= StartDate && tgl.TglSuratBunga <= EndDate)
                          && tgl.Status == false
                          select new TanggalBungaVM
                          {
                              Id = tgl.Id,
                              TglSuratBunga = tgl.TglSuratBunga,
                              NoKupon = tgl.NoKupon,
                              NilaiBunga = tgl.NilaiBunga
                          }).ToList();
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public void Cetak()
        {
            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "TemplateDoc3"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Doc); // membaca isi file 

            //var result = _context.TransaksiTanggal
            //                        //.OrderBy(x => x.TglSuratBunga)
            //                        .Include(x => x.TransaksiDetail)
            //                        .Include(x => x.TransaksiDetail.TransaksiLaporan)
            //                        .Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
            //                        .Where(x => x.Status == false)
            //                        .AsEnumerable()
            //                        .GroupBy(x => x.Detail_Id)
            //                        .Select(x => x.FirstOrDefault());

            var result = (from lap in _context.TransaksiLaporan
                         .Include(x => x.Perusahaan)
                         .ToList()
                          select new LaporanOJKVM
                          {
                              Id = lap.Id,
                              Perusahaan = lap.Perusahaan.Nama,
                              NamaProduk = lap.NamaProduk,
                              Details = (from det in _context.TransaksiDetail.ToList()
                                         where det.Trans_Id == lap.Id
                                         select new DetailOJKVM
                                         {
                                             Id = det.Id,
                                             TglTerbit = det.TglTerbit,
                                             TglJatuhTempo = det.TglJatuhTempo
                                         }).ToList()
                          }).ToList();

            WSection section = document.Sections[1];
            WTable table = section.Tables[0] as WTable;
            WTableRow row;
            WTableCell cell;
            foreach (var item in result)
            {
                //Adds the second row into table
                row = table.AddRow(true, false);
                //Adds the first cell into row
                cell = row.AddCell();
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Adds the second cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.Perusahaan);
                //Adds the third cell into row
                cell = row.AddCell();
                cell.AddParagraph().AppendText(item.NamaProduk);
                //Adds the fourth cell into row
                cell = row.AddCell();
                foreach (var item2 in item.Details)
                {
                    cell.AddParagraph().AppendText(item2.TglTerbit.ToString("dd-MM-yyyy"));
                }
                //Adds the fifth cell into row
                cell = row.AddCell();
                foreach (var item2 in item.Details)
                {
                    cell.AddParagraph().AppendText(item2.TglJatuhTempo.ToString("dd-MM-yyyy"));
                }
                //5Adds the sixth cell into row
                cell = row.AddCell();
                //cell.AddParagraph().AppendText(item.Nominal.ToString("N2"));
                cell = row.AddCell();
                //foreach (var item2 in item)
                //{
                //    //Adds the seventh cell into row

                //    cell.AddParagraph().AppendText(item2.NoKupon.ToString());
                //    //Adds the eight cell into row

                //    cell.AddParagraph().AppendText(item2.NilaiBunga.ToString("N2"));
                //}
                //cell = row.AddCell();


            }

            document.Save("Output.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }

        public ActionResult Index_OJK()
        {
            return View();
        }

        public void LaporanOJKAlt(string startDate, string endDate, Int64 Aturan, string User)
        {
            DateTime sDate;
            DateTime eDates;
            DateTime.TryParse(startDate, out sDate);
            DateTime.TryParse(endDate, out eDates);
            var eDate = eDates.AddHours(24).AddMinutes(59).AddSeconds(59);

            var tahun = sDate.Year;
            var bulan = sDate.ToString("MMMM", new System.Globalization.CultureInfo("id-ID"));
            var bulan2 = eDate.ToString("MMMM", new System.Globalization.CultureInfo("id-ID"));
            //backup
            //"select " +
            //    "tt.Detail_Id as Detail_Id," +
            //    "p.Nama as Nama_Perusahaan," +
            //    "p.PersentaseKredit as Persentase," +
            //    "pr.Initial as Initial," +
            //    "tl.NamaProduk as NamaProduk," +
            //    "td.Seri as Seri," +
            //    "td.TglTerbit as TglTerbit," +
            //    "td.TglJatuhTempo as TglTempo," +
            //    "td.Nominal as Nominal," +
            //    "tt.NoKupon as Kupon," +
            //    "tt.NilaiBunga as NilaiBunga " +
            //    "from TransaksiTanggals tt " +
            //    "join TransaksiDetails td on tt.Detail_Id = td.Id " +
            //    "join TransaksiLaporans tl on td.Trans_Id = tl.Id " +
            //    "join Perusahaan p on tl.Perusahaan_Id = p.Id " +
            //    "join Produk pr on tl.Produk_Id = pr.Id " +
            //    "where tt.TglSuratBunga >= @startDate " +
            //    "and tt.TglSuratBunga <= @endDate " +
            //    "and tl.isDelete = 'false' " +
            //    "and tl.Produk_Id != '1' " +
            //    "and tl.Produk_Id != '4' " +
            //    "order by p.Nama",

            var result = _con.Query<OjkVM>(
               "EXEC SP_GetLaporanOjk @startDate, @endDate",
                new
                {
                    startDate = startDate,
                    endDate = endDate
                }).ToList();

            var aturan = _context.AturanOJK.Single(x => x.Id == Aturan);
            var alamatOJK = _context.OJK.FirstOrDefault();


            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "OJK6"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Docx); // membaca isi file 
            if (eDate.Month <= 6)
            {
                document.Replace("%%Tipe2%%", "TENGAH TAHUNAN", false, true);
                document.Replace("%%Tipe1%%", "Tengah Tahunan", false, true);
            }
            else
            {
                document.Replace("%%Tipe2%%", "TAHUNAN", false, true);
                document.Replace("%%Tipe1%%", "Tahunan", false, true);
            }
            document.Replace("%%periode%%", bulan + " s/d " + bulan2 + " " + tahun, false, true);
            document.Replace("%%periode2%%", bulan + " " + tahun + " s/d " + bulan2 + " " + tahun, false, true);
            document.Replace("%%Aturan%%", aturan.Aturan, false, true);
            document.Replace("%%IsiAturan%%", aturan.IsiAturan, false, true);
            document.Replace("%%User%%", User, false, true);
            document.Replace("%%Gedung%%", alamatOJK.Alamat1, false, true);
            document.Replace("%%Jalan%%", alamatOJK.Alamat2, false, true);
            document.Replace("%%Kota%%", alamatOJK.Alamat3, false, true);


            IWTextRange textRange;
            WSection section = document.Sections[1];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            //var no = 1;

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
                cell.AddParagraph().AppendText("").CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //no++;
                //Column perusahaan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Nama_Perusahaan);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column nama produk
                if (item.Seri == "-")
                {
                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(item.Initial + " " + item.NamaProduk);
                    textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    textRange.CharacterFormat.FontName = "Arial";
                    textRange.CharacterFormat.FontSize = 9;
                }
                else
                {
                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(item.Initial + " " + item.NamaProduk + " Seri " + item.Seri);
                    textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    textRange.CharacterFormat.FontName = "Arial";
                    textRange.CharacterFormat.FontSize = 9;
                }
                //Column tgl terbit
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TglTerbit.ToString("dd-MM-yyyy"));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column jatuh tempo
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TglTempo.ToString("dd-MM-yyyy"));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column nilai emisi / total
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText((item.Nominal).ToString("N2", culture));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column persentase Kredit
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Persentase.ToString());
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column nomor kupon
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText("Bunga ke : " + item.Kupon.ToString());
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column nilai bunga yg dibayar
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.NilaiBunga.ToString("N2"));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column konversi
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText("Nihil");
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;

                var last = _context.TransaksiTanggal.Where(x => x.Detail_Id == item.Detail_Id).ToList().LastOrDefault();

                if (last.Status == true)
                {
                    if (last.NoKupon == item.Kupon)
                    {
                        //Adds row baru
                        row = table.AddRow(true, false);
                        //Specifies the row height
                        row.Height = 18;
                        //Specifies the row height type
                        row.HeightType = TableRowHeightType.AtLeast;

                        //Column nomor row
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("");
                        cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //no++;
                        //Column perusahaan
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.Nama_Perusahaan);
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Column nama produk
                        if (item.Seri == "-")
                        {
                            cell = row.AddCell();
                            textRange = cell.AddParagraph().AppendText(item.Initial + " " + item.NamaProduk);
                            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                            textRange.CharacterFormat.FontName = "Arial";
                            textRange.CharacterFormat.FontSize = 9;
                        }
                        else
                        {
                            cell = row.AddCell();
                            textRange = cell.AddParagraph().AppendText(item.Initial + " " + item.NamaProduk + " Seri " + item.Seri);
                            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                            textRange.CharacterFormat.FontName = "Arial";
                            textRange.CharacterFormat.FontSize = 9;
                        }
                        //Column tgl terbit
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.TglTerbit.ToString("dd-MM-yyyy"));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column jatuh tempo
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.TglTempo.ToString("dd-MM-yyyy"));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column nilai emisi / total
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText((item.Nominal).ToString("N2", culture));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column persentase Kredit
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.Persentase.ToString());
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column nomor kupon
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("Pokok : ");
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;

                        //Column nilai bunga yg dibayar
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.Nominal.ToString("N2"));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column konversi
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("Nihil");
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                    }

                }
            }

            //Merge Header Horizontal untuk Pembayaran Bunga atau Kupon
            table[0, 7].CellFormat.HorizontalMerge = CellMerge.Start;
            table[0, 8].CellFormat.HorizontalMerge = CellMerge.Continue;

            int index = 0;
            int no = 1;
            string para1 = "", para2 = "", para3 = "", para4 = "", para5 = "", para6 = "";

            foreach (WTableRow row2 in table.Rows)
            {
                //Skip tabel header
                if (row2.Cells[0].Paragraphs[0].Text == "No.")
                    continue;

                //Detect row pertama
                index++;

                if (index == 1)
                {
                    //set filter kolom Nama Produk
                    para3 = row2.Cells[2].Paragraphs[0].Text;

                    //Inisialisasi Vertical Merge
                    row2.Cells[0].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[1].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[2].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[3].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[4].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[5].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[8].CellFormat.VerticalMerge = CellMerge.Start;

                    //Numbering
                    row2.Cells[0].AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                    no++;
                    continue;
                }

                //Get row value
                var text1 = row2.Cells[0].Paragraphs[0].Text;
                var text2 = row2.Cells[1].Paragraphs[0].Text;
                var text3 = row2.Cells[2].Paragraphs[0].Text;
                var text4 = row2.Cells[3].Paragraphs[0].Text;
                var text5 = row2.Cells[4].Paragraphs[0].Text;
                var text6 = row2.Cells[5].Paragraphs[0].Text;

                //Check bila row (nama produk) saat ini sama dgn row sebelumnya
                if (para3.Equals(text3))
                {
                    //Jika sama lakukan Vertical Merge
                    row2.Cells[0].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[1].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[2].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[3].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[4].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[5].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[8].CellFormat.VerticalMerge = CellMerge.Continue;
                }
                else
                {
                    //Jika beda, Vertical Merge diinisialisasi ulang
                    row2.Cells[0].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[1].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[2].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[3].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[4].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[5].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[8].CellFormat.VerticalMerge = CellMerge.Start;

                    row2.Cells[0].AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                    no++;
                }

                //Set filter row dgn baris baru
                para1 = text1;
                para2 = text2;
                para3 = text3;
                para4 = text4;
                para5 = text5;
                para6 = text6;


            }

            foreach (WTableRow _row in table.Rows)
                _row.RowFormat.IsBreakAcrossPages = false;

            //Set row pertama sebagai header
            WTableRow header = table.Rows[0];
            header.IsHeader = true;
            header.Height = 30;
            header.HeightType = TableRowHeightType.AtLeast;

            var emiten = result.GroupBy(x => x.Nama_Perusahaan).Select(x => x.First()).ToList();

            WSection section_b = document.Sections[2];
            WTable table_b = section_b.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table_b.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row_b;
            WTableCell cell_b;

            int no_b = 1;

            foreach (var item in emiten)
            {
                //Adds row baru
                row_b = table_b.AddRow(true, false);
                //Specifies the row height
                row_b.Height = 20;
                //Specifies the row height type
                row_b.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell_b = row_b.AddCell();
                cell_b.AddParagraph().AppendText(no_b.ToString() + ".").CharacterFormat.FontSize = 9;
                cell_b.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no_b++;
                //Column perusahaan
                cell_b = row_b.AddCell();
                cell_b.AddParagraph().AppendText(item.Nama_Perusahaan).CharacterFormat.FontSize = 9;
                cell_b.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column tgl peristiwa
                cell_b = row_b.AddCell();
                cell_b.AddParagraph().AppendText("Evaluasi atas laporan keuangan perusahaan").CharacterFormat.FontSize = 9;
                cell_b.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }

            WSection section_c = document.Sections[2];
            WTable table_c = section_b.Tables[1] as WTable;
            //Specifies the horizontal alignment of the table
            table_c.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row_c;
            WTableCell cell_c;

            int no_c = 1;

            foreach (var item in emiten)
            {
                //Adds row baru
                row_c = table_c.AddRow(true, false);
                //Specifies the row height
                row_c.Height = 20;
                //Specifies the row height type
                row_c.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText(no_c.ToString() + ".").CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no_c++;
                //Column perusahaan
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText(item.Nama_Perusahaan).CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column tgl peristiwa penting
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText("").CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column jenis peristiwa penting
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText("-").CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }

            document.Save("LaporanOJK.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();



        }

        public void LaporanOJK(string startDate, string endDate, Int64 Aturan, string User)
        {
            DateTime sDate;
            DateTime eDates;
            DateTime.TryParse(startDate, out sDate);
            DateTime.TryParse(endDate, out eDates);
            var eDate = eDates.AddHours(24).AddMinutes(59).AddSeconds(59);

            var tahun = sDate.Year;
            var bulan = sDate.ToString("MMMM", new System.Globalization.CultureInfo("id-ID"));
            var bulan2 = eDate.ToString("MMMM", new System.Globalization.CultureInfo("id-ID"));

            var result = (from tgl in _context.TransaksiTanggal.Include(x => x.TransaksiDetail.TransaksiLaporan.Perusahaan)
                          .Include(x => x.TransaksiDetail.TransaksiLaporan.Produk)
                          join lap in _context.TransaksiDetail on tgl.Detail_Id equals lap.Id
                          where tgl.TglSuratBunga > sDate && tgl.TglSuratBunga < eDate
                          && tgl.Status == true && lap.TransaksiLaporan.isDelete == false
                          && lap.TransaksiLaporan.Produk_Id != 1 && lap.TransaksiLaporan.Produk_Id != 4
                          orderby lap.TransaksiLaporan.Perusahaan.Nama
                          //orderby lap.TransaksiLaporan.NamaProduk ascending
                          select new
                          {
                              Detail_Id = tgl.Detail_Id,
                              Perusahaan = lap.TransaksiLaporan.Perusahaan,
                              Persentase = lap.TransaksiLaporan.Perusahaan.PersentaseKredit,
                              initial = lap.TransaksiLaporan.Produk.Initial,
                              NamaProduk = lap.TransaksiLaporan.NamaProduk,
                              Seri = lap.Seri,
                              TglTerbit = lap.TglTerbit,
                              TglTempo = lap.TglJatuhTempo,
                              NilaiTotal = lap.Nominal,
                              Kupon = tgl.NoKupon,
                              NilaiBunga = tgl.NilaiBunga
                          }).ToList();
            //var getData = _con.Query<OjkVM>("Exec SP_GetLaporanOjk @startDate, @endDate",
            //    new {
            //        startDate = sDate,
            //        endDate = eDate
            //    }).ToList();
            var aturan = _context.AturanOJK.Single(x => x.Id == Aturan);
            var alamatOJK = _context.OJK.FirstOrDefault();


            string _path = Path.Combine(Server.MapPath("~/Content/Template"), "OJK6"); //mengambil path lokasi file
            WordDocument document = new WordDocument(_path, FormatType.Docx); // membaca isi file 
            if (eDate.Month <= 6)
            {
                document.Replace("%%Tipe2%%", "TENGAH TAHUNAN", false, true);
                document.Replace("%%Tipe1%%", "Tengah Tahunan", false, true);
            }
            else
            {
                document.Replace("%%Tipe2%%", "TAHUNAN", false, true);
                document.Replace("%%Tipe1%%", "Tahunan", false, true);
            }
            document.Replace("%%periode%%", bulan + " s/d " + bulan2 + " " + tahun, false, true);
            document.Replace("%%periode2%%", bulan + " " + tahun + " s/d " + bulan2 + " " + tahun, false, true);
            document.Replace("%%Aturan%%", aturan.Aturan, false, true);
            document.Replace("%%IsiAturan%%", aturan.IsiAturan, false, true);
            document.Replace("%%User%%", User, false, true);
            document.Replace("%%Gedung%%", alamatOJK.Alamat1, false, true);
            document.Replace("%%Jalan%%", alamatOJK.Alamat2, false, true);
            document.Replace("%%Kota%%", alamatOJK.Alamat3, false, true);


            IWTextRange textRange;
            WSection section = document.Sections[1];
            WTable table = section.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row;
            WTableCell cell;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");

            //var no = 1;

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
                cell.AddParagraph().AppendText("").CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //no++;
                //Column perusahaan
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Perusahaan.Nama);
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column nama produk
                if (item.Seri == "-")
                {
                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(item.initial + " " + item.NamaProduk);
                    textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    textRange.CharacterFormat.FontName = "Arial";
                    textRange.CharacterFormat.FontSize = 9;
                }
                else
                {
                    cell = row.AddCell();
                    textRange = cell.AddParagraph().AppendText(item.initial + " " + item.NamaProduk + " Seri " + item.Seri);
                    textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    textRange.CharacterFormat.FontName = "Arial";
                    textRange.CharacterFormat.FontSize = 9;
                }
                //Column tgl terbit
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TglTerbit.ToString("dd-MM-yyyy"));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column jatuh tempo
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.TglTempo.ToString("dd-MM-yyyy"));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column nilai emisi / total
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText((item.NilaiTotal).ToString("N2", culture));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column persentase Kredit
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.Persentase.ToString());
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column nomor kupon
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText("Bunga ke : " + item.Kupon.ToString());
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column nilai bunga yg dibayar
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText(item.NilaiBunga.ToString("N2"));
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;
                //Column konversi
                cell = row.AddCell();
                textRange = cell.AddParagraph().AppendText("Nihil");
                textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                textRange.CharacterFormat.FontName = "Arial";
                textRange.CharacterFormat.FontSize = 9;

                var last = _context.TransaksiTanggal.Where(x => x.Detail_Id == item.Detail_Id).ToList().LastOrDefault();

                if (last.Status == true)
                {
                    if (last.NoKupon == item.Kupon)
                    {
                        //Adds row baru
                        row = table.AddRow(true, false);
                        //Specifies the row height
                        row.Height = 18;
                        //Specifies the row height type
                        row.HeightType = TableRowHeightType.AtLeast;

                        //Column nomor row
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("");
                        cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //no++;
                        //Column perusahaan
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.Perusahaan.Nama);
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        cell.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Column nama produk
                        if (item.Seri == "-")
                        {
                            cell = row.AddCell();
                            textRange = cell.AddParagraph().AppendText(item.initial + " " + item.NamaProduk);
                            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                            textRange.CharacterFormat.FontName = "Arial";
                            textRange.CharacterFormat.FontSize = 9;
                        }
                        else
                        {
                            cell = row.AddCell();
                            textRange = cell.AddParagraph().AppendText(item.initial + " " + item.NamaProduk + " Seri " + item.Seri);
                            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                            textRange.CharacterFormat.FontName = "Arial";
                            textRange.CharacterFormat.FontSize = 9;
                        }
                        //Column tgl terbit
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.TglTerbit.ToString("dd-MM-yyyy"));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column jatuh tempo
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.TglTempo.ToString("dd-MM-yyyy"));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column nilai emisi / total
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText((item.NilaiTotal).ToString("N2", culture));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column persentase Kredit
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.Persentase.ToString());
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column nomor kupon
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("Pokok : ");
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;

                        //Column nilai bunga yg dibayar
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText(item.NilaiTotal.ToString("N2"));
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                        //Column konversi
                        cell = row.AddCell();
                        textRange = cell.AddParagraph().AppendText("Nihil");
                        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        textRange.CharacterFormat.FontName = "Arial";
                        textRange.CharacterFormat.FontSize = 9;
                    }

                }

                #region Backup Code
                //var last = item.Pembayaran.Last();

                //foreach (var item2 in item.Pembayaran)
                //{
                //    cell = row.AddCell();
                //    cell.AddParagraph().AppendText("Kupon ke : " + item2.Kupon.ToString()).CharacterFormat.FontSize = 9;

                //    cell = row.AddCell();
                //    cell.AddParagraph().AppendText(item2.NilaiBunga.ToString("N2")).CharacterFormat.FontSize = 9;

                //    cell = row.AddCell();
                //    cell.AddParagraph().AppendText("Nihil").CharacterFormat.FontSize = 9;

                //    if (item.Pembayaran.Count() < 4)
                //    {

                //        row = table.AddRow(true, false);
                //        row.RowFormat.IsBreakAcrossPages = false;
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();

                //        if (item2.Equals(last))
                //        {
                //            cell = row.AddCell();
                //            cell = row.AddCell();
                //            cell = row.AddCell();
                //            cell.AddParagraph().ParagraphFormat.KeepFollow = true;
                //        }
                //    }

                //}

                //if (item.Pembayaran.Count() == 1)
                //{
                //    for (int i = 0; i < 2; i++)
                //    {

                //        row = table.AddRow(true, false);
                //        row.RowFormat.IsBreakAcrossPages = false;
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell = row.AddCell();
                //        cell.AddParagraph().ParagraphFormat.KeepFollow = true;
                //    }
                //}
                #endregion
            }

            //Merge Header Horizontal untuk Pembayaran Bunga atau Kupon
            table[0, 7].CellFormat.HorizontalMerge = CellMerge.Start;
            table[0, 8].CellFormat.HorizontalMerge = CellMerge.Continue;

            int index = 0;
            int no = 1;
            string para1 = "", para2 = "", para3 = "", para4 = "", para5 = "", para6 = "";

            foreach (WTableRow row2 in table.Rows)
            {
                //Skip tabel header
                if (row2.Cells[0].Paragraphs[0].Text == "No.")
                    continue;

                //Detect row pertama
                index++;

                if (index == 1)
                {
                    //set filter kolom Nama Produk
                    para3 = row2.Cells[2].Paragraphs[0].Text;

                    //Inisialisasi Vertical Merge
                    row2.Cells[0].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[1].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[2].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[3].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[4].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[5].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[8].CellFormat.VerticalMerge = CellMerge.Start;

                    //Numbering
                    row2.Cells[0].AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                    no++;
                    continue;
                }

                //Get row value
                var text1 = row2.Cells[0].Paragraphs[0].Text;
                var text2 = row2.Cells[1].Paragraphs[0].Text;
                var text3 = row2.Cells[2].Paragraphs[0].Text;
                var text4 = row2.Cells[3].Paragraphs[0].Text;
                var text5 = row2.Cells[4].Paragraphs[0].Text;
                var text6 = row2.Cells[5].Paragraphs[0].Text;

                //Check bila row (nama produk) saat ini sama dgn row sebelumnya
                if (para3.Equals(text3))
                {
                    //Jika sama lakukan Vertical Merge
                    row2.Cells[0].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[1].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[2].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[3].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[4].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[5].CellFormat.VerticalMerge = CellMerge.Continue;
                    row2.Cells[8].CellFormat.VerticalMerge = CellMerge.Continue;
                }
                else
                {
                    //Jika beda, Vertical Merge diinisialisasi ulang
                    row2.Cells[0].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[1].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[2].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[3].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[4].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[5].CellFormat.VerticalMerge = CellMerge.Start;
                    row2.Cells[8].CellFormat.VerticalMerge = CellMerge.Start;

                    row2.Cells[0].AddParagraph().AppendText(no.ToString()).CharacterFormat.FontSize = 9;
                    no++;
                }

                //Set filter row dgn baris baru
                para1 = text1;
                para2 = text2;
                para3 = text3;
                para4 = text4;
                para5 = text5;
                para6 = text6;


            }

            foreach (WTableRow _row in table.Rows)
                _row.RowFormat.IsBreakAcrossPages = false;

            //Set row pertama sebagai header
            WTableRow header = table.Rows[0];
            header.IsHeader = true;
            header.Height = 30;
            header.HeightType = TableRowHeightType.AtLeast;

            var emiten = result.GroupBy(x => x.Perusahaan).Select(x => x.First()).ToList();

            WSection section_b = document.Sections[2];
            WTable table_b = section_b.Tables[0] as WTable;
            //Specifies the horizontal alignment of the table
            table_b.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row_b;
            WTableCell cell_b;

            int no_b = 1;

            foreach (var item in emiten)
            {
                //Adds row baru
                row_b = table_b.AddRow(true, false);
                //Specifies the row height
                row_b.Height = 20;
                //Specifies the row height type
                row_b.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell_b = row_b.AddCell();
                cell_b.AddParagraph().AppendText(no_b.ToString() + ".").CharacterFormat.FontSize = 9;
                cell_b.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no_b++;
                //Column perusahaan
                cell_b = row_b.AddCell();
                cell_b.AddParagraph().AppendText(item.Perusahaan.Nama).CharacterFormat.FontSize = 9;
                cell_b.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column tgl peristiwa
                cell_b = row_b.AddCell();
                cell_b.AddParagraph().AppendText("Evaluasi atas laporan keuangan perusahaan").CharacterFormat.FontSize = 9;
                cell_b.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }

            WSection section_c = document.Sections[2];
            WTable table_c = section_b.Tables[1] as WTable;
            //Specifies the horizontal alignment of the table
            table_c.TableFormat.HorizontalAlignment = RowAlignment.Center;

            WTableRow row_c;
            WTableCell cell_c;

            int no_c = 1;

            foreach (var item in emiten)
            {
                //Adds row baru
                row_c = table_c.AddRow(true, false);
                //Specifies the row height
                row_c.Height = 20;
                //Specifies the row height type
                row_c.HeightType = TableRowHeightType.AtLeast;

                //Column nomor row
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText(no_c.ToString() + ".").CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                no_c++;
                //Column perusahaan
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText(item.Perusahaan.Nama).CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column tgl peristiwa penting
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText("").CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                //Column jenis peristiwa penting
                cell_c = row_c.AddCell();
                cell_c.AddParagraph().AppendText("-").CharacterFormat.FontSize = 9;
                cell_c.CellFormat.VerticalAlignment = VerticalAlignment.Middle;
            }

            document.Save("LaporanOJK.doc", FormatType.Word2007, HttpContext.ApplicationInstance.Response, HttpContentDisposition.Attachment);
            document.Close();
        }
    }
}