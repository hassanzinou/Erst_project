using System;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Library;
using System.Data.Odbc;


namespace Library
{
    /// <summary>
    /// Delegate zum Parsen von Texten
    /// </summary>
    public delegate string Parser(string txt);

    /// <summary>
    /// Sammlung verschiedener Tools
    /// </summary>
    public class Tools
    {
        
        /// <summary>
        /// ‹berpr¸ft, ab der String ein g¸ltiges Datum im Format YYYYMMDD / YYYY-MM-DD ist
        /// </summary>
        /// <param name="strDate">Der zu pr¸fende String.</param>
        /// <returns>Wahr falls das Format stimmt, sonst falsch.</returns>
        public static bool IsDateYYYYMMDD(string strDate)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            DateTime dummy;
            return DateTime.TryParseExact(fRemoveDateSeparators(strDate), "yyyyMMdd", format, DateTimeStyles.AllowWhiteSpaces, out dummy);
        }

        /// <summary>
        /// Lˆscht die Zeichen "/", "-" und "." aus einem String.
        /// </summary>
        /// <param name="strDate">Ein String, aus dem gelˆscht werden soll.</param>
        /// <returns>Der String ohne die Zeichen "/", "-" und ".".</returns>
        private static string fRemoveDateSeparators(string strDate)
        {
            return strDate
                .Replace("/", "")
                .Replace("-", "")
                .Replace(".", "");
        }

