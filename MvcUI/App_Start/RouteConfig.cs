using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mercoplano.Simplex.Server.MvcUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LanguageChange",
                url: "Language/ChangeDirect/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


              routes.MapRoute(
                name: "WhatIsIt",
                url: "BestPriceHostel/WhatIsIt/{id}",
                defaults: new { controller = "BestPriceHostel", action = "WhatIsIt", id = UrlParameter.Optional }
            );
            
        }
    }
}
