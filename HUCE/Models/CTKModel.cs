using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HUCE.Models
{
    public class CTKModel
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
        public string MaHK { get; set; }
        public string TenHK { get; set; }
        public List<string> ListMaMH { get; set; }
    }
}