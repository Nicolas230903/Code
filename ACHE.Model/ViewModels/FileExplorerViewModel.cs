using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACHE.Model
{
    /// <summary>
    /// Summary description for FileExplorerViewModel
    /// </summary>
    public class FileExplorerViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string FechaAlta { get; set; }
        public string Imagen { get; set; }
        public string Icono { get; set; }
        public string Tipo { get; set; }
        public string Path { get; set; }
        public int Items { get; set; }
    }

    public class ResultadosFileExplorerViewModel
    {
        public IList<FileExplorerViewModel> Files;
        public IList<FileExplorerViewModel> Folders;
        public string PathNavigation { get; set; }
        public int TotalItems
        {
            get { return (this.Files.Count + this.Folders.Count); }
        }
    }
}