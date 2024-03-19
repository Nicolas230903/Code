using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;

namespace ACHE.Extensions
{
    public static class StringExtensions
    {

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
        
        
        public static string RemoverAcentos(this string textoOriginal)
        {
            return textoOriginal.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")
                .Replace("Ç", "C").Replace("ç", "c").Replace("Ñ", "N").Replace("ñ", "n").Replace('"', ' ').Trim();
                
                //.Replace("\"", "\\\"");
        }
        private static readonly Regex cleanWhitespace = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);

        public static bool IsValidEmailAddress(this string s)
        {
            return new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(s);
            //Regex rx = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$");
            //return rx.IsMatch(s);
        }

        public static bool IsValidUrl(this string text)
        {
            Regex rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return rx.IsMatch(text);
        }

        public static string CleanHtmlTags(this string s)
        {
            Regex exp = new Regex("<[^<>]*>", RegexOptions.Compiled);

            return exp.Replace(s, "");
        }

        public static bool ContainsWords(this string phrase, string[] words)
        {
            //The staring returnValue is false, but changes if all words are found
            bool returnValue = false;
            //Placeholder for the amount of words found
            int wordsFound = 0;
            //Loop through all of the words we are trying to find
            for (int w = 0; w < words.Length; w++)
            {
                //Loop through all of the words in the phrase
                for (int i = 0; i < phrase.Split(' ').Length; i++)
                {
                    //Get the current word in the phrase
                    String word = phrase.Split(' ')[i];
                    //If the current word is in our list of words to find
                    if (word == words[w])
                        //Add 1 to the wordsFound count
                        wordsFound++;
                }
            }
            //If all of the words are found
            if (wordsFound == words.Length)
                //Set the returnValue to true
                returnValue = true;

            //Return true or false depending on how many words were found
            return returnValue;
        }

        //todo: (nheskew) rename to something more generic (CleanAttributeALittle?) because not everything needs
        // the cleaning power of CleanAttribute (everything should but AntiXss.HtmlAttributeEncode encodes 
        // *everyting* incl. white space :|) so attributes can get really long...but then my only current worry is around
        // the description meta tag. Attributes from untrusted sources *do* need the current CleanAttribute...
        public static string CleanWhitespace(this string s)
        {
            return cleanWhitespace.Replace(s, " ");
        }

