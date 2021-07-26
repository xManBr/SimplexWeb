using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mercoplano.Simplex.Server.MvcUI.Models;
using Mercoplano.Simplex.Server.MvcUI.Business;
using Mercoplano.Simplex.Server.MvcUI.Controllers;
using Mercoplano.Simplex.Server.MvcUI.Entity;


namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{
    public class LanguageController : BaseMvc
    {
        //
        // GET: /Language/

        public ActionResult Index()
        {
            return View();
        }

        public LanguageController()
        {
            PreperView();
        }

        private void PreperView()
        {
            Business.Language language = new Business.Language();
            //LanguageSelectResult[] languageModels = language.List(BaseMvc.LanguageId).ToArray();
            LanguageSelectResult[] languageModels = language.List(0).ToArray();
            ViewBag.Language = languageModels.ToList().Select(model => new { value = model.LanguageId, text = model.LanguageName });
            languageModels = null;
            language = null;
        }

        public ActionResult Change()
        {
            var model = new LanguageModel();
            model.Id = this.ViewLanguageId;// //BaseMvc.GetLanguageId(this.HttpContext.Request.Cookies);
            return View("Change", model);
        }

        [HttpPost]
        public ActionResult Change(Int16 Id)
        {
            this.SetLanguageId(Id.ToString());
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeDirect(String id)
        {
            this.SetLanguageId(id);
            return RedirectToAction("Index", "Home");
        }

    }
}