        /// <summary>
        /// Liefert ein DateTime-Object anhand des Strings im Format YYYYMMDD
        /// </summary>
        /// <param name="strDate">Der String mit dem Datum.</param>
        /// <returns>Das aus dem String geparste DateTime-Objekt.</returns>
        /// <exception>Wirft eine Exception, wenn das Datum nicht im Format yyyyMMdd ist.</exception>
        public static DateTime ParseDateYYYYMMDD(string strDate)
        {
            strDate = strDate.Replace("/", "");
            strDate = strDate.Replace("-", "");
            strDate = strDate.Replace(".", "");
            DateTime dattiDate; ;

            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                dattiDate = DateTime.ParseExact(strDate.Substring(0,8), "yyyyMMdd", format);
                return dattiDate;
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid Date Format (" + strDate + ")!", oExc);
            }
        }

        /// <summary>
        /// Liefert ein DateTime-Object anhand des Strings im Format YYYYMMDDhhmmss
        /// </summary>
        /// <param name="strDate">Der String mit dem Datum.</param>
        /// <returns>Das aus dem String geparste DateTime-Objekt.</returns>
        /// <exception>Wirft eine Exception, wenn das Datum nicht im Format yyyyMMddhhmmss ist.</exception>
        public static DateTime ParseDateYYYYMMhhmmss(string strDate)
        {
            strDate = strDate.Replace("/", "");
            strDate = strDate.Replace("-", "");
            strDate = strDate.Replace(".", "");
            DateTime dattiDate; ;

            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                dattiDate = DateTime.ParseExact(strDate, "yyyyMMddhhmmss", format);
                return dattiDate;
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid Date Format (" + strDate + ")!", oExc);
            }
        }


        /// <summary>
        /// Liefert ein DateTime-Object anhand des Strings im Format YYMMDD
        /// </summary>
        /// <param name="strDate">Der String mit dem Datum.</param>
        /// <returns>Das aus dem String geparste DateTime-Objekt.</returns>
        /// <exception>Wirft eine Exception, wenn das Datum nicht im Format yyMMdd ist.</exception>
        public static DateTime ParseDateYYMMDD(string strDate)
        {
            strDate = strDate.Replace("-", "");
            strDate = strDate.Replace(".", "");
            DateTime dattiDate = DateTime.MinValue;
            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                dattiDate = DateTime.ParseExact(strDate, "yyMMdd", format);
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid Date Format (" + strDate + ")!", oExc);
            }
            return dattiDate;
        }


        /// <summary>
        /// ‹berpr¸ft, ab der String ein g¸ltiges Datum im Format dd.MM.yyyy oder ddMMyyyy ist
        /// </summary>
        /// <param name="strDate">Der zu pr¸fende String.</param>
        /// <returns>Wahr falls das Format stimmt, sonst falsch.</returns>
        public static bool IsDateDDMMYYYY(string strDate)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            DateTime dummy;
            return DateTime.TryParseExact(fRemoveDateSeparators(strDate), "ddMMyyyy", format, DateTimeStyles.AllowWhiteSpaces, out dummy);
        }

        /// <summary>
        /// Liefert ein DateTime-Object anhand des Strings im Format dd.MM.yyyy oder ddMMyyyy
        /// </summary>
        /// <param name="strDate">Der String mit dem Datum.</param>
        /// <returns>Das aus dem String geparste DateTime-Objekt.</returns>
        /// <exception>Wirft eine Exception, wenn das Datum nicht im Format ddMMyyyy ist.</exception>
        public static DateTime ParseDateDDMMYYYY(string strDate)
        {
            strDate = strDate.Replace(".", "");
            strDate = strDate.Replace("-", "");
            strDate = strDate.Replace("/", "");
            strDate = strDate.Substring(0, 8);
            DateTime dattiDate = DateTime.MinValue;
            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                dattiDate = DateTime.ParseExact(strDate, "ddMMyyyy", format);
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid Date Format! '" + strDate + "'", oExc);
            }
            return dattiDate;
        }


        /// <summary>
        /// ‹berpr¸ft, ab der String ein g¸ltiges Datum im Format d.M.yyyy ist
        /// </summary>
        /// <param name="strDate">Der zu pr¸fende String.</param>
        /// <returns>Wahr falls das Format stimmt, sonst falsch.</returns>
        public static bool IsDateDMYYYY(string strDate)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("de-DE", true);
            DateTime dummy;
            return DateTime.TryParse(strDate, format, DateTimeStyles.AllowWhiteSpaces, out dummy);
        }

        /// <summary>
        /// Liefert ein DateTime-Object anhand des Strings im Format d.M.yyyy
        /// </summary>
        /// <param name="strDate">Der String mit dem Datum.</param>
        /// <returns>Das aus dem String geparste DateTime-Objekt.</returns>
        /// <exception>Wirft eine Exception, wenn das Datum nicht im Format ddMMyyyy ist.</exception>
        public static DateTime ParseDateDMYYYY(string strDate)
        {
            DateTime dattiDate = DateTime.MinValue;
            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("de-DE", true);
                dattiDate = DateTime.Parse(strDate, format);
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid Date Format! '" + strDate + "'", oExc);
            }
            return dattiDate;
        }


        /// <summary>
        /// ‹berpr¸ft, ab der String ein g¸ltiges Datum im Format dd.MM.yy oder ddMMyy ist
        /// </summary>
        /// <param name="strDate">Der zu pr¸fende String.</param>
        /// <returns>Wahr falls das Format stimmt, sonst falsch.</returns>
        public static bool IsDateDDMMYY(string strDate)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            DateTime dummy;
            return DateTime.TryParseExact(fRemoveDateSeparators(strDate), "ddMMyy", format, DateTimeStyles.AllowWhiteSpaces, out dummy);
        }

        /// <summary>
        /// ‹berpr¸ft, ab der String ein g¸ltiges Datum im Format yy.MM.dd oder yyMMdd ist
        /// </summary>
        /// <param name="strDate">Der zu pr¸fende String.</param>
        /// <returns>Wahr falls das Format stimmt, sonst falsch.</returns>
        public static bool IsDateYYMMDD(string strDate)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
            DateTime dummy;
            return DateTime.TryParseExact(fRemoveDateSeparators(strDate), "yyMMdd", format, DateTimeStyles.AllowWhiteSpaces, out dummy);
        }

        /// <summary>
        /// Liefert ein DateTime-Object anhand des Strings im Format dd.MM.yy oder ddMMyy
        /// </summary>
        /// <param name="strDate">Der String mit dem Datum.</param>
        /// <returns>Das aus dem String geparste DateTime-Objekt.</returns>
        /// <exception>Wirft eine Exception, wenn das Datum nicht im Format ddMMyyyy ist.</exception>
        public static DateTime ParseDateDDMMYY(string strDate)
        {
            DateTime dattiDate = DateTime.MinValue;
            strDate = strDate.Replace(".", "");
            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                dattiDate = DateTime.ParseExact(strDate, "ddMMyy", format);
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid Date Format!", oExc);
            }
            return dattiDate;
        }

        public static string ParseStringPreisInCent(int preis)
        {
            string cpreis = "";

            try
            {
                cpreis = ParseDecimalPreisInCent(preis).ToString();
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid INT Format!", oExc);
            }
            return cpreis;
        }

        public static decimal ParseDecimalPreisInCent(int preis)
        {
            decimal cpreis = 0;

            try
            {
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                cpreis = Decimal.Parse(preis.ToString(),format)/100;
            }
            catch (Exception oExc)
            {
                throw new Exception("invalid INT Format!", oExc);
            }
            return cpreis;
        }

        /// <summary>
        /// Konvertiert den ¸bergebene String von OEM nach ANSI
        /// </summary>
        /// <param name="strText">Der zu konvertierende Text.</param>
        /// <returns>Der konvertierte Text.</returns>
        public static string ConvertOemToAnsi(string strText)
        {
            StringBuilder strBuilder = new StringBuilder(strText);
            char[] ansi_umlaute = { '÷', 'ƒ', '‹', 'ˆ', '‰', '¸', 'ﬂ' }; // ÷, ƒ, ‹, ˆ, ‰, ¸, ﬂ
            char[] oem_umlaute = { 'ô', 'é', 'ö', 'î', 'Ñ', 'Å', '·' }; // ô, é, ö, î, Ñ, Å, ·

            int i, j;
            char a;

            for (i = 0; i < strBuilder.Length; i++)
            {
                a = strBuilder[i];
                for (j = 0; j <= 6; j++)
                    if (a == oem_umlaute[j])
                        strBuilder[i] = ansi_umlaute[j];
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Konvertieren eines numerischen Strings in gepackte Decimal-Darstellung
        /// </summary>
        /// <param name="strSource">Der zu konvertierende String.</param>
        /// <returns>Der String in gepackter Dezimal-Darstellung.</returns>
        public static char[] NumStrToPackedDec(string strSource)
        {
            ArrayList arrliPackedChares = new ArrayList();

            byte bytPOSITIVE = 0x0c;
            byte bytNEGATIVE = 0x0d;
            char chaPLUS_SIGN = '+';
            char chaMINUS_SIGN = '-';
            char chaSign = chaPLUS_SIGN;	/* default sign */

            int iSIndex;                        /* source index */

            /* validate source and target lengths */

            if (strSource.Length < 1 || strSource.Length > 32)
            {
                throw new Exception("packing string has invalid length");
            }

            /* get packing - last digit out of the way first */
            byte bytLastByte = (byte)strSource[strSource.Length - 1];
            bytLastByte = (byte)(bytLastByte << (byte)4);
            arrliPackedChares.Insert(0, (char)bytLastByte);

            if (strSource.Length > 1)
            {
                iSIndex = strSource.Length - 2;
                while (iSIndex >= 0)
                {
                    if ((iSIndex == 0) &&
                        (strSource[0] == chaPLUS_SIGN || strSource[0] == chaMINUS_SIGN))
                    {
                        chaSign = strSource[iSIndex];	/* save sign */
                        break;					        /* force end of loop */
                    }
                    else
                    {
                        byte bytByte = (byte)(strSource[iSIndex] & 0x0f);	/* right-hand nibble */
                        iSIndex--;
                        if (iSIndex >= 0)
                        {
                            bytByte |= (byte)(strSource[iSIndex] << (byte)4); /* left-hand nibble */
                            iSIndex--;
                        }
                        arrliPackedChares.Insert(0, (char)bytByte);
                    }
                }
            }

            /* put sign on target */
            if (chaSign == chaPLUS_SIGN)
            {
                arrliPackedChares[arrliPackedChares.Count - 1] = (char)(((char)arrliPackedChares[arrliPackedChares.Count - 1]) | bytPOSITIVE);
            }
            else
            {
                arrliPackedChares[arrliPackedChares.Count - 1] = (char)(((char)arrliPackedChares[arrliPackedChares.Count - 1]) | bytNEGATIVE);
            }

            // ByteArray in char-Array ¸bertragen
            char[] chaarPackedBytes = new Char[arrliPackedChares.Count];
            for (int i = 0; i < arrliPackedChares.Count; i++)
            {
                char chaByte = (char)arrliPackedChares[i];
                chaarPackedBytes[i] = chaByte;
            }
            return chaarPackedBytes;
        }



        /// <summary>
        /// Entschl¸sseln einer Datei per GnuPG.
        /// </summary>
        /// <param name="strSourceFile">Die zu entschl¸sselnde Datei.</param>
        /// <param name="strTargetFile">Die Datei f¸r das entschl¸sselte Ergebnis.</param>
        /// <param name="strPassword">Das Passwort zum entschl¸sseln.</param>
        /// <exception>Erzeugt einen allgemeinen Ausnahmefehler.</exception>
        static public void GnuPgDecrypt(string strGPGPfad, string strSourceFile, string strTargetFile, string strPassword)
        {
            try
            {
                if (!strGPGPfad.EndsWith("\\"))
                {
                    strGPGPfad += "\\";
                }
                FileInfo filinFile = new FileInfo(strSourceFile);
                string strArg = "--output {0} --batch --passphrase-fd 0 --decrypt {1} ";	// 0=Target-File 1=User-ID 2=Source-File
                strArg = string.Format(strArg, strTargetFile, strSourceFile);

                ProcessStartInfo oProcessStartInfo = new ProcessStartInfo(strGPGPfad + "gpg.exe", strArg);
                oProcessStartInfo.WorkingDirectory = filinFile.DirectoryName;
                oProcessStartInfo.CreateNoWindow = true;
                oProcessStartInfo.UseShellExecute = false;
                oProcessStartInfo.RedirectStandardInput = true;
                oProcessStartInfo.RedirectStandardOutput = true;
                oProcessStartInfo.RedirectStandardError = true;
                Process oProcess = Process.Start(oProcessStartInfo);

                // Send pass phrase, if any
                oProcess.StandardInput.WriteLine(strPassword);
                oProcess.StandardInput.Flush();

                oProcess.WaitForExit();

                // Check results and prepare output
                if (oProcess.ExitCode != 0)
                {
                    throw new Exception("GPG-Decrypt-Error: [" + oProcess.ExitCode.ToString() + "]");
                }
            }
            catch (Exception oExc)
            {
                throw new Exception("Error during decrytion with gnuPG", oExc);
            }
        }


        /// <summary>
        /// Verschl¸sseln einer Datei per GnuPG.
        /// </summary>
        /// <param name="strGPGPfad">Der GnuPG-Pfad.</param>
        /// <param name="strSourceFile">Die zu verschl¸sselnde Datei.</param>
        /// <param name="strUserId">Die ID des Empf‰ngers.</param>
        /// <param name="strTargetFile">Die Datei f¸r das verschl¸sselte Ergebnis.</param>
        static public void GnuPgEncrypt(string strGPGPfad, string strSourceFile, string strUserId, string strTargetFile)
        {

            string[] userId = new string[] { strUserId };

            GnuPgEncrypt(strGPGPfad, strSourceFile, userId, strTargetFile);

        }

        /// <summary>
        /// Verschl¸sseln einer Datei per GnuPG.
        /// </summary>
        /// <param name="strGPGPfad">Der GnuPG-Pfad.</param>
        /// <param name="strSourceFile">Die zu verschl¸sselnde Datei.</param>
        /// <param name="userIds">Ein Array von Ids der Empf‰nger.</param>
        /// <param name="strTargetFile">Die Datei f¸r das verschl¸sselte Ergebnis.</param>
        /// <exception>Erzeugt einen allgemeinen Ausnahmefehler.</exception>
        static public void GnuPgEncrypt(string strGPGPfad, string strSourceFile, string[] userIds, string strTargetFile)
        {
            if (userIds.Length <= 0)
            {
                throw new Exception("Error during decrytion with gnuPG! No Key is specified!");
            }

            try
            {
                if (!strGPGPfad.EndsWith("\\"))
                {
                    strGPGPfad += "\\";
                }
                FileInfo filinFile = new FileInfo(strSourceFile);

                string recipient = " --recipient ";
                string recipientList = string.Empty;

                foreach (string userId in userIds)
                {
                    recipientList += recipient + "\"" + userId + "\"";
                }

                string strArg = " --output \"{0}\" --batch --encrypt {1} \"{2}\" ";	// 0=Target-File 1=User-ID 2=Source-File
                strArg = string.Format(strArg, strTargetFile, recipientList, strSourceFile);

                ProcessStartInfo oProcessStartInfo = new ProcessStartInfo(strGPGPfad + "gpg.exe", strArg);
                oProcessStartInfo.WorkingDirectory = filinFile.DirectoryName;
                oProcessStartInfo.CreateNoWindow = true;
                oProcessStartInfo.UseShellExecute = false;
                oProcessStartInfo.RedirectStandardInput = true;
                oProcessStartInfo.RedirectStandardOutput = true;
                oProcessStartInfo.RedirectStandardError = true;
                Process oProcess = Process.Start(oProcessStartInfo);
                oProcess.WaitForExit();

                // Check results and prepare output
                if (oProcess.ExitCode != 0)
                {
                    throw new Exception("GPG-Encrypt-Error: [" + oProcess.ExitCode.ToString() + "] ('" + strGPGPfad + "gpg.exe" + strArg + "')");
                }

            }
            catch (Exception oExc)
            {
                throw new Exception("Error during decrytion with gnuPG", oExc);
            }
        }

        /// <summary>
        /// Ersetzt eine Formatzeichenfolge durch ein Datum.
        /// YYYYMMDD, YYMMDD, MMDDYYYY, MMDDYY, KWxx_YYYY, KW-1_YYYY, _KWxx_, _KW-1_
        /// </summary>
        /// <param name="strFile">Zeichenfolge mit Datumsplazhalter.</param>
        /// <returns>Die Zeichenfolge mit dem aktuellen Datum.</returns>
        public static string GetDateString(string strFile)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtToday = DateTime.Today;
            CultureInfo ciDE = new CultureInfo("de-DE");
            Calendar calToday = ciDE.Calendar;
            CalendarWeekRule cwrToday = ciDE.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dowToday = ciDE.DateTimeFormat.FirstDayOfWeek;

            strFile = strFile.Replace("YYYYMMDDHHMMSS", dtNow.ToString("yyyyMMddHHmmss"));
            strFile = strFile.Replace("YYYYMMDD-HHMMSS", dtNow.ToString("yyyyMMdd-HHmmss"));
            strFile = strFile.Replace("YYYYMMDD_HHMMSS", dtNow.ToString("yyyyMMdd_HHmmss"));
            strFile = strFile.Replace("YYYYMMDD-HHMM", dtNow.ToString("yyyyMMdd-HHmm"));
            strFile = strFile.Replace("YYYYMMDD_HHMM", dtNow.ToString("yyyyMMdd_HHmm"));
            strFile = strFile.Replace("YYYYMMDDHHMM", dtNow.ToString("yyyyMMddHHmm"));
            strFile = strFile.Replace("YYYYMMDD", dtNow.ToString("yyyyMMdd"));
            strFile = strFile.Replace("YYMMDD_HHMMSS", dtNow.ToString("yyMMdd_HHmmss"));
            strFile = strFile.Replace("YYMMDD.HHMM", dtNow.ToString("yyMMdd.HHmm"));
            strFile = strFile.Replace("YYMMDD", dtNow.ToString("yyMMdd"));
            strFile = strFile.Replace("DDMMYYYY-HHMMSS", dtNow.ToString("ddMMyyyy-HHmmss"));
            strFile = strFile.Replace("DDMMYYYY_HHMMSS", dtNow.ToString("ddMMyyyy_HHmmss"));
            strFile = strFile.Replace("DDMMYYYYHHMMSS", dtNow.ToString("ddMMyyyyHHmmss"));
            strFile = strFile.Replace("DDMMYYYY-HHMM", dtNow.ToString("ddMMyyyy-HHmm"));
            strFile = strFile.Replace("DDMMYYYY_HHMM", dtNow.ToString("ddMMyyyy_HHmm"));
            strFile = strFile.Replace("DDMMYYYYHHMM", dtNow.ToString("ddMMyyyyHHmm"));
            strFile = strFile.Replace("DDMMYYYY", dtNow.ToString("ddMMyyyy"));
            strFile = strFile.Replace("DDMMYY", dtNow.ToString("ddMMyy"));

            strFile = strFile.Replace("KWxx_YYYY", "KW" + calToday.GetWeekOfYear(dtToday, cwrToday, dowToday).ToString("00") + "_" + dtToday.ToString("yyyy"));
            strFile = strFile.Replace("KW-1_YYYY", "KW" + calToday.GetWeekOfYear(dtToday.AddDays(-7), cwrToday, dowToday).ToString("00") + "_" + dtToday.ToString("yyyy"));
            strFile = strFile.Replace("_KWxx_", "_KW" + calToday.GetWeekOfYear(dtToday, cwrToday, dowToday).ToString("00") + "_");
            strFile = strFile.Replace("_KW-1_", "_KW" + calToday.GetWeekOfYear(dtToday.AddDays(-7), cwrToday, dowToday).ToString("00") + "_");

            return strFile;
        }

        /// <summary>
        /// Liefert f¸r ein deutsches Datum die Kalenderwoche
        /// </summary>
        /// <param name="dattiDate">Das Datum.</param>
        /// <returns>Die Kalenderwoche.</returns>
        public static int GetWeekOfYearDE(DateTime dattiDate)
        {
            CultureInfo ciDE = new CultureInfo("de-DE");
            Calendar calToday = ciDE.Calendar;
            CalendarWeekRule cwrToday = ciDE.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dowToday = ciDE.DateTimeFormat.FirstDayOfWeek;
            return calToday.GetWeekOfYear(dattiDate, cwrToday, dowToday);
        }

        /// <summary>
        /// Pr¸ft mit einem regul‰ren Ausdruck, ob der String eine Emailadresse ist.
        /// </summary>
        /// <param name="email">er zu pr¸fende String.</param>
        /// <returns>Wahr wenn es sich um eine Emailadresse handelt, sonst falsch.</returns>
        static public bool IsEmail(string email)
        {
            email = email.ToLower();

            string pattern = @"^((([a-z]|[0-9]|!|#|$|%|&|'|\*|\+|\-|/|=|\?|\^|_|`|\{|\||\}|~)+(\.([a-z]|[0-9]|!|#|$|%|&|'|\*|\+|\-|/|=|\?|\^|_|`|\{|\||\}|~)+)*)@((((([a-z]|[0-9])([a-z]|[0-9]|\-){0,61}([a-z]|[0-9])\.))*([a-z]|[0-9])([a-z]|[0-9]|\-){0,61}([a-z]|[0-9])\.(af|ax|al|dz|as|ad|ao|ai|aq|ag|ar|am|aw|au|at|az|bs|bh|bd|bb|by|be|bz|bj|bm|bt|bo|ba|bw|bv|br|io|bn|bg|bf|bi|kh|cm|ca|cv|ky|cf|td|cl|cn|cx|cc|co|km|cg|cd|ck|cr|ci|hr|cu|cy|cz|dk|dj|dm|do|ec|eg|eu|sv|gq|er|ee|et|fk|fo|fj|fi|fr|gf|pf|tf|ga|gm|ge|de|gh|gi|gr|gl|gd|gp|gu|gt|gg|gn|gw|gy|ht|hm|va|hn|hk|hu|is|in|id|ir|iq|ie|im|il|it|jm|jp|je|jo|kz|ke|ki|kp|kr|kw|kg|la|lv|lb|ls|lr|ly|li|lt|lu|mo|me|mk|mg|mw|my|mv|ml|mt|mh|mq|mr|mu|yt|mx|fm|md|mc|mn|ms|ma|mz|mm|na|nr|np|nl|an|nc|nz|ni|ne|ng|nu|nf|mp|no|om|pk|pw|ps|pa|pg|py|pe|ph|pn|pl|pt|pr|qa|re|ro|ru|rw|sh|kn|lc|pm|vc|ws|sm|st|sa|sn|cs|sc|sl|sg|sk|si|sb|so|za|gs|es|lk|sd|sr|sj|sz|se|ch|sy|tw|tj|tz|th|tl|tg|tk|to|tt|tn|tr|tm|tc|tv|ug|ua|ae|gb|uk|us|um|uy|uz|vu|ve|vn|vg|vi|wf|eh|ye|zm|zw|com|edu|gov|int|mil|net|org|biz|info|name|pro|aero|coop|museum|arpa))|(((([0-9]){1,3}\.){3}([0-9]){1,3}))|(\[((([0-9]){1,3}\.){3}([0-9]){1,3})\])))$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
            //if (!regex.IsMatch(email)) throw new Exception("The emailaddress: " + email + " seems to be invalid!");

            return regex.IsMatch(email);
        }

        /// <summary>
        /// Pr¸ft on eine Emailadresse g¸ltig ist.
        /// </summary>
        /// <param name="email">Die Emailadresse.</param>
        /// <returns>Wahr, wenn die Adresse dem regul‰ren Ausdruck entspricht und die Dom‰ne gepingt werden kann. Sonst falsch.</returns>
        public static bool IsValidEmail(string email)
        {
            bool ValidMail = true;

            if (IsEmail(email) == false)
            {
                ValidMail = false;
            }

            return ValidMail;
        }


        /// <summary>
        /// Wandelt einen decimal-Wert in einen String f¸r die Verwendung im Ikaros-Importformat
        /// </summary>
        /// <param name="value">decimal Wert der gewandelt werden soll</param>
        /// <returns>Der String im Ikaros-Importformat.</returns>
        public static string DecimalToIkarosStr(decimal value)
        {
            System.IFormatProvider iFormat = new System.Globalization.CultureInfo("en-US", true);
            return (value == decimal.MinValue) ? "" : value.ToString("F4", iFormat);
        }

        public static string DecimalToIkarosStr(double value)
        {
            System.IFormatProvider iFormat = new System.Globalization.CultureInfo("en-US", true);
            return (value == double.MinValue) ? "" : value.ToString("F4", iFormat);
        }
    }
}
