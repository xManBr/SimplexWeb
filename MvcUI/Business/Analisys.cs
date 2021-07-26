using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mercoplano.Simplex.Server.MvcUI.Models;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class Analisys
    {
        public static WindowInterval Getintervals(string[,] intervals, int column)
        {
            List<decimal> inOrder = new List<decimal>();
            for (int i = 1; i < intervals.GetLength(1); i++)
            {
                // Pegar os valores mais proximos de zero.
                // intervals[i,column];
                if (intervals[i, column] != "null")
                {
                    inOrder.Add(Convert.ToDecimal(intervals[i, column]));
                }
            }

            inOrder = inOrder.OrderByDescending(x => x).ToList();

            WindowInterval windowInterval = new WindowInterval();

            windowInterval.Column = column;
            windowInterval.Start = inOrder.FirstOrDefault(x => x < 0);
            windowInterval.End = inOrder.FirstOrDefault(x => x > 0);

            return windowInterval;
        }

        public static List<WindowInterval> AllowedWindow(decimal[,] outputSetps, String[] columnsDescr, String[] linesDescr, int originalDecisionLength)
        {
            // Pega a partir da primeira sobra
            int lines = outputSetps.GetLength(1);
            int columns = (outputSetps.GetLength(1) - originalDecisionLength);
            string[,] intervals = new string[lines, columns];
            List<WindowInterval> windowInterval = new List<WindowInterval>();
            for (int j = originalDecisionLength + 1; j < outputSetps.GetLength(1); j++)
            {
                if (!(columnsDescr[j].Contains("A")))
                {// Não se aplica para eventuais colunas de variáves artificiais
                    for (int i = 1; i < outputSetps.GetLength(0); i++)
                    {
                        if (outputSetps[i, j] != 0)
                        {
                            intervals[i, j - originalDecisionLength] = Convert.ToString(outputSetps[i, 0] / outputSetps[i, j]);
                        }
                        else
                        {
                            intervals[i, j - originalDecisionLength] = "null";
                        }
                    }
                    windowInterval.Add(Getintervals(intervals, j - originalDecisionLength));
                }
            }
            return windowInterval;
        }
    }
}