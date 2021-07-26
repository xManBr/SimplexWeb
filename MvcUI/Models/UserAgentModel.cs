using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Mercoplano.Simplex.Server.MvcUI;
using Mercoplano.Simplex.Server.MvcUI.Models;

namespace Mercoplano.Simplex.Server.MvcUI.Models
{
    [Serializable]
    public class UserAgentModel : BaseModel
    {
        public UserAgentModel()
        {
            this.LanguageModel = new LanguageModel();
        }
        public int UserAgentId
        {
            get;
            set;
        }

        public Int32 ProviderId { get; set; }
        public Int32 TransporterId { get; set; }
        public String Culture { get; set; }
        //public String Culture { get; set; }
        [Required(ErrorMessage = "*")]
        public Int16 UserDefaultCountryId { get; set; }

        [Required(ErrorMessage = "*")]
        public Int16 DatePickerFormatId { get; set; }
        public String DatePickerFormat { get; set; }

        [Required(ErrorMessage = "*")]
        public String DateFormat { get; set; }

        [Required(ErrorMessage = "*")]
        public Int16 UserDecimalSeparetorId { get; set; }
        public String UserDecimalSeparetor { get; set; }

        [Required(ErrorMessage = "*")]
        public String Code
        {
            get;
            set;
        }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public String Password
        {
            get;
            set;
        }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public String PasswordConfirmation
        {
            get;
            set;
        }

        [Required(ErrorMessage = "*")]
        public Int16 LanguageId
        {
            get;
            set;
        }

        public LanguageModel LanguageModel { get; set; }

        [Required(ErrorMessage = "*")]
        public String Name
        {
            get;
            set;
        }

        [DataType(DataType.EmailAddress)]
        public String Email
        {
            get;
            set;
        }


        public String ActivityKey
        {
            get;
            set;
        }

        public String Level
        {   // Nivel de acesso, define a hierarquida de poderes do usuário
            // Nivel Zero (0) significa que o usuário não tem direito a acessar nada pe peculiar no sistema.
            get;
            set;
        }

    }
}