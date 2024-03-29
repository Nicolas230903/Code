﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ACHE.WebClientes
{
    public class RouteConfig
    {
        //public static void RegisterRoutes(RouteCollection routes)
        //{
        //    routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //    routes.MapRoute(
        //        "Login",
        //        "Login",
        //        new { controller = "Account", action = "Index" }
        //    );

        //    //routes.MapRoute(
        //    //    "Detalle",
        //    //    "MisAutorizaciones/Detalle/{id}",
        //    //    new { controller = "Home", action = "Detalle", id = UrlParameter.Optional }
        //    //);

        //    //routes.MapRoute(
        //    //    "Default",
        //    //    "{controller}/{action}/{id}",
        //    //    new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        //    //);
        //}

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Index",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Login",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
