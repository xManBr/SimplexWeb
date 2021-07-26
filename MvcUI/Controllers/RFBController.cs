using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{
    public class RFBController : BaseMvc
    {
        //
        // GET: /RFB/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cpf_Serasa_Spc_Receita_Federal_Leilao()
        {
            return View();
        }

        public ActionResult Cpf_Cnpj_IRPF_IRPJ_Comex_Receita_Federal_Brasil()
        {
            return View();
        }
        public ActionResult News()
        {
            return View();
        }
	}
}