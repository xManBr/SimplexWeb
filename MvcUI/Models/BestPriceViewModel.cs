using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Mercoplano.Simplex.Server.MvcUI.Models
{
    public class VariableIdentityModel
    {
        public VariableIdentityModel()
        {
            this.LineDescrs = new List<LineDescr>();
        }
        public List<LineDescr> LineDescrs { get; set; }
    }

    public class InitialBestPriceViewModel
    {
        [Required]
        [Display(Name = "Number Of Decision Variables")]
        [Range(2, 100)]
        public int DecisionVariables { get; set; }

        public int RestrictionsVariables { get; set; }

    }

}