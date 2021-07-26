using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mercoplano.Simplex.Server.MvcUI.Models
{
    [Serializable()]
    public class LanguageModel
    {
        public LanguageModel()
        {
            this.StatusModel = new StatusModel();
        }
        public Int16 Id { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }

        public StatusModel StatusModel { get; set; }
    }
}