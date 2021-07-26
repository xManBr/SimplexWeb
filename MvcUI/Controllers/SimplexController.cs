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

using System.Web.Globalization;

namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{



    public class SimplexController : BaseMvc
    {
        public SimplexController()
        {
            PreperView();
        }

        private void PreperView()
        {
            List<RetrictionSign> retrictionSigns = new List<RetrictionSign>();
            retrictionSigns.Add(new RetrictionSign() { Sign = "<=" });
            retrictionSigns.Add(new RetrictionSign() { Sign = ">=" });
            retrictionSigns.Add(new RetrictionSign() { Sign = "==" });

            ViewBag.RetrictionSigns = retrictionSigns.Select(x => new { value = x.Sign, text = x.Sign });
            retrictionSigns = null;

            ViewBag.Goal = new SelectList(new[]
                {
                     new { value = "Max(p)", text = BaseMvc.GetLabel("=MaxZ(primal)", this.ViewLanguageId) },
                     new { value = "Min(d)", text = BaseMvc.GetLabel("=MinZ(dual)", this.ViewLanguageId) },
                     new { value = "Min(p)", text = BaseMvc.GetLabel("=MinZ(primal)", this.ViewLanguageId) }
                   
                }, "value", "text");
        }

        public ActionResult SimplexFailure(String msg)
        {
            ViewBag.FailureMessage = msg;

            return View("SimplexFailure");
        }


        [AllowAnonymous]
        public ActionResult Help()
        {

            return View("Help");
        }
        //
        // GET: /Simplex/
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult StepByStep(SimplexModel model)
        {
            ModelState.Clear();
            return View("StepByStep", model);

        }

        [AllowAnonymous]
        public ActionResult StepByStepTransMatrix(SimplexModel model)
        {
            ModelState.Clear();
            return View("StepByStepTransMatrix", model);
        }

        [AllowAnonymous]
        public ActionResult StepByStepDualToPrimal(SimplexModel model)
        {
            ModelState.Clear();
            return View("StepByStepDualToPrimal", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult StepByStepDualToPrimal(SimplexModel model, string returnUrl)
        {
            SimplexModel simplexModel = new SimplexModel();
            bool isFinish = false;
            bool hasSolution = false;
            decimal[,] outputSetps = new decimal[,] { };
            if (ModelState.IsValid)
            {
                //Proximo passo              
                ZMethod z = new ZMethod("X");
                decimal[,] inputSteps = z.SetpByStepAdjustment(model.Equations);
                isFinish = model.IsFinish;
                hasSolution = model.HasSolution;
                string stepMethod = "standard";// model.StepMethod;

                String[] linesDescr = new String[model.LineDescrs.Count];
                String[] columnsDescr = new String[model.ColumnsDescrs.Count];

                int i = 0;
                foreach (var item in model.LineDescrs)
                {
                    linesDescr[i] = item.Label;
                    i++;

                }
                int jj = 0;
                foreach (var item in model.ColumnsDescrs)
                {
                    columnsDescr[jj] = item.Label;
                    jj++;

                }
                int decisionLength = inputSteps.GetLength(1) - 1;
                outputSetps = z.DualToPrimal(decisionLength, inputSteps, ref columnsDescr, ref linesDescr);
                z.LabelAdjDual(ref linesDescr, ref columnsDescr, "X", BaseMvc.GetLabel("Value", this.ViewLanguageId), "Y");

                simplexModel.Goal = model.Goal;
                simplexModel.StepByStep = model.StepByStep;
                simplexModel.StepMethod = stepMethod;
                simplexModel = ValuesToBack(simplexModel, outputSetps, columnsDescr, linesDescr, isFinish, hasSolution);


            }

            return Solve(simplexModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult StepByStepTransMatrix(SimplexModel model, string returnUrl)
        {
            // Modolo para o  callback do Min dual Step by Step....
            bool hasSolution = false;
            bool isFinish = model.IsFinish;
            decimal[,] outputSetps = new decimal[,] { };
            SimplexModel simplexModel = new SimplexModel();

            if (ModelState.IsValid)
            {
                string stepMethod = model.StepMethod;
                List<Decimal[]> inputSteps = new List<Decimal[]>();
                int decisionLength = model.Equations[0].Coefficients.Count();
                int restrictionLength = model.Equations.Count();
                Decimal[] inputStep = new Decimal[] { };
                String[] restrictiongSigns = new String[model.Equations.Count];
                String[] linesDescr = new String[] { };
                String[] columnsDescr = new String[] { };
                string restrictiongSignsStr = String.Empty;
                foreach (var equation in model.Equations)
                {
                    int j = 0;
                    restrictiongSignsStr += equation.Coefficients[0].Number > 0 ? "<=" + "," : ",";// No dual o sinal sera sempre >=
                    inputStep = new Decimal[decisionLength];
                    foreach (var coefficients in equation.Coefficients)
                    {
                        inputStep[j] = coefficients.Number;
                        j++;
                    }
                    inputSteps.Add(inputStep);
                }

                restrictiongSigns = restrictiongSignsStr.Substring(0, restrictiongSignsStr.Length - 1).Split(',');
                simplexModel.VariLabel = "Y";
                ZMethod zmethod = new ZMethod(simplexModel.VariLabel);
                outputSetps = zmethod.SetpByStepMax(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);

                simplexModel.Goal = model.Goal;
                simplexModel.StepByStep = model.StepByStep;
                simplexModel = ValuesToBack(simplexModel, outputSetps, columnsDescr, linesDescr, isFinish, hasSolution);
                simplexModel.StepMethod = stepMethod;
                return StepByStep(simplexModel);
            }
            else
            {
                return View("Error");// Error
            }
        }


        [AllowAnonymous]
        public ActionResult StepByStepTable(SimplexModel model)
        {
            ModelState.Clear();
            return View("StepByStepTable", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult StepByStepTable(SimplexModel model, string returnUrl)
        {
            SimplexModel simplexModel = new SimplexModel();
            bool isFinish = model.IsFinish;
            bool hasSolution = model.HasSolution;
            decimal[,] outputSetps = new decimal[,] { };
            if (ModelState.IsValid)
            {
                //Proximo passo              
                ZMethod z = new ZMethod(model.VariLabel);
                decimal[,] inputSteps = z.SetpByStepAdjustment(model.Equations);
                isFinish = model.IsFinish;
                hasSolution = model.HasSolution;
                string stepMethod = model.StepMethod;

                String[] linesDescr = new String[model.LineDescrs.Count];
                String[] columnsDescr = new String[model.ColumnsDescrs.Count];

                int i = 0;
                foreach (var item in model.LineDescrs)
                {
                    linesDescr[i] = item.Label;
                    i++;

                }
                int jj = 0;
                foreach (var item in model.ColumnsDescrs)
                {
                    columnsDescr[jj] = item.Label;
                    jj++;

                }
                outputSetps = z.SetpByStep(ref stepMethod, inputSteps, ref columnsDescr, ref linesDescr, ref isFinish, ref hasSolution);

                simplexModel.Goal = model.Goal;
                simplexModel.StepByStep = model.StepByStep;
                simplexModel.StepMethod = stepMethod;
                simplexModel = ValuesToBack(simplexModel, outputSetps, columnsDescr, linesDescr, isFinish, hasSolution);

                //Saiu
                foreach (var old in model.LineDescrs)
                {
                    if (!linesDescr.Contains(old.Label) && !old.Label.Contains("Z"))
                    {
                        simplexModel.goOut = old.Label;
                        break;
                    }
                }
                //Entrou
                for (int ii = 1; ii < linesDescr.Length; ii++)
                {

                    if (!model.LineDescrs.Exists(x => x.Label == linesDescr[ii]))
                    {
                        simplexModel.goOn = linesDescr[ii];
                    }
                }
            }
            
            if ((isFinish == false) || (hasSolution == false))
            {
                return StepByStepTable(simplexModel);
            }
            else
            {
                if (simplexModel.Goal != "Min(d)")
                {
                    return Solve(simplexModel);
                }
                else
                {
                    simplexModel.VariLabel = "X";
                    return StepByStepDualToPrimal(simplexModel);
                }
            }
        }

        [AllowAnonymous]
        public ActionResult Play()
        {
            ModelState.Clear();
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Play(InitialSimplexViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                SimplexStartModel nextModel = new SimplexStartModel();
                nextModel.OriginalDecisionLength = model.DecisionVariables;

                for (int i = 0; i <= model.RestrictionsVariables; i++)
                {
                    StartEquation startEquation = new StartEquation();
                    for (int j = 0; j <= model.DecisionVariables; j++)
                    {
                        StartCoefficient startCoefficient = new StartCoefficient();
                        startEquation.StartCoefficients.Add(startCoefficient);
                    }

                    nextModel.StartEquations.Add(startEquation);
                }
                //model.DecisionVariables
                // ModelState.AddModelError("", "Invalid username or password.");
                ModelState.Clear();
                return Modeling(nextModel);
            }

            // If we got this far, something failed, redisplay form
            return View("Play", model);
        }

        private SimplexModel ValuesToBack(SimplexModel simplexModel, decimal[,] outputSetps, String[] columnsDescr, String[] linesDescr, bool isFinish, bool hasSolution)
        {
            simplexModel.IsFinish = isFinish;
            simplexModel.HasSolution = hasSolution;

            foreach (var thisColumns in columnsDescr.ToList())
            {
                ColumnsDescr itemColumn = new Models.ColumnsDescr();
                itemColumn.Label = thisColumns;
                simplexModel.ColumnsDescrs.Add(itemColumn);
            }

            foreach (var thisLine in linesDescr.ToList())
            {
                LineDescr itemLine = new LineDescr();
                itemLine.Label = thisLine;
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
        public ActionResult Modeling(SimplexStartModel model)
        {
            ModelState.Clear();
            return View("Modeling", model);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Modeling(SimplexStartModel model, string returnUrl)
        {
            try
            {
                bool hasSolution = false;
                bool isFinish = model.IsFinish;
                decimal[,] outputSetps = new decimal[,] { };
                SimplexModel simplexModel = new SimplexModel();
                simplexModel.OriginalDecisionLength = model.OriginalDecisionLength;
                if (ModelState.IsValid)
                {
                    model.StepMethod = (model.Goal == "Min(p)" ? "notStandard" : "standard");
                    string stepMethod = model.StepMethod;
                    List<Decimal[]> inputSteps = new List<Decimal[]>();
                    int decisionLength = model.StartEquations[0].StartCoefficients.Count() + 1;
                    int restrictionLength = model.StartEquations.Count();
                    Decimal[] inputStep = new Decimal[decisionLength];
                    String[] restrictiongSigns = new String[model.StartEquations.Count];//String[] restrictiongSigns = new String[model.Equations.Count - 1];
                    String[] linesDescr = new String[] { };
                    String[] columnsDescr = new String[] { };
                    string restrictiongSignsStr = String.Empty;
                    foreach (var equation in model.StartEquations)
                    {
                        int j = 0;
                        restrictiongSignsStr += equation.RetrictionSign + ",";
                        inputStep = new Decimal[decisionLength];
                        foreach (var startCoefficients in equation.StartCoefficients)
                        {
                            if (j == (decisionLength - 1))
                            {
                                inputStep[0] = Util.CoefficientNumber(startCoefficients.Number, this.ViewLanguageId);
                            }
                            else
                            {
                                inputStep[j + 1] = Util.CoefficientNumber(startCoefficients.Number, this.ViewLanguageId);
                            }
                            j++;
                        }
                        inputSteps.Add(inputStep);
                    }
                    restrictiongSigns = restrictiongSignsStr.Substring(0, restrictiongSignsStr.Length - 1).Split(',');



                    if ((model.Goal == "Min(d)") || (model.Goal == "Min(p)"))
                    {
                        if (model.Goal == "Min(d)")
                        {
                            ZMethod zmethod = new ZMethod("X");
                            if (!model.StepByStep)
                            {
                                outputSetps = zmethod.StraightToSolutionMinDual(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                zmethod.LabelAdjDual(ref linesDescr, ref columnsDescr, "X", BaseMvc.GetLabel("Value", this.ViewLanguageId));
                            }
                            else
                            {
                                outputSetps = zmethod.SetpByStepMinDual(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                if (inputSteps.Count > 0)
                                {
                                    simplexModel.OriginalDecisionLength = inputSteps[0].Count() - 1;
                                }
                            }
                        }
                        else
                        {// Min Primal
                            WMethod wmethod = new WMethod("X");
                            if (!model.StepByStep)
                            {
                                outputSetps = wmethod.StraightToSolutionMinPrim(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                            }
                            else
                            {
                                outputSetps = wmethod.SetpByStepMinPrim(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);
                            }
                        }
                    }
                    else//Max - primal
                    {
                        if (restrictiongSigns.Contains("==") || (restrictiongSigns.Contains(">=")))
                        {// Max não padrão
                            XMethod xmethod = new XMethod("X");
                            if (!model.StepByStep)
                            {
                                outputSetps = xmethod.StraightToSolutionMax(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);
                            }
                            else
                            {
                                outputSetps = xmethod.SetpByStepMax(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);
                                stepMethod = "notStandard"; // Quando a Max não é do tipo padrão e eh passo a passo tem que mudar o método para calculo 
                            }
                        }
                        else
                        {
                            ZMethod zmethod = new ZMethod("X");
                            if (!model.StepByStep)
                            {
                                outputSetps = zmethod.StraightToSolutionMax(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);
                            }
                            else
                            {
                                outputSetps = zmethod.SetpByStepMax(inputSteps, restrictiongSigns, ref columnsDescr, ref linesDescr, ref hasSolution);
                                columnsDescr[0] = BaseMvc.GetLabel("Value", this.ViewLanguageId);
                            }
                        }
                    }

                    simplexModel.Goal = model.Goal;
                    simplexModel.StepByStep = model.StepByStep;
                    simplexModel = ValuesToBack(simplexModel, outputSetps, columnsDescr, linesDescr, isFinish, hasSolution);
                    simplexModel.StepMethod = stepMethod;
                    if (!simplexModel.StepByStep)
                    {

                        if ((hasSolution) && (simplexModel.Equations[0].Coefficients[0].Number != 0))
                        {
                            return Solve(simplexModel);
                        }
                        else
                        {
                            return NotSolve(simplexModel);
                        }
                    }
                    else
                    {
                        if (simplexModel.Goal != "Min(d)")
                        {
                            return StepByStep(simplexModel);//View("StepByStep", simplexModel);
                        }
                        else
                        {
                            return StepByStepTransMatrix(simplexModel);
                        }
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