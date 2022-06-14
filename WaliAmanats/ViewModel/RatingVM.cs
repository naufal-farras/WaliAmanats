using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
    public class RatingVM
    {
        public TransaksiRating TransaksiRating { get; set; }
    }
    public class ViewVM
    {
        public TransaksiKeuangan TransaksiKeuangan { get; set; }
        public List<DetailKeuangan> DetailKeuangan { get; set; }
    }
    public class JaminanVM
    {
        public TransaksiJaminan TransaksiJaminan { get; set; }
    }
}