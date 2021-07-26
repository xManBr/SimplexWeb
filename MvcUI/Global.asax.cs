using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Mercoplano.Simplex.Server.MvcUI.Models;
using Mercoplano.Simplex.Server.MvcUI.Controllers;
using Mercoplano.Simplex.Server.MvcUI.Business;
using Mercoplano.Simplex.Server.MvcUI.Entity;

using System.Globalization;

namespace Mercoplano.Simplex.Server.MvcUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Business.TranslationDate translationDate = new Business.TranslationDate();
            BaseMvc.TranslateLoad();
            translationDate = null;

        }

        protected void Session_Start()
        {
            // Verificando se precisa atualizar a tabela de traduções em memória
            Business.TranslationDate translationDate = new Business.TranslationDate();
            if (BaseMvc.TranslationServerDate < translationDate.Get())
            {
                BaseMvc.TranslateLoad();
            }
            translationDate = null;

            /*
           CultureInfo cultureInfo = ResolveCulture();
           Business.Language language = new Business.Language();
           LanguageSelectResult languageSelectResult = language.List(cultureInfo.TwoLetterISOLanguageName);

           int LanguageId = 0;
           string culture = String.Empty;
           if (languageSelectResult != null)
           {
               LanguageId = languageSelectResult.LanguageId;
           }
           else
           {
               LanguageId = Config.LANGUAGEID;
           }

           BaseMvc baseMvc = new BaseMvc();
           baseMvc.SetLanguageId(LanguageId.ToString());

           baseMvc = null;
           cultureInfo = null;
           language = null;
           languageSelectResult = null;
            * 
            * */

        }

        public static CultureInfo ResolveCulture()
        {
            string[] languages = HttpContext.Current.Request.UserLanguages;

            if (languages == null || languages.Length == 0)
                return null;

            try
            {
                string language = languages[0].ToLowerInvariant().Trim();
                return CultureInfo.CreateSpecificCulture(language);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }


    }
}