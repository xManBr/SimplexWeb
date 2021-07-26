using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class WMethod
    {
        /*
         * Método Simplex para Minização.... 
         * */

        private String VariLabel { get; set; }

        public WMethod(String variLabel)
        {
            this.VariLabel = variLabel;

        }

        public void LabelAdj(ref String[] linesDescr, ref String[] columnsDescr, String label, String valueLabel)
        {
            for (int i = 1; i < linesDescr.Length; i++)
            {
                linesDescr[i] = label + linesDescr[i];
            }
            columnsDescr[0] = valueLabel;
        }

        #region Step by Step
        public decimal[,] SetpByStepMinPrim(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                outputSteps = this.Adjustment(inputSteps, ref restrictionSign);
                //int decisionLength = outputSteps.GetLength(1) - 1;
                outputSteps = this.PreperInitialMatrix(outputSteps, restrictionSign, ref columnsDescr, ref linesDescr, ref hasSolution);
            }

            return outputSteps;
        }
        #endregion

        #region Preper Min Dual
        public decimal[,] StraightToSolutionMinPrim(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                outputSteps = this.Adjustment(inputSteps, ref restrictionSign);
                outputSteps = InitialMatrix(outputSteps, restrictionSign, ref columnsDescr, ref linesDescr, ref hasSolution);
            }

            return outputSteps;
        }

        public decimal[,] Adjustment(List<decimal[]> inputSteps, ref String[] restrictionSign)
        {
            decimal[,] outputSteps = new decimal[,] { };

            int i = 0;
            int lines = inputSteps.Count();
            int columns = inputSteps[0].GetLength(0);
            if (lines > 0)
            {
                outputSteps = new decimal[inputSteps.Count(), columns];
                i = 0;
                foreach (var inputStep in inputSteps)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        outputSteps[i, j] = inputStep[j];
                    }
                    i++;
                }
            }

            return outputSteps;
        }
        #endregion

        public decimal[,] PreperInitialMatrix(decimal[,] inputStep, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            //
            Int32 decisionLengthOrginal = inputStep.GetLength(1) - 1;
            int decisionLength = decisionLengthOrginal;
            int restrictionLength = inputStep.GetLength(0) - 1;
            int lines = inputStep.GetLength(0) + 1;// Presum que a matrix original vem sem a coluna W
            int dobleColumns = 0;
            foreach (var restriction in restrictionSign)
            {
                if (restriction == ">=")
                {
                    dobleColumns++;
                }
            }
            Int32 lastColumn = decisionLength;
            columnsDescr = new String[1 + decisionLength + restrictionLength + dobleColumns];
            columnsDescr[0] = "Z";
            for (int j = 1; j <= decisionLength; j++)
            {
                columnsDescr[j] = this.VariLabel + j.ToString();
            }
            linesDescr = new String[lines];

            decimal[,] outputSetp = new decimal[lines, columnsDescr.Length];

            for (int i = 0; i < inputStep.GetLength(0); i++)
            {
                for (int j = 0; j < inputStep.GetLength(1); j++)
                {
                    outputSetp[i, j] = inputStep[i, j];
                }
            }

            // definindo as ppsições das variaveis de folga e artificais
            int artificialLength = 0;
            for (int i = 1; i < lines - 1; i++)
            {// Nesta fase não precisa incluir a linha W nem a Z
                if (restrictionSign[i] == ">=")
                {
                    lastColumn++;
                    decisionLength++;
                    outputSetp[i, lastColumn] = -1;
                    columnsDescr[lastColumn] = this.VariLabel + decisionLength;

                    lastColumn++;
                    artificialLength++;
                    outputSetp[i, lastColumn] = 1;

                    columnsDescr[lastColumn] = "A" + artificialLength;
                    linesDescr[i] = columnsDescr[lastColumn];
                }
                else if (restrictionSign[i] == "<=")
                {
                    lastColumn++;
                    decisionLength++;
                    outputSetp[i, lastColumn] = 1;

                    columnsDescr[lastColumn] = this.VariLabel + decisionLength;
                    linesDescr[i] = columnsDescr[lastColumn];
                }
                else
                {//=
                    lastColumn++;
                    artificialLength++;
                    outputSetp[i, lastColumn] = 0;

                    columnsDescr[lastColumn] = "A" + artificialLength;
                    linesDescr[i] = columnsDescr[lastColumn];
                }
            }
            // Calculando a linha W
            for (int j = 0; j <= lastColumn; j++)
            {
                if ((columnsDescr[j] == null) || (!columnsDescr[j].Contains("A")))
                {
                    decimal wTotal = 0;
                    for (int i = 1; i < lines - 1; i++)
                    {// Não pega a ultima linha, pois compo-la é o obetivo deste bloco
                        if ((linesDescr[i] == null) || (linesDescr[i].Contains("A")))
                        {
                            wTotal += outputSetp[i, j];
                        }
                    }
                    outputSetp[lines - 1, j] = wTotal * -1;
                }
            }

            linesDescr[0] = "-Z";
            linesDescr[linesDescr.Length - 1] = "W";

            return outputSetp;

        }

        public decimal[,] InitialMatrix(decimal[,] inputStep, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            inputStep = PreperInitialMatrix(inputStep, restrictionSign, ref columnsDescr, ref linesDescr, ref hasSolution);
            ZMethod z = new ZMethod("X");
            inputStep = z.Evaluation("primal", inputStep, ref columnsDescr, ref linesDescr, ref hasSolution);
            z = null;

            return inputStep;
        }
    }
}