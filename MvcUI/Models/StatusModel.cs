using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mercoplano.Simplex.Server.MvcUI.Models
{
    public class StatusModel
    {
        public String ObjectTypeId { get; set; }
        public String StatusId { get; set; }
        public String StatusName { get; set; }
        public Int32 TranslationId { get; set; }
        public String LastModifiedDate { get; set; }
        public String CreationDate { get; set; }
    }

}