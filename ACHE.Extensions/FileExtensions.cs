using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;
using System.IO;

namespace ACHE.Extensions
{
    public static class FileExtensions
    {
        public static void MoveFileWithReplace(this string sourceFileName, string destFileName)
        {

            //first, delete target file if exists, as File.Move() does not support overwrite
            if (File.Exists(destFileName))
                File.Delete(destFileName);

            File.Move(sourceFileName, destFileName);
        }

        public static void MoveDirectoryWithReplace(this string sourceFileName, string destFileName)
        {

            //first, delete target file if exists, as File.Move() does not support overwrite
            if (Directory.Exists(destFileName))
                Directory.Delete(destFileName);

            Directory.Move(sourceFileName, destFileName);
        }

        public static void ClearFolder(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

    }

    public class MIMEType
    {

        #region MIME type list
        private static readonly Dictionary<String, String> MimeTypeDict = new Dictionary<String, String>()
    {
            {     "bin", "application/octet-stream" },
            {     "zip", "application/x-zip-compressed" },
   
            {     "jpg", "image/jpeg" },
            {     "jpeg", "image/jpeg" },
            {     "png", "image/png" },
            {     "gif", "image/gif" },

            {     "mp4", "video/mp4" },
            {     "avi", "video/avi" },
            {     "mpeg", "video/mpeg" },
            
            {     "xls", "application/vnd.ms-excel" },
            {     "doc", "application/msword" },
            {     "ppt", "application/vnd.ms-powerpoint" },
            {     "pdf", "application/pdf" },
            {     "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            {     "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            {     "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" }
          
    };
        #endregion

        #region Get
        /// <summary>
        /// Returns the mime type for the requested file extension. Returns
        /// the default application/octet-stream if the extension is not found.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static String Get(String extension)
        {
            return Get(extension, MimeTypeDict["bin"]);
        }

        /// <summary>
        /// Returns the mime type for the requested file extension. Returns the
        /// specified defaultMimeType if the extension is not found.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="defaultMimeType"></param>
        /// <returns></returns>
        public static String Get(String extension, String defaultMimeType)
        {
            if (extension.StartsWith("."))
                extension = extension.Remove(0, 1);

            if (MimeTypeDict.ContainsKey(extension))
                return MimeTypeDict[extension];
            else
                return defaultMimeType;
        }
        #endregion Get

    }
}
