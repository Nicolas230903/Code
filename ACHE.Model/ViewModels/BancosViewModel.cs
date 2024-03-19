using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for BancosViewModel
    /// </summary>
    public class BancosViewModel
    {
        public int ID { get; set; }
        public int IDBancoBase { get; set; }
        public string Nombre { get; set; }
        public string NroCuenta { get; set; }
        public string Moneda { get; set; }
        public string Activo { get; set; }
        public string SaldoInicial { get; set; }
        public string SaldoActual { get; set; }
        public string Ejecutivo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Observacion { get; set; }
    }

    public class ResultadosBancosViewModel
    {
        public IList<BancosViewModel> Items;
        public int TotalPage { get; set; }
        public int TotalItems { get; set; }
    }
}