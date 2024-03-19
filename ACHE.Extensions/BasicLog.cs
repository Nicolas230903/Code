using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace ACHE.Extensions
{
    public static class BasicLog
    {
        public static void AppendToFile(string fileName, string msj, string detalle)
        {
            var texto = string.Format("Fecha: {0} \r\nError: {1}\r\nDetalle: {2}\r\n^^-------------------------------------------------------------------^^\r\n", DateTime.Now, msj, detalle);
            fileName = fileName.Replace("XX", DateTime.Now.ToString("yyyy-MM-dd"));
            File.AppendAllText(fileName, texto, Encoding.GetEncoding(1252));
        }
    }

}
