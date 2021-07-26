using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Windows;

using Simplex = Mercoplano.Simplex.Server.MvcUI.Business;

namespace MyTest
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            /*

            Int16 decisionLength = 3;
            Int16 restrictionLength = 3;
            decimal[,] inputStep = new decimal[4, 4];

            String[] restrictionSign = new String[] { "Z", ">=", "==", "<=" };
            String[] columnsDescr = new String[] { };
            String[] linesDescr = new String[] { };


            //Z
            inputStep[0, 0] = 0;
            inputStep[0, 1] = 3;
            inputStep[0, 2] = 2;
            inputStep[0, 3] = 1;

            //X4
            inputStep[1, 0] = 6;
            inputStep[1, 1] = 3;
            inputStep[1, 2] = 1;
            inputStep[1, 3] = 3;

            //X5
            inputStep[2, 0] = 6;
            inputStep[2, 1] = 3;
            inputStep[2, 2] = 2;
            inputStep[2, 3] = 0;

            //X6
            inputStep[3, 0] = 1;
            inputStep[3, 1] = 1;
            inputStep[3, 2] = -1;
            inputStep[3, 3] = 0;

            
            // Dual
            Int16 decisionLength = 3;
            Int16 restrictionLength = 3;
            decimal[,] inputStep = new decimal[4, 4];

            String[] restrictionSign = new String[] { "Z", "<=", "<=", "<=" };
            String[] columnsDescr = new String[] { };
            String[] linesDescr = new String[] { };

            inputStep[0, 0] = 0;
            inputStep[0, 1] = 200;
            inputStep[0, 2] = 250;
            inputStep[0, 3] = 120;

            //X4
            inputStep[1, 0] = 20;
            inputStep[1, 1] = 10;
            inputStep[1, 2] = 20;
            inputStep[1, 3] = 20;

            //X5
            inputStep[2, 0] = 30;
            inputStep[2, 1] = 10;
            inputStep[2, 2] = 20;
            inputStep[2, 3] = 40;

            //X6
            inputStep[3, 0] = 35;
            inputStep[3, 1] = 40;
            inputStep[3, 2] = 30;
            inputStep[3, 3] = 20;



            // Simplex.WMethod simpex = new Simplex.WMethod();
            Simplex.ZMethod simpex = new Simplex.ZMethod();

            //inicializando o valor objetivo
            inputStep[0,0] = 0;
            decimal[,] outPutSetp = simpex.InitialMatrix(decisionLength, restrictionLength, inputStep, restrictionSign, ref columnsDescr, ref linesDescr);

            **/

            List<decimal[]> inputSteps = new List<decimal[]>();

            String[] restrictionSign = new String[] { "Z", ">=", ">=", ">=" };
            String[] columnsDescr = new String[] { };
            String[] linesDescr = new String[] { };

            decimal[] inputStep = new decimal[4];

            inputSteps.Add(new decimal[] { 0, 20, 30, 35 });
            inputSteps.Add(new decimal[] { 200, 10, 10, 40 });
            inputSteps.Add(new decimal[] { 250, 20, 20, 30 });
            inputSteps.Add(new decimal[] { 120, 20, 40, 20 });


            Simplex.ZMethod simplex = new Simplex.ZMethod();
            decimal[,] outPutSetp = new decimal[,] { };
            //decimal[,] outPutSetp = simplex.StraightToSolution(inputSteps, restrictionSign, ref columnsDescr, ref linesDescr);


            Console.WriteLine(outPutSetp.GetLength(0));
            Console.WriteLine(outPutSetp.GetLength(1));
            Console.ReadLine();
        }
    }



}