        public static string RemoveLineBreaks(this string lines)
        {
            return lines.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
        }

        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return lines.Replace("\r\n", replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement)
                        .Replace("\t", replacement)
                        .Trim();
        }

        public static string Truncate(this string text, int maxLength, string suffix)
        {
            // replaces the truncated string to a ...
            string truncatedString = text;

            if (maxLength <= 0) return truncatedString;
            int strLength = maxLength - suffix.Length;

            if (strLength <= 0) return truncatedString;

            if (text == null || text.Length <= maxLength) return truncatedString;

            truncatedString = text.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;
            return truncatedString;
        }

        public static bool IsNotNullOrEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }

        public static bool IsStrongPassword(this string s)
        {
            bool isStrong = Regex.IsMatch(s, @"[\d]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[a-z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[A-Z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[\s~!@#\$%\^&\*\(\)\{\}\|\[\]\\:;'?,.`+=<>\/]");
            if (isStrong) isStrong = s.Length > 7;
            return isStrong;
        }

        public static string ToProperCase(this string text)
        {
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text);
        }

        public static string ToDescription(this Enum en) //ext method
        {

            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {

                object[] attrs = memInfo[0].GetCustomAttributes(
                typeof(DescriptionAttribute),

                false);

                if (attrs != null && attrs.Length > 0)

                    return ((DescriptionAttribute)attrs[0]).Description;

            }

            return en.ToString();

        }

        public static string ReplaceAll(this string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string[] Split(this string input, string pattern)
        {
            return Regex.Split(input, pattern);
        }

        public static string[] Split(this string input, string pattern, System.Text.RegularExpressions.RegexOptions options)
        {
            return Regex.Split(input, pattern, options);
        }

        public static string Join(this string[] input)
        {
            return string.Join("", input);
        }

        public static string Join(this string[] input, string seperator)
        {
            return string.Join(seperator, input);
        }

        public static string MakeNotNull(this string input)
        {
            if (input == null)
                return string.Empty;
            return input;
        }

		public static string GenerateRandom(this string input, int lenght) {

            string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz/////*****";
			
			Random random = new Random();
			StringBuilder buffer = new StringBuilder(lenght);
			for (int i = 0; i < lenght; i++) {
				int randomNumber = random.Next(0, _characters.Length);
				buffer.Append(_characters, randomNumber, 1);
			}
			return buffer.ToString();
		}

        public static string GenerateRandomPassword(this string input)
        {

            string _charactersMayus = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string _charactersMinus = "abcdefghijklmnopqrstuvwxyz";
            string _charactersNumeros = "0123456789";

            Random random = new Random();
            StringBuilder buffer = new StringBuilder(8);

            int randomNumber = random.Next(0, _charactersMinus.Length);
            buffer.Append(_charactersMinus, randomNumber, 1);

            randomNumber = random.Next(0, _charactersMayus.Length);
            buffer.Append(_charactersMayus, randomNumber, 1);

            randomNumber = random.Next(0, _charactersMayus.Length);
            buffer.Append(_charactersMayus, randomNumber, 1);

            randomNumber = random.Next(0, _charactersMinus.Length);
            buffer.Append(_charactersMinus, randomNumber, 1);

            randomNumber = random.Next(0, _charactersNumeros.Length);
            buffer.Append(_charactersNumeros, randomNumber, 1);

            randomNumber = random.Next(0, _charactersMayus.Length);
            buffer.Append(_charactersMayus, randomNumber, 1);

            randomNumber = random.Next(0, _charactersNumeros.Length);
            buffer.Append(_charactersNumeros, randomNumber, 1);

            randomNumber = random.Next(0, _charactersMinus.Length);
            buffer.Append(_charactersMinus, randomNumber, 1);

            return buffer.ToString();
        }

        public static string EncryptToSHA1(this string str)
        {
            string rethash = "";
            try
            {

                System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                byte[] combined = encoder.GetBytes(str);
                hash.ComputeHash(combined);
                rethash = Convert.ToBase64String(hash.Hash);
            }
            catch (Exception ex)
            {
                string strerr = "Error in HashCode : " + ex.Message;
            }
            return rethash;
        }

        public static bool ExtensionIsDangerous(this string extension)
        {
            return (extension.Equals(".REG") || extension.Equals(".DOCM") || extension.Equals(".DOTM") || extension.Equals(".XLSM")
                || extension.Equals(".XLTM") || extension.Equals(".XLAM") || extension.Equals(".PPTM") || extension.Equals(".POTM")
                || extension.Equals(".PPAM") || extension.Equals(".PPSM") || extension.Equals(".SLDM") || extension.Equals(".PS1")
                || extension.Equals(".PS1XML") || extension.Equals(".PS2") || extension.Equals(".PS2XML") || extension.Equals(".PSC1")
                || extension.Equals(".PSC2") || extension.Equals(".INF") || extension.Equals(".MSH") || extension.Equals(".MSH1")
                
                || extension.Equals(".PSC2") || extension.Equals(".INF") || extension.Equals(".MSH") || extension.Equals(".MSH1")
                || extension.Equals(".MSH2") || extension.Equals(".MSHXML") || extension.Equals(".MSH1XML") || extension.Equals(".MSH2XML")
                || extension.Equals(".WS") || extension.Equals(".WSF") || extension.Equals(".WSC") || extension.Equals(".WSH")

                || extension.Equals(".JS") || extension.Equals(".JSE") || extension.Equals(".VBE") || extension.Equals(".VB")
                || extension.Equals(".WS") || extension.Equals(".WSF") || extension.Equals(".WSC") || extension.Equals(".WSH")
                || extension.Equals(".VBS") || extension.Equals(".CMD") || extension.Equals(".BAT") || extension.Equals(".HTA")
                || extension.Equals(".EXE") || extension.Equals(".COM") || extension.Equals(".MSI") || extension.Equals(".APPLICATION")

                || extension.Equals(".PIF") || extension.Equals(".JAR") || extension.Equals(".CPL") || extension.Equals(".SCR")
                || extension.Equals(".MSP") || extension.Equals(".ASPX") || extension.Equals(".ASHX") || extension.Equals(".CS")
                );
        }

        public static bool IsValidCUIT(this string cuit)
        {
            if (cuit == null)
            {
                return false;
            }

            cuit = cuit.Replace("-", string.Empty);
            if (cuit.Length != 11)
            {
                return false;
            }
            else
            {
                int calculado = CalcularDigitoCuil(cuit);
                int digito = int.Parse(cuit.Substring(10));
                return calculado == digito;
            }
        }

        //public static bool IsValidPassword(this string password)
        //{
        //    if (password == null)
        //    {
        //        return false;
        //    }
        //    if (Regex.Replace(password, "/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/", "") == string.Empty)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        private static int CalcularDigitoCuil(string cuil)
        {
            int[] mult = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            char[] nums = cuil.ToCharArray();
            int total = 0;
            for (int i = 0; i < mult.Length; i++)
            {
                total += int.Parse(nums[i].ToString()) * mult[i];
            }
            var resto = total % 11;
            return resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;
        }

        public static string RemoverCaracteresParaPDF(this string textoOriginal)
        {
            return textoOriginal.RemoverAcentos().ReplaceAll(" ", "_").Replace(",", "").Replace("\"", "").Replace("/", "").Replace(":", "")
                                                 .Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("&", "").Trim();
            // \/ : * ? " <> |
        }
        public static bool ExtensionIsOK(this string extension)
        {
            if ((((!extension.Equals(".PDF") && !extension.Equals(".DOC")) && (!extension.Equals(".DOCX") && !extension.Equals(".RTF"))) && ((!extension.Equals(".JPG") && !extension.Equals(".JPEG")) && (!extension.Equals(".PNG") && !extension.Equals(".GIF")))) && !extension.Equals(".XLS"))
            {
                return extension.Equals(".XLSX");
            }
            return true;
        }


    }
}
