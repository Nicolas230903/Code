﻿using System.Web;
using System.Web.Mvc;

namespace ACHE.WebClientes
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new BasicAuthAttribute());
            //filters.Add(new LogonAuthorize());
            filters.Add(new LoggedOrAuthorizedAttribute());
        }
    }
}
