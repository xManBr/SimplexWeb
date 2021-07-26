using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mercoplano.Simplex.Server.MvcUI.Entity;

using Mercoplano.Simplex.Server.MvcUI.Models;
using Mercoplano.Simplex.Server.MvcUI.Controllers;
using System.Data.Entity.Core.Objects;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class TranslationDate
    {
        private SimplexEntities db = new SimplexEntities();

        public DateTime Get()
        {
            ObjectResult<Nullable<DateTime>> dates = db.TranslationDateSelect();

            var myDates = dates.ToList();
            if (myDates.Count != 0)
            {
                return (DateTime)myDates.First();
            }
            else
            {
                return DateTime.MinValue;
            }          
        }
    }
}