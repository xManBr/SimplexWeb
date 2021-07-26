using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Mercoplano.Simplex.Server.MvcUI.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            this.StatusModel = new StatusModel();
        }
        public StatusModel StatusModel { get; set; }
    }
}