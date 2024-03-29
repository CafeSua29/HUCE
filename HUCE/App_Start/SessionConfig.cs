﻿using HUCE.Controllers;
using HUCE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public static void DeSession()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}