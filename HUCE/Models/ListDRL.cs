using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HUCE.Models
{
    public class ListDRL
    {
        public List<DiemRenLuyen> listdrl { get; set; }

        public ListDRL() 
        {
            listdrl = new List<DiemRenLuyen>();
        }
    }
}