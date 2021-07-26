using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mercoplano.Simplex.Server.MvcUI.Entity;
using Mercoplano.Simplex.Server.MvcUI.Models;
using System.Globalization;

using System.Web.Routing;


namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{
    public class BaseMvc : Controller
    {
        public BaseMvc()
        {
            ViewBag.LanguageId = this.ViewLanguageId;
        }

        public Int16 ViewLanguageId
        {
            get
            {
                var languageId = this.GetLanguageId();
                if (languageId == 0)
                {// na primeira entrada pode ser que ainda não se saiba quem é o browser....
                    string[] languages = Request != null ? Request.UserLanguages : new string[] { };
                    languageId = BaseMvc.GetLanguageCultureId(languages);
                }

                return languageId;
            }
        }
        #region Translate

        public static void TranslateLoad()
        {
            Business.Translation translation = new Business.Translation();
            //BaseMvc.TranslationModels = translation.PreperToView();
            BaseMvc.TranslationSelectResult = translation.PreperToView();
            BaseMvc.TranslationServerDate = DateTime.Now;
            translation = null;
        }


        public static CultureInfo ResolveCulture(string[] languages)
        {
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

        public static Int16 GetLanguageCultureId(string[] languages)
        {
            Int16 languageId = Config.LANGUAGEID;
            CultureInfo cultureInfo = ResolveCulture(languages);
            if (cultureInfo != null)
            {
                Business.Language language = new Business.Language();
                LanguageSelectResult languageSelectResult = language.List(cultureInfo.TwoLetterISOLanguageName);

                if (languageSelectResult != null)
                {
                    languageId = languageSelectResult.LanguageId;
                }
            }
            return languageId;
        }

        public void SetLanguageId(string languageId)
        {
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get("simplexplay");
            bool newCookie = false;
            if (cookie == null)
            {
                cookie = new HttpCookie("simplexplay");
                newCookie = true;
            }
            cookie.Values["LanguageId"] = languageId.ToString();
            cookie.Expires = DateTime.Now.AddDays(2);

            if (newCookie)
            {
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie); //This is used for Add cookies.
            }
            else
            {
                System.Web.HttpContext.Current.Response.Cookies.Set(cookie);
                //Response.SetCookie(cookie); //SetCookie is used for update the cookies.
            }
        }

        private Int16 GetLanguageId()
        {
            string value = "0";
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get("simplexplay");
            if (cookie != null)
            {
                value = cookie.Values["languageId"];
            }
            return value != null ? Convert.ToInt16(value) : Convert.ToInt16(0);
        }


        public static String GetLabel(String labelCode, Int16 languageId)
        {
            Business.Translation translation = new Business.Translation();
            String label;
            try
            {// Aterado para não precisar 
                var result = TranslationSelectResult.Find(x => ((x.LabelCode.ToLower() == labelCode.ToLower().Replace(" ","").Trim()) && (x.LanguageId == languageId)));
                label = result != null ? result.LabelName : "[" + labelCode + "]";
            }
            catch
            {
                label = "[" + labelCode + "]";
                translation.InsertAjust(labelCode);
            }
            return label;
        }

        public static DateTime TranslationServerDate { get; set; }

        public static List<TranslationSelectResult> TranslationSelectResult { get; set; }

        #endregion

        #region Legado

        public static Boolean UserLoged
        {
            get
            {
                return BaseMvc.UserProfileId != 0 ? true : false;
            }
        }

        public static void Logoff()
        {
            System.Web.HttpContext.Current.Session.Remove("UserAgent");
        }

        #region Global Setup
        public static String DecimalSeparetor
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            }
        }
        #endregion
        #region Identitification User Profile

        public static Int16 UserDefaultCountry
        {
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.UserDefaultCountryId != 0))
                {
                    return model.UserDefaultCountryId;
                }
                else
                {
                    return Config.USERDEFAULTCOUNTRYID;
                }
            }
        }
        public static Int32 UserProfileId
        {
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.UserAgentId != 0))
                {
                    return model.UserAgentId;
                }
                else
                {
                    return 0;
                }
            }
        }
        public static String UserProfileLogin
        {
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.Code != null))
                {
                    return model.Code;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public static String UserProfileName
        {
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.Name != null))
                {
                    return model.Name;
                }
                else
                {
                    return String.Empty;
                }
            }

        }
        public static String DatePickerFormat
        {//"dd/mm/yy"
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.DatePickerFormat != null))
                {
                    return model.DatePickerFormat;
                }
                else
                {
                    return Config.DATEPICKERFORMAT;
                }
            }
        }
        public static String DateFormat
        { //"dd/MM/yyyy"
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.DateFormat != null))
                {
                    return model.DateFormat;
                }
                else
                {
                    return Config.DATEFORMAT;
                }
            }
        }
        public static String UserDecimalSeparetor
        {
            get
            {
                var model = (UserAgentModel)System.Web.HttpContext.Current.Session["UserAgent"];
                if ((model != null) && (model.UserDecimalSeparetor != null))
                {
                    return model.UserDecimalSeparetor;
                }
                else
                {
                    return BaseMvc.DecimalSeparetor;
                }
            }
        }

        #endregion


        #endregion


    }
}