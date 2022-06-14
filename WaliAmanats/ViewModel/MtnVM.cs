using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Master;
using WaliAmanats.Models.Transaksi;

namespace WaliAmanats.ViewModel
{
       public class MtnVM
    {
        public Int64? IdKejadian { get; set; }
        public Int64? IdPerusahaan { get; set; }
        public string NamaPerusahaan { get; set; }
        public string Informasi { get; set; }
        public string Kejadian { get; set; }
        public TransaksiKejadian TransaksiKejadian { get; set; }

    }

}