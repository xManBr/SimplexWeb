using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mercoplano.Simplex.Server.MvcUI.Models;
using Mercoplano.Simplex.Server.MvcUI.Entity;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class Language
    {
        private SimplexEntities db = new SimplexEntities();

        public LanguageSelectResult[] List(Int16 languageId)
        {
            /* Eventuais telas de lista de idiomas que forem copias de outro sistema devems er adaptadas para este novo formato de lista de retorno*/
            LanguageSelectResult[] languageSelectResult = db.LanguageSelect(languageId).ToArray();
            return languageSelectResult;           
        }

        public LanguageSelectResult List(String languageCode)
        {
            LanguageSelectResult[] languageSelectResult = db.LanguageSelect(0).ToArray();
            LanguageSelectResult thisLanguage = languageSelectResult.FirstOrDefault(x => x.LanguageCode.Substring(0, 2).ToLower() == languageCode.Substring(0, 2).ToLower());

            return thisLanguage;
        }
    }
}