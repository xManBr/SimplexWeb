using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mercoplano.Simplex.Server.MvcUI.Controllers;

namespace Mercoplano.Simplex.Server.MvcUI.Business
{
    public class Util
    {
        public static String AjustDecimalSign(string variable)
        {
            if ((variable.Contains(".") && System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != "."))
            {
                variable = variable.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            else if ((variable.Contains(",") && System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != ","))
            {
                variable = variable.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            }
            return variable;
        }

        public static decimal CoefficientNumber(string variable, Int16 languageId)
        {
            decimal value = 0;
            if ((variable == null) || (variable.Trim() == String.Empty))
            {
                value = 0;
            }
            else
            {
                try
                {
                    string[] variableArr = variable.Split('/').ToArray();
                    if (variableArr.Length == 1)
                    {
                        value = Convert.ToDecimal(AjustDecimalSign(variableArr[0]));
                    }
                    else if ((variableArr.Length == 2) && (Convert.ToDecimal(variableArr[1]) > 0))
                    {
                        value = Convert.ToDecimal(AjustDecimalSign(variableArr[0])) / Convert.ToDecimal(AjustDecimalSign(variableArr[1]));
                    }
                    else
                    {
                        throw new Exception(BaseMvc.GetLabel("InvalidValueEnter", languageId));
                    }
                }
                catch
                {
                    throw new Exception(BaseMvc.GetLabel("InvalidValueEnter", languageId));
                }
            }
            return value;
        }

    }
}