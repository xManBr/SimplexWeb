using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace Mercoplano.Simplex.Server.MvcUI.Models
{
    public class WindowInterval
    {
        public int Column { get; set; }
        public decimal Start { get; set; }
        public decimal End { get; set; }
    }

    public class FromLabelTo
    {
        public string FromLabel {get;set;}
        public string ToLabel {get;set;}
    }

    #region Simplex Model
    public class SimplexModel
    {
        public SimplexModel()
        {
            this.LineDescrs = new List<LineDescr>();
            this.ColumnsDescrs = new List<ColumnsDescr>();
            //this.RetrictionSigns = new List<RetrictionSign>();
            this.Equations = new List<Equation>();
            this.WindowIntervals = new List<WindowInterval>();
            this.RestrictionLimits = new List<RestrictionLimit>();
        }
        public int OriginalDecisionLength { get; set; }
        public string VariLabel { get; set; }
        public string Goal { get; set; }
        public string Method { get; set; }
        public bool StepByStep { get; set; }
        public string StepMethod { get; set; }
        public bool IsFinish { get; set; }
        public bool HasSolution { get; set; }
        public string goOut { get; set; }
        public string goOn { get; set; }

        public List<FromLabelTo> FromLabelTos { get; set; }
        public List<LineDescr> LineDescrs { get; set; }
        public List<ColumnsDescr> ColumnsDescrs { get; set; }
        //public List<RetrictionSign> RetrictionSigns { get; set; }
        public List<Equation> Equations { get; set; }
        public List<WindowInterval> WindowIntervals { get; set; }
        public List<RestrictionLimit> RestrictionLimits { get; set; }
    }
    public class Equation
    {
        public Equation()
        {

            this.Coefficients = new List<Coefficient>();
        }
        public string Line { get; set; }
        public string RetrictionSign { get; set; }
        public List<Coefficient> Coefficients { get; set; }
    }

    public class RestrictionLimit
    {
        public string Sign { get; set; }
        public decimal Number { get; set; }
    }

    public class Coefficient
    {
        [Required]
        [Display(Name = "*")]
        [Range(double.MinValue, double.MaxValue)]
        public decimal Number { get; set; }

    }
    #endregion
    public class LineDescr
    {
        public string Type { get; set; } // D-ecision, R-estriction or Z (Z Value)
        public string Label { get; set; }
    }

    public class ColumnsDescr
    {
        public string Type { get; set; } // D-ecision, R-estriction or Z (Z Value)
        public string DisplayName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }

    public class RetrictionSign
    {
        public string Sign { get; set; }
    }

    #region SimplexStartModel

    public class SimplexStartModel
    {
        public SimplexStartModel()
        {
            this.LineDescrs = new List<LineDescr>();
            this.ColumnsDescrs = new List<ColumnsDescr>();
            //this.RetrictionSigns = new List<RetrictionSign>();
            this.StartEquations = new List<StartEquation>();
        }

        public int OriginalDecisionLength { get; set; }
        public string VariLabel { get; set; }
        public string Goal { get; set; }
        public string Method { get; set; }
        public bool StepByStep { get; set; }
        public string StepMethod { get; set; }
        public bool IsFinish { get; set; }
        public bool HasSolution { get; set; }

        public List<LineDescr> LineDescrs { get; set; }
        public List<ColumnsDescr> ColumnsDescrs { get; set; }
        //public List<RetrictionSign> RetrictionSigns { get; set; }
        public List<StartEquation> StartEquations { get; set; }
    }

    public class StartEquation
    {
        public StartEquation()
        {

            this.StartCoefficients = new List<StartCoefficient>();
        }
        public string Line { get; set; }
        public string RetrictionSign { get; set; }
        public List<StartCoefficient> StartCoefficients { get; set; }
    }

    public class StartCoefficient
    {
        public string Number
        {
            get;
            set;
        }

    }

    #endregion
    public class InitialSimplexViewModel
    {
        [Required]
        [Display(Name = "Number Of Decision Variables")]
        [Range(2, 100)]
        public int DecisionVariables { get; set; }

        [Required]
        [Display(Name = "Quantity Restrictions")]
        [Range(2, 100)]
        public int RestrictionsVariables { get; set; }

    }
}

