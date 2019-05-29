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
            routes.MapRoute(
                name: "index",
                url: "display/{ip}/{port}",
                defaults: new { controller = "First", action = "Index" });


            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name:"display",
                url: "display/{ip}/{port}/{time}",
                defaults: new { controller = "First", action = "Display" });

            
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Save",
                url: "save/{ip}/{port}/{time}/{saveTime}/{file}",
                defaults: new { controller = "First", action = "Save" }
            );

            routes.MapRoute(
              name: "Default",
              url: "{action}/{id}",
              defaults: new { controller = "First", action = "Default" , id = UrlParameter.Optional}
          );
        }
    }
}
