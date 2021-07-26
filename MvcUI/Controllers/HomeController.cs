using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Mercoplano.Simplex.Server.MvcUI.Entity;

namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{
    public class HomeController : BaseMvc
    {

        private SimplexEntities db = new SimplexEntities();

        public ActionResult Index()
        {

            string url = this.HttpContext.Request.Url.ToString();

            string urlAnetrior = String.Empty;

            if (this.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority).Contains("receitafederal.com.br"))
            {
                return RedirectToAction("Cpf_Serasa_Spc_Receita_Federal_Leilao", "RFB");
            }
            else if (this.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority).Contains("melhorpreco.com"))
            {
                db.InteresseInsert("#entrada", url, urlAnetrior);
                return RedirectToAction("Play", "BestPriceHostel");
            }
            else if (this.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority).Contains("maisbarato.com"))
            {
                db.InteresseInsert("#entrada", url, urlAnetrior);
                return RedirectToAction("Play", "BestPriceHostel");
            }
            else if (this.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority).Contains("preco.com.br"))
            {
                db.InteresseInsert("#entrada", url, urlAnetrior);
                return RedirectToAction("Play", "BestPriceHostel");
            }
            else
            {
                db.InteresseInsert("#entrada", url, urlAnetrior);
                return View();
            }
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}