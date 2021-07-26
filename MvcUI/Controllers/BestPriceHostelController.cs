using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Mercoplano.Simplex.Server.MvcUI.Models;
using Mercoplano.Simplex.Server.MvcUI.Business;

using Mercoplano.Simplex.Server.MvcUI.Entity;



using System.Web.Globalization;

namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{
    public class BestPriceHostelController : BaseMvc
    {

        private SimplexEntities db = new SimplexEntities();


        // --> ´Seria TCC -> Pesquisa Operacional para otmizar a logística e produção de Acuçar e álcool ou fontes energéticas renováveis... ?????
        // Ideia -> to Hostel Administrator ==> W´d you like to publish your business in this web site for Marketing?
        // Custos são apresentados de forma resumida, entretanto o operador pode obtar por abrir os custos,
        // informando individualmente e por tipo de hospedagem os custos de água, luz, internet, cafe da amanha, produtos de limpeza e mão de obra 
        // de limpeza e manutenção, bem como custos administrativos. Exemplo de diálogo: Se tiver à mão você pode informar os custos totais e por tipo de hospedágem ou informar-los de forma fragmentados para que o sistema faça a análise mais detalhada, entretanto isso vai fazer com que os relatórios fiquem mais extensos.
        /*
         * Gostou de nosso modelo - compartilhe. Tem sugestões para melhorar a modelagem este problema, acrescente novas restrições {abre para entrada de novas inequações}
         * 
         * */
        // GET: /BestPriceHostel/

        [AllowAnonymous]
        public ActionResult Help()
        {

            return View("Help");
        }
        [AllowAnonymous]
        public ActionResult WhatIsIt(String id)
        {

            return View();
        }

        private string GetFromLabelTos(List<FromLabelTo> fromLabelTos, string name)
        {
            if (fromLabelTos.FirstOrDefault(x => x.ToLabel == name) != null)
            {
                return fromLabelTos.FirstOrDefault(x => x.ToLabel == name).FromLabel;
            }
            else
            {
                return name;
            }
        }

        private SimplexModel ValuesToBack(SimplexModel simplexModel, decimal[,] outputSetps, String[] columnsDescr, String[] linesDescr, bool isFinish, bool hasSolution, List<FromLabelTo> fromLabelTos)
        {
            simplexModel.IsFinish = isFinish;
            simplexModel.HasSolution = hasSolution;

            for (int i = 0; i < columnsDescr.Length; i++)
            {
                var thisColumns = columnsDescr[i];
                ColumnsDescr itemColumn = new Models.ColumnsDescr();
                itemColumn.Label = GetFromLabelTos(fromLabelTos, thisColumns);//Chave a ser exibida
                itemColumn.DisplayName = itemColumn.Label;
                if (i > simplexModel.OriginalDecisionLength)
                {
                    itemColumn.Type = "R";
                }
                else
                {
                    itemColumn.Type = i != 0 ? "D" : "Z";
                }

                simplexModel.ColumnsDescrs.Add(itemColumn);
            }

            foreach (var thisLine in linesDescr.ToList())
            {
                LineDescr itemLine = new LineDescr();
                itemLine.Label = GetFromLabelTos(fromLabelTos, thisLine);
                ColumnsDescr itemColumn = simplexModel.ColumnsDescrs.FirstOrDefault(x => x.Label == itemLine.Label);
                if (itemColumn != null)
                {
                    itemLine.Type = simplexModel.ColumnsDescrs.FirstOrDefault(x => x.Label == itemLine.Label).Type;
                }
                else
                {
                    itemLine.Type = "?";
                }

                simplexModel.LineDescrs.Add(itemLine);
            }

            for (int i = 0; i < outputSetps.GetLength(0); i++)
            {
                Equation e = new Equation();
                if (simplexModel.LineDescrs.Count > 0)
                {
                    e.Line = simplexModel.LineDescrs[i].Label;
                }
                for (int j = 0; j < outputSetps.GetLength(1); j++)
                {
                    Coefficient c = new Coefficient();
                    //c.Number = Math.Truncate(outputSetps[i, j] * 1000000) / 1000000;
                    c.Number = outputSetps[i, j];
                    e.Coefficients.Add(c);
                }
                simplexModel.Equations.Add(e);
            }

            return simplexModel;
        }

        public ActionResult Index()
        {
            var interesses = db.Interesse.OrderByDescending(x => x.Id).Take(50).ToList();
            return View(interesses);
        }


        [HttpPost]
        public ActionResult Interesse(String textSearch)
        {
            string url = this.HttpContext.Request.Url.ToString();

            string urlAnetrior;

            try
            {
                urlAnetrior = this.HttpContext.Request.UrlReferrer.ToString();
            }
            catch (Exception ex)
            {
                urlAnetrior = "indefinifo";
            }

            if (textSearch != String.Empty)
            { 
            db.InteresseInsert(textSearch, url, urlAnetrior);

            return RedirectPermanent("https://www.google.com.br/search?q=" + textSearch + "&source=lnms&tbm=shop");

            }
            else
            {
                /*
                var interesses = (from m in db.Interesse
                                  where m.Interesse1 != "#entrada" && m.Interesse1 != String.Empty
                                  orderby m.Id descending                                 
                                  select m.Interesse1).Distinct().Take(50).ToList();
                */

                var interesses = db.InteresseSelect().ToList();

                return View("Play",interesses);
            }
        }


        public ActionResult SimplexFailure(String msg)
        {
            ViewBag.FailureMessage = msg;

            return RedirectToAction("SimplexFailure", "Simplex");
            //return View("SimplexFailure");
        }

        [AllowAnonymous]
        public ActionResult Solve(SimplexModel model)
        {
            ModelState.Clear();
            return View("Solve", model);
        }

        [AllowAnonymous]
        public ActionResult NotSolve(SimplexModel model)
        {
            ModelState.Clear();
            return View("NotSolve");
        }


        [AllowAnonymous]
        public ActionResult Identity(VariableIdentityModel model)
        {
            ModelState.Clear();
            return View("Identity", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Identity(VariableIdentityModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                SimplexStartModel nextModel = new SimplexStartModel();
                //  nextModel.OriginalDecisionLength = model.LineDescrs.Count;
                nextModel.OriginalDecisionLength = model.LineDescrs.Count - 1;

                model.LineDescrs[0].Label = BaseMvc.GetLabel("GeneralTotalRestriction", this.ViewLanguageId);
                nextModel.LineDescrs = model.LineDescrs;
                List<ColumnsDescr> columnsDescrs = new List<ColumnsDescr>();
                ColumnsDescr columnsDescr = new ColumnsDescr() { DisplayName = "PricePerAccommodation", Description = "Pricetobechargedforeachtypeofaccommodation" };
                columnsDescrs.Add(columnsDescr);

                columnsDescr = new ColumnsDescr() { DisplayName = "MaximumAccommodation", Description = "MaximumGestofeachtypeofAccommodationonRoom" };
                columnsDescrs.Add(columnsDescr);

                columnsDescr = new ColumnsDescr() { DisplayName = "CostPerAccommodation", Description = "MainteneceCostPerEachtypeofAccommodation" };
                columnsDescrs.Add(columnsDescr);

                columnsDescr = new ColumnsDescr() { DisplayName = "TotalAccommodation", Description = "MaximumGestofeachtypeofAccommodationonSite" };
                columnsDescrs.Add(columnsDescr);

                nextModel.ColumnsDescrs = columnsDescrs;
                for (int i = 0; i <= columnsDescrs.Count; i++)
                {
                    StartEquation startEquation = new StartEquation();
                    for (int j = 0; j < model.LineDescrs.Count; j++)
                    {
                        StartCoefficient startCoefficient = new StartCoefficient();
                        startEquation.StartCoefficients.Add(startCoefficient);
                    }

                    nextModel.StartEquations.Add(startEquation);
                }
                ModelState.Clear();
                return Modeling(nextModel);
            }
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult Play()
        {
            ModelState.Clear();

            /*
           var interesses = (from m in db.Interesse
                             where m.Interesse1 != "#entrada" && m.Interesse1 != String.Empty
                             orderby m.Id descending
                             select m).Distinct().Take(50).ToList();
           */
            var interesses = db.InteresseSelect().ToList();

            return View(interesses);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Play(InitialBestPriceViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                VariableIdentityModel nextModel = new VariableIdentityModel();

                for (int i = 0; i <= model.DecisionVariables; i++)
                {
                    LineDescr lineDesc = new LineDescr();
                    lineDesc.Label = string.Empty;
                    nextModel.LineDescrs.Add(lineDesc);
                }

                return Identity(nextModel);
            }
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult Modeling(SimplexStartModel model)
        {
            ModelState.Clear();
            return View("Modeling", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Modeling(SimplexStartModel model, string returnUrl)
        {// [TODO] leitura dos valores digitados e inserção nos objetos de cálculo.
            // Falta definir o limite para cada restrição...
            try
            {
                bool hasSolution = false;
                bool isFinish = model.IsFinish;
                decimal[,] outputSetps = new decimal[,] { };
                SimplexModel simplexModel = new SimplexModel();
                simplexModel.OriginalDecisionLength = model.OriginalDecisionLength;
                if (ModelState.IsValid)
                {
                    model.Goal = "Max(p)";
                    model.StepMethod = "standard";
                    string stepMethod = model.StepMethod;
                    List<Decimal[]> inputSteps = new List<Decimal[]>();
                    //int decisionLength = model.StartEquations[0].StartCoefficients.Count() + 1;
                    int decisionLength = model.StartEquations[0].StartCoefficients.Count();
                    int restrictionLength = 5; // Na modelagem que vai vir do banco de dados este dado será recuperado
                    // O total de acomodação vai formar 3 equações.....
                    Decimal[] inputStep = new Decimal[decisionLength];
                    String[] restrictiongSigns = new String[restrictionLength];
                    String[] linesDescr = new String[] { };
                    String[] columnsDescr = new String[] { };
                    string restrictiongSignsStr = String.Empty;
                    restrictiongSignsStr += "<=,<=,<=,<=,<=";
                    int i = 0;

                    List<FromLabelTo> fromLabelTos = new List<FromLabelTo>();
                    int iLabel = 0;
                    //Decisões
                    for (iLabel = 1; iLabel < model.LineDescrs.Count; iLabel++)
                    {
                        var types = model.LineDescrs[iLabel];
                        FromLabelTo fromLabelTo = new Models.FromLabelTo();
                        fromLabelTo.FromLabel = types.Label;
                        fromLabelTo.ToLabel = "X" + iLabel.ToString();
                        fromLabelTos.Add(fromLabelTo);
                    }

                    //Restrições
                    iLabel--;//  ultimo ingremento ocorreu dentro do for, portanto tem que ajustar.
                    int jLabel = 0;
                    for (jLabel = 1; jLabel < model.ColumnsDescrs.Count - 1; jLabel++)// O primeiro se refere aa função objetivo, e não requer tradução de para
                    {
                        iLabel++;
                        var types = model.ColumnsDescrs[jLabel];
                        FromLabelTo fromLabelTo = new Models.FromLabelTo();
                        fromLabelTo.FromLabel = types.DisplayName;
                        fromLabelTo.ToLabel = "X" + iLabel.ToString();
                        fromLabelTos.Add(fromLabelTo);
                    }

                    // Migra as equações que vão ficar do mesmo jeito
                    foreach (var equation in model.StartEquations)
                    {
                        if (i != (model.StartEquations.Count - 1))
                        {
                            int j = 0;
                            inputStep = new Decimal[decisionLength];
                            foreach (var startCoefficients in equation.StartCoefficients)
                            {
                                inputStep[j] = Util.CoefficientNumber(startCoefficients.Number, this.ViewLanguageId);
                                /*
                                if (j == (decisionLength - 1))
                                {
                                    inputStep[0] = Util.CoefficientNumber(startCoefficients.Number, this.ViewLanguageId);
                                }
                                else
                                {
                                    inputStep[j + 1] = Util.CoefficientNumber(startCoefficients.Number, this.ViewLanguageId);
                                }
                                 * */
                                j++;
                            }
                        }

                        inputSteps.Add(inputStep);
                    }

                    // Equações Ajustadas
                    int ii = model.StartEquations.Count - 1;
                    StartEquation thiStartEquation = model.StartEquations[ii];

                    //foreach (var startCoefficients in thiStartEquation.StartCoefficients)]
                    int icount = 0;
                    for (int j = 1; j < thiStartEquation.StartCoefficients.Count; j++)
                    {
                        inputStep = new Decimal[decisionLength];
                        inputStep[0] = Util.CoefficientNumber(thiStartEquation.StartCoefficients[j].Number, this.ViewLanguageId);
                        inputStep[j] = 1;
                        inputSteps.Add(inputStep);

                        FromLabelTo fromLabelTo = new Models.FromLabelTo();
                        fromLabelTo.FromLabel = model.ColumnsDescrs[model.ColumnsDescrs.Count - 1].DisplayName + "-" + fromLabelTos[icount++].FromLabel;
                        iLabel++;
                        fromLabelTo.ToLabel = "X" + iLabel.ToString();
                        fromLabelTos.Add(fromLabelTo);
                    }

                    inputSteps.RemoveAt(ii);// Reveme o item que foi ajustado.
                    restrictiongSigns = restrictiongSignsStr.Substring(0, restrictiongSignsStr.Length - 1).Split(',');

                    ZMethod zmethod = new ZMethod("X");
                    outputSetps = zmethod.StraightToSolutionMax(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                    columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);


                    simplexModel = ValuesToBack(simplexModel, outputSetps, columnsDescr, linesDescr, isFinish, hasSolution, fromLabelTos);

                    simplexModel.FromLabelTos = fromLabelTos;
                    simplexModel.WindowIntervals = Analisys.AllowedWindow(outputSetps, columnsDescr, linesDescr, model.OriginalDecisionLength);//[TODO] Rever a formaçao desses valores start e end

                    List<RestrictionLimit> restrictionLimits = new List<RestrictionLimit>();
                    for (int iRest = 0; iRest < inputSteps.Count - 1; iRest++)
                    {
                        RestrictionLimit restrictionLimit = new RestrictionLimit();
                        decimal[] e = inputSteps[iRest + 1];
                        restrictionLimit.Number = e[0];
                        restrictionLimit.Sign = restrictiongSigns[iRest];
                        simplexModel.RestrictionLimits.Add(restrictionLimit);
                        if (restrictionLimit.Sign == ">=")
                        {
                            simplexModel.WindowIntervals[iRest].Start = restrictionLimit.Number + simplexModel.WindowIntervals[iRest].Start;
                            simplexModel.WindowIntervals[iRest].End = restrictionLimit.Number + simplexModel.WindowIntervals[iRest].End;
                        }
                        else
                        {
                            decimal aux = restrictionLimit.Number - simplexModel.WindowIntervals[iRest].End;

                            if (simplexModel.WindowIntervals[iRest].Start != 0)
                            {
                                simplexModel.WindowIntervals[iRest].End = restrictionLimit.Number - simplexModel.WindowIntervals[iRest].Start;
                            }
                            else
                            {
                                simplexModel.WindowIntervals[iRest].End = 0;
                            }
                            simplexModel.WindowIntervals[iRest].Start = aux;
                        }

                        if (simplexModel.WindowIntervals[iRest].Start < 0)
                        {
                            simplexModel.WindowIntervals[iRest].Start = 0;
                        }

                        if (simplexModel.WindowIntervals[iRest].End < 0)
                        {
                            simplexModel.WindowIntervals[iRest].End = 0;
                        }
                    }

                    simplexModel.Goal = model.Goal;

                    if ((hasSolution) && (simplexModel.Equations[0].Coefficients[0].Number != 0))
                    {
                        return Solve(simplexModel);//[TODO] Análise de Sensibilidade
                    }
                    else
                    {
                        return NotSolve(simplexModel);
                    }
                }
                else
                {
                    return View("Error");// Error     

                }
            }
            catch (Exception e)
            {
                return SimplexFailure(e.Message);

            }

        }

    }

}