using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WaliAmanats.Models.Base;

namespace WaliAmanats.Models.Master
{
    public class SettingRatio : BaseModel
    {
        [ForeignKey("Perusahaan_Id")]
        public Perusahaan Perusahaan { get; set; }
        public Int64 Perusahaan_Id { get; set; }
        [ForeignKey("Ratio_Id")]
        public Ratio Ratio { get; set; }
        public Int64 Ratio_Id { get; set; }
        [ForeignKey("Measurement_Id")]
        public Measurement Measurement { get; set; }
        public Int64 Measurement_Id { get; set; }
        public string Target { get; set; }
    }
}