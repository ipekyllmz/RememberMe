using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using uygulama.Models.Context;

namespace uygulama
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            Database.SetInitializer<DatabaseContext>(new DropCreateDatabaseIfModelChanges<DatabaseContext>());
           
            RouteConfig.RegisterRoutes(RouteTable.Routes);
          

        }


    }
}
