using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for BancosViewModel
    /// REGINFO_CV_CABECERA
    /// </summary>
    public class CitiComprasViewModel
    {
        public int ID { get; set; }
        public string CUITInformante { get; set; }
        public string Periodo { get; set; }
        public string Secuencia { get; set; }
        public string SinMovimiento { get; set; }
        public string ProrratearCreditoFiscalComputable { get; set; }
        public string CreditoFiscalComputableGlobalOPorComprobante { get; set; }

        public string ImporteCreditoFiscal_ComputableGlobal { get; set; }
        public string ImporteCreditoFiscal_ComputableConAsignacionDirecta { get; set; }
        public string ImporteCreditoFiscal_ComputableDeterminadoPorProrrateo { get; set; }
        public string ImporteCreditoFiscal_NoComputableGlobal { get; set; }
    }

    public class ResultadosCitiComprasViewModel
    {
        public IList<CitiComprasViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}