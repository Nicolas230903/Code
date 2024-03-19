using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACHE.Extensions;

namespace ACHE.Model
{
    public class CommonModel
    {
        public static void GenerarArchivo(DataTable dt, string path, string fileName)
        {
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add(fileName);
            //ws.Cell("A1").InsertData(dt.AsEnumerable());

            //Establezco el header
            int index = 1;
            foreach (var dc in dt.Columns)
            {
                ws.Cell(1, index).Value = dt.Columns[index - 1].Caption.SplitCamelCase();
                ws.Cell(1, index).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#4F81BD");// .FromTheme(ClosedXML.Excel.XLThemeColor.Accent1);
                ws.Cell(1, index).Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                index++;
            }

            //Inserto los resultados
            ws.Cell(2, 1).Value = dt.AsEnumerable();
            ws.RangeUsed().SetAutoFilter();

            //wb.Worksheets.Add(dt, fileName);
            ws.Columns().AdjustToContents();
            wb.SaveAs(path + "_" + DateTime.Now.ToString("yyymmdd") + ".xlsx");//HHmmss
        }

        public static void GenerarArchivoSinFecha(DataTable dt, string path, string fileName)
        {
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add(fileName);
            //ws.Cell("A1").InsertData(dt.AsEnumerable());

            //Establezco el header
            int index = 1;
            foreach (var dc in dt.Columns)
            {
                ws.Cell(1, index).Value = dt.Columns[index - 1].Caption.SplitCamelCase();
                ws.Cell(1, index).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#4F81BD");// .FromTheme(ClosedXML.Excel.XLThemeColor.Accent1);
                ws.Cell(1, index).Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                index++;
            }

            //Inserto los resultados
            ws.Cell(2, 1).Value = dt.AsEnumerable();
            ws.RangeUsed().SetAutoFilter();

            //wb.Worksheets.Add(dt, fileName);
            ws.Columns().AdjustToContents();
            wb.SaveAs(path + ".xlsx");
        }
        public static List<Combo2ViewModel> ObtenerProvincias()
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    return dbContext.Provincias.ToList()
                           .Select(x => new Combo2ViewModel()
                           {
                               ID = x.IDProvincia,
                               Nombre = x.Nombre
                           }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<Combo2ViewModel> obtenerCiudades(int idProvincia)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    return dbContext.vCiudades.Where(x => x.IDProvincia == idProvincia).OrderBy(x => x.Nombre)
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.IDCiudad,
                            Nombre = x.Nombre
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }     


        public static ResultadosCombo2ViewModel obtenerCiudadesPaginadas(int page, int pageSize)
        {
            try
            {
                using (var dbContext = new ACHEEntities())
                {
                    ResultadosCombo2ViewModel resultado = new ResultadosCombo2ViewModel();
                    page--;
                    var list = dbContext.Ciudades.OrderBy(x => x.Nombre).Skip(page * pageSize).Take(pageSize).ToList()
                        .Select(x => new Combo2ViewModel()
                        {
                            ID = x.IDCiudad,
                            Nombre = x.Nombre
                        }).ToList();

                    resultado.TotalPage = ((list.Count() - 1) / pageSize) + 1;
                    resultado.TotalItems = list.Count();
                    resultado.Items = list.ToList();
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }        
    }
}