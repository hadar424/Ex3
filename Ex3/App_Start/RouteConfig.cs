using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ex3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("display", "display/{ip}/{port}/{time}",
                defaults: new { controller = "First", action = "display" });

            routes.MapRoute(
                name: "Default",
                url: "Display/{ip}/{port}",
                defaults: new { controller = "First", action = "Index" }
            );
        }
    }
}
