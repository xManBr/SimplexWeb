using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mercoplano.Simplex.Server.MvcUI.Models;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class ZMethod
    {

        private int iterateLimit = 100;
        private String VariLabel { get; set; }

        public ZMethod(String variLabel)
        {
            this.VariLabel = variLabel;

        }

        public void LabelAdjDual(ref String[] linesDescr, ref String[] columnsDescr, String label, String valueLabel)
        {
            for (int i = 1; i < linesDescr.Length; i++)
            {
                linesDescr[i] = label + linesDescr[i];
            }
            columnsDescr[0] = valueLabel;

            for (int j = 1; j < linesDescr.Length; j++)
            {
                linesDescr[j] = linesDescr[j].Replace("Y", "label");
            }
        }

        public void LabelAdjDual(ref String[] linesDescr, ref String[] columnsDescr, String label, String valueLabel, String replaceLabel)
        {
            for (int i = 1; i < linesDescr.Length; i++)
            {
                linesDescr[i] = label + linesDescr[i];
            }

            columnsDescr[0] = valueLabel;
            for (int j = 1; j < columnsDescr.Length; j++)
            {
                columnsDescr[j] = columnsDescr[j].Replace(replaceLabel, label);
            }
        }

        public decimal[,] StraightToSolutionMax(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                outputSteps = this.Adjustment(inputSteps, ref restrictionSign); //O Ajuste de Max primal não deve ser o mesmo do Min Dual
                int decisionLength = outputSteps.GetLength(1) - 1;
                outputSteps = this.InitialMatrix(decisionLength, outputSteps, ref columnsDescr, ref linesDescr, ref hasSolution);
            }

            return outputSteps;
        }
        #region Preper Max Primal
        public decimal[,] Adjustment(List<decimal[]> inputSteps, ref String[] restrictionSign)
        {//Ajuste para Max Primal
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
                restrictionSign = new String[outputSteps.GetLength(0)];
            }

            return outputSteps;
        }

        #endregion
        #region Preper Min Dual
        public decimal[,] StraightToSolutionMinDual(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                decimal[] inputStep = (decimal[])inputSteps.First();
                outputSteps = this.AdjustmenToDual(inputSteps, ref restrictionSign);
                outputSteps = this.TransposedMatrix(outputSteps);
                int decisionLength = outputSteps.GetLength(1) - 1;
                outputSteps = this.InitialMatrix(decisionLength, outputSteps, ref columnsDescr, ref linesDescr, ref hasSolution);
                outputSteps = this.DualToPrimal(decisionLength, outputSteps, ref columnsDescr, ref linesDescr);
            }

            return outputSteps;
        }

        public decimal[,] AdjustmenToDual(List<decimal[]> inputSteps, ref String[] restrictionSign)
        {// Ajusta para metodo dual

            decimal[,] outputSteps = new decimal[,] { };

            int i = 0;
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                int columns = inputSteps.ElementAt(0).Length;

                foreach (var restriction in restrictionSign.ToList())
                {
                    decimal[] inputStep = inputSteps.ElementAt(i);
                    if (restriction == "<=")
                    {
                        for (int j = 0; j < inputStep.Length; j++)
                        {
                            inputStep[j] = inputStep[j] * -1;
                        }
                        restrictionSign[i] = ">=";
                    }
                    else if (restriction == "==")
                    {
                        decimal[] outputStep = new decimal[columns];
                        for (int j = 0; j < inputStep.Length; j++)
                        {
                            outputStep[j] = inputStep[j];// >=
                            //inputSteps.Add(outputStep);
                            inputStep[j] = inputStep[j] * -1;// <=  
                        }
                        inputSteps.Add(outputStep);
                    }
                    i++;
                }

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
                restrictionSign = new String[outputSteps.GetLength(0)];
                for (int line = 1; line < restrictionSign.Length; line++)
                {
                    restrictionSign[line] = ">=";
                }
            }

            return outputSteps;
        }

        public decimal[,] TransposedMatrix(decimal[,] inputStep)
        {
            int lines = inputStep.GetLength(0);
            int columns = inputStep.GetLength(1);

            decimal[,] outputSetp = new decimal[columns, lines];

            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    outputSetp[j, i] = inputStep[i, j];
                }
            }

            return outputSetp;

        }

        public decimal[,] DualToPrimal(int decicionsLength, decimal[,] inputStep, ref String[] columnsDescr, ref String[] linesDescr)
        {
            decicionsLength = inputStep.GetLength(1) - inputStep.GetLength(0);
            int[] linesDescrNumberOld = new int[linesDescr.Length];
            for (int i = 1; i < linesDescr.Length; i++)
            {
                linesDescrNumberOld[i] = Convert.ToInt32(linesDescr[i].Substring(1));
            }

            int lines = inputStep.GetLength(0);
            int columns = inputStep.GetLength(1);
            string linesDescrStr = "Z";

            //Verificando quantas linhas deve ter a matrix de saida
            int countBaseLines = 1;// Incluindo o Z.
            for (int j = 1; j < columns; j++)
            {
                if (inputStep[0, j] > 0)
                {//Esta na base 
                    countBaseLines++;
                }
            }

            decimal[,] outputSetp = new decimal[countBaseLines, columns];
            Int32 restrictionLength = columns - 1 - decicionsLength;
            int[] fromTo = new int[columns];

            //  Recuperado Valor
            int lineCount = 1;
            int dualColumns = 0;
            for (int j = 1; j < columns; j++)
            {
                dualColumns = j <= decicionsLength ? j + restrictionLength : j - decicionsLength;
                fromTo[j] = dualColumns;
                if (inputStep[0, j] > 0)
                {//Esta na base                   
                    linesDescrStr += "," + dualColumns.ToString();
                    outputSetp[lineCount++, 0] = inputStep[0, j];
                }
            }
            //Recuperando Z
            outputSetp[0, 0] = inputStep[0, 0];
            for (int line = 1; line < lines; line++)
            {
                dualColumns = fromTo[linesDescrNumberOld[line]];
                outputSetp[0, dualColumns] = inputStep[line, 0];
            }


            String[] lineDescrNew = linesDescrStr.Split(',');
            //Recuperando os dados do miolo da matriz
            for (int j = 1; j < columns; j++)
            {
                if (inputStep[0, j] > 0)
                {
                    dualColumns = fromTo[j];
                    int dualLine = this.GetIndex(lineDescrNew, fromTo[j].ToString());

                    for (int line = 1; line < linesDescrNumberOld.Length; line++)
                    {
                        int varOldline = linesDescrNumberOld[line];
                        dualColumns = fromTo[varOldline];
                        outputSetp[dualLine, dualColumns] = inputStep[line, j] * -1;
                    }
                }
            }

            // Colocando 1 na posição linha = columns.
            for (int i = 1; i < lineDescrNew.Length; i++)
            {
                outputSetp[i, Convert.ToInt32(lineDescrNew[i])] = 1;
            }

            linesDescr = lineDescrNew;
            return outputSetp;

        }

        #endregion

        private int GetIndex(String[] arr, string compareTo)
        {
            Int32 ret = 0;
            for (int j = 0; j < arr.Length; j++)
            {
                if (arr[j] == compareTo)
                {
                    return j;
                }
            }
            return ret;
        }

        #region SetpByStep

        public decimal[,] SetpByStepAdjustment(List<Equation> equations)
        {
            // Transforma a lista numa matriz bidirecionl
            decimal[,] outputSteps = new decimal[,] { };

            int decisionLength = equations[0].Coefficients.Count();
            int restrictionLength = equations.Count();
            outputSteps = new decimal[restrictionLength, decisionLength];
            int i = 0;
            foreach (var equation in equations)
            {
                for (int j = 0; j < decisionLength; j++)
                {
                    outputSteps[i, j] = equation.Coefficients[j].Number;
                }
                i++;
            }
            return outputSteps;
        }

        public decimal[,] SetpByStepAdjustmentOLd(List<decimal[]> inputSteps)
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

        public decimal[,] SetpByStepMax(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                outputSteps = this.Adjustment(inputSteps, ref restrictionSign); //O Ajuste de Max primal não deve ser o mesmo do Min Dual
                int decisionLength = outputSteps.GetLength(1) - 1;
                outputSteps = this.PreperInitialMatrix(decisionLength, outputSteps, ref columnsDescr, ref linesDescr, ref hasSolution);
            }

            return outputSteps;
        }

        public decimal[,] SetpByStepMinDual(List<decimal[]> inputSteps, String[] restrictionSign, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            decimal[,] outputSteps = new decimal[,] { };
            int lines = inputSteps.Count();
            if (lines > 0)
            {
                decimal[] inputStep = (decimal[])inputSteps.First();
                outputSteps = this.AdjustmenToDual(inputSteps, ref restrictionSign);
                outputSteps = this.TransposedMatrix(outputSteps);

                String columnsDescrStr = String.Empty;
                for (int j = 1; j < outputSteps.GetLength(1); j++)
                {
                    columnsDescrStr += ",Y" + j.ToString();
                }

                columnsDescr = columnsDescrStr.Split(',');
            }

            linesDescr = new String[inputSteps.Count];// para nao dar erro na preparação para mostrar equação na tela

            return outputSteps;
        }

        public decimal[,] SetpByStep(ref String stepMethod, decimal[,] inputStep, ref String[] columnsDescr, ref String[] linesDescr, ref bool isFinish, ref bool hasSolution)
        {
            if (stepMethod == "standard")
            {
                inputStep = Standard(inputStep, ref isFinish, ref columnsDescr, ref linesDescr, ref hasSolution);

            }
            else if (stepMethod == "notStandard")
            {// variaveis artificiais
                if ((!hasSolution) && (isFinish))// Se ainda não tem solução, mas finalizou corretamente a etapa das variáveis artificiais então parte para a etapa do simplex padrão
                {
                    // Segunda etapa - verificando se ainda tem varivavel negativa em Z depois de Verificar W.
                    // Ajustado as colunas da matrix para rodar simplex padrão
                    // Ainda não terminou, tem que partir para uma nova iteração
                    int lines = inputStep.GetLength(0); // --> A partir deste ponto a linha W não eh mais necessária
                    int columns = inputStep.GetLength(1);

                    decimal[,] outputSted = new decimal[lines, columns];
                    outputSted = inputStep;
                    int newColumns = columns - columnsDescr.Count(x => x.Contains("A"));
                    int newLines = inputStep.GetLength(0) - 1; // --> A partir deste ponto a linha W não eh mais necessária

                    string[] newColumnsDescr = new String[newColumns];
                    String[] newLinesDescr = new String[newLines];
                    inputStep = new decimal[newLines, newColumns];
                    int nextColumn = 0;
                    for (int j = 0; j < columns; j++)
                    {
                        if (!columnsDescr[j].Contains("A"))
                        {
                            newColumnsDescr[nextColumn] = columnsDescr[j];
                            for (int i = 0; i < newLines; i++)
                            {
                                inputStep[i, nextColumn] = outputSted[i, j];
                            }

                            nextColumn++;
                        }
                    }

                    newLinesDescr[0] = "Z";
                    for (int i = 1; i < newLinesDescr.Length; i++)
                    {
                        newLinesDescr[i] = linesDescr[i];
                    }

                    linesDescr = newLinesDescr;
                    columnsDescr = newColumnsDescr;

                    stepMethod = "standard";
                    //
                    isFinish = false;
                    inputStep = Standard(inputStep, ref isFinish, ref columnsDescr, ref linesDescr, ref hasSolution);

                }
                else
                {
                    inputStep = NotStandard(inputStep, ref isFinish, ref columnsDescr, ref linesDescr, ref hasSolution);

                    //Verificando se a solução NotStandard também satisfaz a solução Standard
                    if ((hasSolution) && (isFinish))
                    {
                        for (int j = 1; j < inputStep.GetLength(1); j++)
                        {
                            if (!columnsDescr[j].Contains("A"))
                            {
                                if (inputStep[0, j] < 0)
                                {
                                    hasSolution = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return inputStep;
            // Se lineBaseStep = 0 trata-se de simples padrão, e tem que verirficar se Z tem algum valor negativo
            // se lineBaseStep > 0 trata-se de simplex não padrão por variaveis artificiais, então verificar se W tem valor zero na coluna de valores (coluna zero). 
        }
        #endregion

        public decimal[,] PreperInitialMatrix(int decisionLength, decimal[,] inputStep, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            Int32 restrictionLength = inputStep.GetLength(0) - 1;

            int additionalLength = 1;
            int lines = inputStep.GetLength(0);

            columnsDescr = new String[decisionLength + restrictionLength + additionalLength];
            //columnsDescr = new String[decisionLength * 2 + additionalLength];
            columnsDescr[0] = "Z";
            for (int j = 1; j < columnsDescr.Length; j++)
            {
                columnsDescr[j] = this.VariLabel + j.ToString();
            }

            linesDescr = new String[lines];
            linesDescr[0] = "Z";
            for (int i = 1; i < linesDescr.Length; i++)
            {
                linesDescr[i] = this.VariLabel + (i + decisionLength).ToString();
            }

            decimal[,] outputSetp = new decimal[lines, decisionLength + restrictionLength + additionalLength];

            //Montando a matrix zero do Dual.

            for (int i = 0; i < inputStep.GetLength(0); i++)
            {
                for (int j = 0; j < outputSetp.GetLength(1); j++)// outputSetp.GetLength(1) eh necessario para percorrer até as colunas da folga
                {
                    if (j < inputStep.GetLength(1))// se a coluna for até a última variavel de decisão
                    {
                        if (j > 0) // Acrescentando variaveis de sobra
                        {
                            outputSetp[i, j] = i != 0 ? inputStep[i, j] : inputStep[i, j] * -1;
                        }
                        else
                        {
                            outputSetp[i, j] = inputStep[i, j];
                        }
                    }
                    else
                    {
                        if (linesDescr[i] == columnsDescr[j])
                        {
                            outputSetp[i, j] = 1;
                        }
                    }
                }
            }

            return outputSetp;
        }

        public decimal[,] InitialMatrix(int decisionLength, decimal[,] inputStep, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {

            /*
            Int32 restrictionLength = inputStep.GetLength(0) - 1;

            int additionalLength = 1;
            int lines = inputStep.GetLength(0);

            columnsDescr = new String[decisionLength + restrictionLength + additionalLength];
            //columnsDescr = new String[decisionLength * 2 + additionalLength];
            columnsDescr[0] = "Z";
            for (int j = 1; j < columnsDescr.Length; j++)
            {
                columnsDescr[j] = "X" + j.ToString();
            }

            linesDescr = new String[lines];
            linesDescr[0] = "Z";
            for (int i = 1; i < linesDescr.Length; i++)
            {
                linesDescr[i] = "X" + (i + decisionLength).ToString();
            }

            decimal[,] outputSetp = new decimal[lines, decisionLength + restrictionLength + additionalLength];

            //Montando a matrix zero do Dual.

            for (int i = 0; i < inputStep.GetLength(0); i++)
            {
                for (int j = 0; j < outputSetp.GetLength(1); j++)// outputSetp.GetLength(1) eh necessario para percorrer até as colunas da folga
                {
                    if (j < inputStep.GetLength(1))// se a coluna for até a última variavel de decisão
                    {
                        if (j > 0) // Acrescentando variaveis de sobra
                        {
                            outputSetp[i, j] = i != 0 ? inputStep[i, j] : inputStep[i, j] * -1;
                        }
                        else
                        {
                            outputSetp[i, j] = inputStep[i, j];
                        }
                    }
                    else
                    {
                        if (linesDescr[i] == columnsDescr[j])
                        {
                            outputSetp[i, j] = 1;
                        }
                    }
                }
            }
             * 
             * */

            inputStep = PreperInitialMatrix(decisionLength, inputStep, ref columnsDescr, ref linesDescr, ref hasSolution);
            inputStep = this.Evaluation("dual", inputStep, ref columnsDescr, ref linesDescr, ref hasSolution);
            return inputStep;
        }

        public decimal[,] Evaluation(String method, decimal[,] inputStep, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {// a rotina que cham este metodo eh que deve verificar se trata-se de min ou max e montar o matriz zero.
            bool isFinish;
            int iterarion = 0;
            if (method == "dual")
            {
                isFinish = false;
                iterarion = 0;
                while ((!isFinish) || (iterarion > iterateLimit))
                {//[TODO] Se não houve alteração na base desde a última iteração e ainda gerou solução, então isso quer dizer que o processo deve ser abortado desde então, pois qualquer processamento adicional é desnecessário
                    inputStep = Standard(inputStep, ref isFinish, ref columnsDescr, ref linesDescr, ref hasSolution);
                    iterarion++;
                    if (!hasSolution)
                    {
                        break;
                    }
                }
            }
            else
            {// variaveis artificiais
                isFinish = false;
                iterarion = 0;
                while ((!isFinish) || (iterarion > iterateLimit))
                {//[TODO] Se não houve alteração na base desde a última iteração e ainda gerou solução, então isso quer dizer que o processo deve ser abortado desde então, pois qualquer processamento adicional é desnecessário
                    inputStep = NotStandard(inputStep, ref isFinish, ref columnsDescr, ref linesDescr, ref hasSolution);
                    iterarion++;
                    if (!hasSolution)
                    {
                        break;
                    }
                }

                //Verificando se a solução NotStandard também satisfaz a solução Standard
                if ((hasSolution) && (isFinish))
                {
                    for (int j = 1; j < inputStep.GetLength(1); j++)
                    {
                        if (!columnsDescr[j].Contains("A"))
                        {

                            if (inputStep[0, j] < 0)
                            {
                                hasSolution = false;
                                break;
                            }
                        }
                    }
                }

                if ((!hasSolution) && (isFinish))// Se ainda não tem solução, mas finalizou corretamente a etapa das variáveis artificiais então parte para a etapa do simplex padrão
                {
                    // Segunda etapa - verificando se ainda tem varivavel negativa em Z depois de Verificar W.
                    // Ajustado as colunas da matrix para rodar simplex padrão
                    // Ainda não terminou, tem que partir para uma nova iteração
                    int lines = inputStep.GetLength(0); // --> A partir deste ponto a linha W não eh mais necessária
                    int columns = inputStep.GetLength(1);

                    decimal[,] outputSted = new decimal[lines, columns];
                    outputSted = inputStep;
                    int newColumns = columns - columnsDescr.Count(x => x.Contains("A"));
                    int newLines = inputStep.GetLength(0) - 1; // --> A partir deste ponto a linha W não eh mais necessária

                    string[] newColumnsDescr = new String[newColumns];
                    String[] newLinesDescr = new String[newLines];
                    inputStep = new decimal[newLines, newColumns];
                    int nextColumn = 0;
                    for (int j = 0; j < columns; j++)
                    {
                        if (!columnsDescr[j].Contains("A"))
                        {
                            newColumnsDescr[nextColumn] = columnsDescr[j];
                            for (int i = 0; i < newLines; i++)
                            {
                                inputStep[i, nextColumn] = outputSted[i, j];
                            }

                            nextColumn++;
                        }
                    }

                    newLinesDescr[0] = "Z";
                    for (int i = 1; i < newLinesDescr.Length; i++)
                    {
                        newLinesDescr[i] = linesDescr[i];
                    }

                    linesDescr = newLinesDescr;
                    columnsDescr = newColumnsDescr;
                    //
                    isFinish = false;
                    iterarion = 0;
                    while ((!isFinish) || (iterarion > iterateLimit))
                    {
                        inputStep = Standard(inputStep, ref isFinish, ref columnsDescr, ref linesDescr, ref hasSolution);
                        iterarion++;
                        if (!hasSolution)
                        {
                            break;
                        }
                    }
                }
            }

            return inputStep;
            // Se lineBaseStep = 0 trata-se de simples padrão, e tem que verirficar se Z tem algum valor negativo
            // se lineBaseStep > 0 trata-se de simplex não padrão por variaveis artificiais, então verificar se W tem valor zero na coluna de valores (coluna zero). 
        }

        public decimal[,] NotStandard(decimal[,] inputStep, ref bool isFinish, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            int lines = inputStep.GetLength(0);
            int columns = inputStep.GetLength(1);

            int lineBaseStep = inputStep.GetLength(0) - 1;
            int valueColumnsIndex = 0;

            //decimal[,] outputSted = new decimal[,];
            decimal[,] outputSted = new decimal[lines, columns];

            outputSted = Iterate(lineBaseStep, inputStep, outputSted, ref columnsDescr, ref linesDescr, ref hasSolution);
            isFinish = false;

            //if ( (Math.Truncate(outputSted[lineBaseStep, valueColumnsIndex] * 1000000) >= 0) && (outputSted[lineBaseStep, valueColumnsIndex] < 1))
            decimal delteValue  =  System.Math.Round(Math.Abs(Math.Truncate(outputSted[lineBaseStep, valueColumnsIndex])) - Math.Abs(outputSted[lineBaseStep, valueColumnsIndex]) , 1);

            if ( (Math.Truncate(outputSted[lineBaseStep, valueColumnsIndex]) == 0) && (delteValue == 0) )
            {
                isFinish = true;
            }

            if ((isFinish) && (!hasSolution))
            {// A iteração já se iniciou com solução
                hasSolution = true;
            }
            return outputSted;
        }

        public decimal[,] Standard(decimal[,] inputStep, ref bool isFinish, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            int lines = inputStep.GetLength(0);
            int columns = inputStep.GetLength(1);

            int lineBaseStep = 0;

            // ? Quando for dual vai vir com  a  linha W, mesmo que com zeros
            decimal[,] outputSted = new decimal[lines, columns];

            // Ainda não terminou, tem que partir para uma nova iteração
            outputSted = Iterate(lineBaseStep, inputStep, outputSted, ref columnsDescr, ref linesDescr, ref hasSolution);

            isFinish = true;
            for (int j = 1; j < outputSted.GetLength(1); j++)
            {
                if (outputSted[lineBaseStep, j] < 0)
                {
                    isFinish = false;
                    break;
                }
            }

            if ((isFinish) && (!hasSolution))
            {// A iteração já se iniciou com solução
                hasSolution = true;
            }

            return outputSted;
        }

        public decimal[,] Iterate(int lineBaseStep, decimal[,] inputStep, decimal[,] outputSted, ref String[] columnsDescr, ref String[] linesDescr, ref bool hasSolution)
        {
            hasSolution = false;
            Int16 valueColumnsIndex = 0;
            int lines = inputStep.GetLength(0);
            int columns = inputStep.GetLength(1); //inputStep.GetLength(1) - 1;

            if (lines > 0)
            {
                // considera que a coluna [columns  = 0] esta reservada para o valores resultantes
                // Verificando qual é a colunas com valore mais negativo da linha base de fases (Z ou W)
                int pivotColumn = 0;
                decimal pivoltValue = 0;
                for (int j = 1; j < columns; j++)
                {
                    if ((inputStep[lineBaseStep, j] < 0) && (inputStep[lineBaseStep, j] < pivoltValue))
                    {
                        pivotColumn = j;
                        pivoltValue = inputStep[lineBaseStep, j];
                    }
                }

                if (pivotColumn != 0)
                {// Verificando a linha pivot
                    int pivotline = 0;
                    pivoltValue = 0;
                    bool isFristValue = true;
                    // A primeira linha eh Z e a ultima linha esta reservada para W, se for caso.
                    int limitPivotColumn = lineBaseStep == 0 ? lines : (lines - 1);

                    for (int i = 1; i < limitPivotColumn; i++)
                    {
                        decimal result = 0;
                        if (inputStep[i, pivotColumn] > 0)
                        {
                            result = inputStep[i, valueColumnsIndex] / inputStep[i, pivotColumn];
                        }
                        else
                        {
                            result = 0;
                        }
                        if (isFristValue)
                        {
                            if (result > 0)
                            {
                                pivotline = i;
                                pivoltValue = result;
                                isFristValue = false;
                            }
                        }
                        else
                        {
                            if ((result > 0) && (result < pivoltValue))
                            {
                                pivotline = i;
                                pivoltValue = result;
                            }
                        }
                    }

                    if (pivotline != 0)
                    {
                        // Nova Linha Base
                        outputSted[pivotline, valueColumnsIndex] = pivoltValue;
                        decimal baseValue = inputStep[pivotline, pivotColumn];

                        //definindo a variavel que esta entando na base
                        //outputSted[pivotline, 0] = pivotColumn;
                        linesDescr[pivotline] = columnsDescr[pivotColumn];

                        // Setando os novos valores da linha Base
                        for (int i = 1; i < columns; i++)
                        {
                            if (i != pivotColumn)
                            {
                                outputSted[pivotline, i] = inputStep[pivotline, i] / baseValue;
                            }
                            else
                            {
                                outputSted[pivotline, i] = 1;
                            }
                        }

                        // inverte o sinal das linhas da colunas base para melhor performace na proxima etapa
                        for (int i = 0; i < lines; i++)
                        {
                            inputStep[i, pivotColumn] = inputStep[i, pivotColumn] * -1;
                        }

                        // Novas linhas, alem da base
                        for (int i = 0; i < lines; i++)
                        {
                            if (i != pivotline)
                            {
                                for (int j = 0; j < columns; j++)
                                {
                                    if (j != pivotColumn)
                                    {
                                        outputSted[i, j] = outputSted[pivotline, j] * inputStep[i, pivotColumn] + inputStep[i, j];
                                    }
                                    else
                                    {
                                        outputSted[i, j] = 0;
                                    }
                                }
                            }
                        }
                        hasSolution = true;
                    }
                }

            }
            return hasSolution ? outputSted : inputStep;
        }
    }
}