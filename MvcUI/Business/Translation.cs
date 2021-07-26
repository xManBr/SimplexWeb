using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//using DAL = Mercoplano.LogisticsOperation.Server.Itinerary.SQLServer;
using Mercoplano.Simplex.Server.MvcUI.Models;
using Mercoplano.Simplex.Server.MvcUI.Controllers;
using Mercoplano.Simplex.Server.MvcUI.Entity;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class Translation// : Mercoplano.LogisticsOperation.Server.Business.Translation
    {
        private SimplexEntities db = new SimplexEntities();

        public void InsertAjust(String labelName)
        {
            db.TranslationAjust(labelName);
            //DAL.TranslationDAO.InsertAjust(labelName);
        }

        public TranslationSelectResult[] Search()
        {          

            TranslationSelectResult[] translationSelectResult = db.TranslationSelect(0).ToArray();
            //TranslationModel[] translationModels = DAL.TranslationDAO.Search().ToArray();
            return translationSelectResult;
        }

        public List<TranslationSelectResult> PreperToView()
        {
            List<TranslationSelectResult> translationSelectResult = db.TranslationSelect(0).ToList();
            //List<TranslationModel> translationModels =  DAL.TranslationDAO.Search();

            foreach (var list in translationSelectResult)
            {
                String myLabelCode;
                try
                {   //English label
                    myLabelCode = translationSelectResult.Find(x => ((x.TranslationId == list.TranslationId) && (x.LanguageCode == Config.DEFAULT_LABELCODE_LANGUAGE))).LabelName;
                }
                catch
                {
                    myLabelCode = string.Empty;
                }

                list.LabelCode = myLabelCode.Replace(" ", string.Empty).ToLower();
            }

            return translationSelectResult;
        }
    }
}