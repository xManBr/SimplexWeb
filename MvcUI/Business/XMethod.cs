using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mercoplano.Simplex.Server.MvcUI.Models;


namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class XMethod
    {
        /*
         * Tentativa - Maximinização, por dual(com variaveis artificiais)
         * - Receber os dados
         * - Produzir a matriz transposta
         * - fazer a minimização pelo método de variaveis artificiais
         * - votar á matriz original
         * */

        private int iterateLimit = 100;
        private String VariLabel { get; set; }

        public XMethod(String variLabel)
        {
            this.VariLabel = variLabel;

        }


        public decimal[,] SetpByStepMax(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                ZMethod z = new ZMethod("X");
                WMethod w = new WMethod("X");
                decimal[] inputStep = (decimal[])inputSteps.First();
                outputSteps = w.Adjustment(inputSteps, ref restrictionSign);

                int decisionLength = outputSteps.GetLength(1) - 1;
                outputSteps = this.PreperInitialMatrix(outputSteps, restrictionSign, ref columnsDescr, ref linesDescr, ref hasSolution);
            }

            return outputSteps;
        }
        
        #region Preper Max Primal X
        public decimal[,] StraightToSolutionMax(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                ZMethod z = new ZMethod("X");
                WMethod w = new WMethod("X");
                decimal[] inputStep = (decimal[])inputSteps.First();
                outputSteps = w.Adjustment(inputSteps, ref restrictionSign);
                outputSteps = this.PreperInitialMatrix(outputSteps, restrictionSign, ref columnsDescr, ref linesDescr, ref hasSolution);
                outputSteps = z.Evaluation("primal", outputSteps, ref columnsDescr, ref linesDescr, ref hasSolution);
                z = null;
                w = null;
            }

            return outputSteps;
        }

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
                    if (j > 0)// Esta proposta tem que negativar as variaveis da linha Z.
                    {
                        outputSetp[i, j] = i != 0 ? inputStep[i, j] : inputStep[i, j] * -1;
                    }
                    else
                    {
                        outputSetp[i, j] = inputStep[i, j];
                    }
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
        #endregion

    }
}