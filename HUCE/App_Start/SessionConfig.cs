using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HUCE.App_Start
{
    public class SessionConfig
    {
        public static void SetSession(string tk, string mq)
        {
            HttpContext.Current.Session["username"] = tk;
            HttpContext.Current.Session["maquyen"] = mq;
        }

        public static string GetSession()
        {
            return (string)HttpContext.Current.Session["username"];
        }

        public static string GetQuyen()
        {
            return (string)HttpContext.Current.Session["maquyen"];
        }
    }
}