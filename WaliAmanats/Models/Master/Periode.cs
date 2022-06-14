using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    public class Periode : BaseModel
    {
        public int Hari { get; set; }
        public string Penyebut { get; set; }
    }
}