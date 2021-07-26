using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mercoplano.Simplex.Server.MvcUI.Entity;


namespace Mercoplano.Simplex.Server.MvcUI.Views
{
    public class InteresseController : Controller
    {


        private SimplexEntities db = new SimplexEntities();


        // GET: Interesse
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(String interesse1)
        {
          return View();
        }
        
    }
}