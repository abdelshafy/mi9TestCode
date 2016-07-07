using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AwesomeService
{
    /// <summary>
    /// Web Api Configuration class
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the configuration
        /// Here Cross-origin resource sharing needs to be enabled.
        /// Default routing to the API is also configured here.
        /// </summary>
        /// <param name="config">HttpConfiguration</param>
        public static void Register(HttpConfiguration config)
        {
            //Cross-origin resource sharing 
            config.EnableCors();
            
            //Default Routing
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "service", id = RouteParameter.Optional }
            );
        }
    }
}
