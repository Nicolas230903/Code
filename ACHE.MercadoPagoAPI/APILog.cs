using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace ACHE.MercadoPago
{
	class APILog {
		static string APP_FILENAME = ConfigurationManager.AppSettings["ApiError"].Replace("XX", DateTime.Now.ToString("yyyyMMdd"));

		public static void AppErrorToFile(string msj, string detalle) {
			var texto = string.Format("Fecha: {0} \r\nError: {1}\r\nDetalle: {2}\r\n^^-------------------------------------------------------------------^^\r\n", DateTime.Now, msj, detalle);
			File.AppendAllText(APP_FILENAME, texto, Encoding.GetEncoding(1252));
		}
	}

}
