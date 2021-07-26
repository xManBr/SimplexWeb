using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mercoplano.Simplex.Server.MvcUI.Controllers
{
    public class Config //: Mercoplano.LogisticsOperation.Server.Itinerary.BusinessEntity.Config
    {
        #region Business Const
        public const string CATEGORY_CODE_DRIVER = "MOT";
        #endregion
        public const Int32 UPLOAD_FORMAT_DEFAULT_LIMIT = 2;
        public const Int32 FILE_TYPE_EXCELL_ID = 1;
        #region Compatible

        public const String TRUE = "true";
        public const String TRUE_CHEKBOX_FORMCOLLECTION = "true,false";
        public const String TRUE_RADIOBOX_FORMCOLLECTION = "this";
        #endregion
        internal const Int16 VERSION_RULE_ID_ACTIVED = 1; // ~Versão de classes de regras ativa - isso poderá ser determinado plo próprio usário [IMPLEMENTAÇÃO FUTURA]
        public static String LABEL_NO_TRANSLATION = "NO TRANSLATION";

        #region Recurrent Label
        internal const Int32 LOGISTICS_TITULO_LABEL_ID = 12; // Logistica
        internal const Int32 INSTITUITION_TITULO_LABEL_ID = 30;// Instituition
        internal const Int32 SECTOR_TITULO_LABEL_ID = 19;// Instituition
        internal const Int32 YES_LABEL_ID = 40; //Sim
        internal const Int32 NO_LABEL_ID = 41;//Não
        internal const Int32 SEND_LABEL_ID = 6;//Enviar
        internal const Int32 STATUS_LABEL_ID = 13; // Status
        internal const Int32 SELECTION_LABEL_ID = 11;// Id Label Selecione
        internal const Int32 DESCRIPTION_LABEL_ID = 20; // Descrição
        internal const Int32 CODE_LABEL_ID = 21;//Código
        internal const Int32 MILEAGE_LABEL_ID = 22;// Quilomegtragem
        internal const Int32 BOX_LABEL_ID = 23;// Box
        internal const Int32 COMPOSITION_LABEL_ID = 34;// Compopsição
        internal const Int32 REGION_LABEL_ID = 35;// Region
        internal const Int32 ROUTE_CODE_LABEL_ID = 37;// Código do Roteiro/ Itinerario / Composição
        internal const Int32 VEHICLE_LABEL_ID = 39;// Veículo - Categoria de Meio de Transporte
        internal const Int32 MSG_OPERATION_COMPLETED_SUCCESSFULLY = 33;
        internal const Int32 UNSUCCESSFUL_LABEL_ID = 76;// sem sucesso
        internal const Int32 CREATE_LABEL_ID = 77; // Create
        internal const Int32 SEARCH_LABEL_ID = 80; // Search
        internal const Int32 NAME_LABEL_ID = 2;//Name
        #endregion

        #region Recurrent Message
        internal const int MSG_OPTION_NOT_SELECTED_LABEL_ID = 28;
        internal const int MSG_INFORMATION_INVALID_LABEL_ID = 17;
        #endregion

        // A partir daqui entra config copiado de LogisticsLoad

        /*CTRL SHIFT U*/

        public static String ACCOUNT_ACTIVATE_URL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_ACTIVATE_URL"].ToString();
            }
        }

        public static Int32 ACCOUNT_ACTIVATE_EMAIL_TEMPLATEID
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_ACTIVATE_EMAIL_TEMPLATEID"].ToString());
            }
        }

        public static String RESET_PASSWORD_URL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RESET_PASSWORD_URL"].ToString();
            }
        }

        public static Int32 RESET_PASSWORD_EMAIL_TEMPLATEID
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RESET_PASSWORD_EMAIL_TEMPLATEID"].ToString());
            }
        }

        #region Keys
        public static String EMAIL_AUTHENTICATIONINFO
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EMAIL_AUTHENTICATIONINFO"].ToString();
            }
        }
        public static Boolean EMAIL_STORED_FIRST
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EMAIL_STORED_FIRST"]);
            }
        }
        public static String EMAIL_SYSTEM_DEFAULT
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EMAIL_SYSTEM_DEFAULT"].ToString();
            }

        }

        #endregion

        #region Suport
        public const String TYPE_OF_REGISTER_DEFAULT = "LOAD";
        #endregion

        public static Int16 LANGUAGEID
        {
            get
            {
                return Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["LANGUAGE_DEFAULT_ID"].ToString());
            }
        }

        #region Global Default
        public const Int32 TRANSPORTER_ID_DEFALUT = 2;
        public const Int32 PROVIDER_ID_DEFALUT = 0;

        public const Char SeparatorDefault = '^';
        public const Int16 USERDEFAULTCOUNTRYID = 3;
        public const String DATEPICKERFORMAT = "dd/mm/yy";
        public const String DATEFORMAT = "dd/MM/yyyy";
        public const String CULTURE = "en";
        public const String DEFAULT_LABELCODE_LANGUAGE = "en";
        #endregion

        #region Status DEF
        public const String STATUSID_ACT = "ACT";
        public const String STATUSID_TMP = "TMP"; //Temporário
        public const String OBJECTTYPEID_DEF = "DEF";
        public const String DEFAULT_ROOT_PATH = "~/";
        #endregion

        #region XML Roots
        public const String TRANSLATIONS_XML_ROOT = "Translations";
        #endregion

        #region Compatible

        //public const String TRUE = "true";
        //public const String TRUE_CHEKBOX_FORMCOLLECTION = "true,false";
        //public const String TRUE_RADIOBOX_FORMCOLLECTION = "this";

        #endregion
    }
}

