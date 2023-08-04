using System;
using System.Data.OleDb;
using System.IO;
using System.Collections.Specialized;
using System.Text;

namespace Library
{
    /// <summary>
    /// LogFile Klasse
    /// </summary>
    /// 
    public class LogFile
    {
        //*********************************************************************
        // 
        /// <summary>
        /// Aktiviert und definiert den Excelmodus
        /// </summary>
        /// <param name="strarColTitle">Liste der Spaltenüberschriften</param>
        /// <param name="iArtCol">Spaltenindex der Art der Bemerkungszeile</param>
        /// <param name="iFirstBemCol">Spaltenindex der 1. Bemerkungsspalte</param>
        //public void DefineExcelMode(string[] strarColTitle, int iArtCol, int iFirstBemCol)
        //{
        //    m_bExcelMode = true;
        //    m_iArtCol = iArtCol;
        //    m_iFirstBemCol = iFirstBemCol;
        //    m_strarColTitle = strarColTitle;
        //}


        /// <summary>
        /// Erstellt ein LogFile unter dem angegebenen Filenamen 
        /// </summary>
        /// <param name="strLogFileName"></param>
        public void Open(string strLogFileName)
        {
            m_strErrorMessage = string.Empty;
            m_strLogFileName = strLogFileName;

            try
            {
                FileStream oFileStream = new FileStream(m_strLogFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                m_strwrLogFile = new StreamWriter(oFileStream, System.Text.Encoding.Default);
                m_strwrLogFile.AutoFlush = true;
            }
            catch (Exception exc)
            {
                // Fehler beim Öffnen der Files
                throw new Exception("Can't open logfile!", exc);
            }

            WriteHead();
        }

        /// <summary>
        /// Erstellt ein LogFile unter dem angegebenen Filenamen, ohne Header
        /// </summary>
        /// <param name="strLogFileName"></param>
        public void OpenWithoutHead(string strLogFileName)
        {
            m_strErrorMessage = string.Empty;
            m_strLogFileName = strLogFileName;

            try
            {
                FileStream oFileStream = new FileStream(m_strLogFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                m_strwrLogFile = new StreamWriter(oFileStream, System.Text.Encoding.Default);
                m_strwrLogFile.AutoFlush = true;
            }
            catch (Exception exc)
            {
                // Fehler beim Öffnen der Files
                throw new Exception("Can't open logfile!", exc);
            }
        }

        /// <summary>
        /// Bestehende Log-File öffnen und Meldungen anfügen
        /// </summary>
        /// <param name="strLogFileName">Name der Logdatei</param>
        public void OpenAppend(string strLogFileName)
        {
            m_strErrorMessage = string.Empty;
            m_strLogFileName = strLogFileName;

            try
            {
                //18.04.07 MSCHRECK:AppendModus mit ReadWrite führt zum Fehler, geändert in Write	
                //FileStream oFileStream = new FileStream(m_strLogFileName, FileMode.Append, FileAccess.ReadWrite, FileShare.Read); 
                FileStream oFileStream = new FileStream(m_strLogFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                m_strwrLogFile = new StreamWriter(oFileStream, System.Text.Encoding.Default);
                m_strwrLogFile.AutoFlush = true;
            }
            catch (Exception exc)
            {
                // Fehler beim Öffnen der Files
                throw new Exception("Can't open logfile!", exc);
            }

            WriteLine();
            WriteHead();
        }


        /// <summary>
        /// Schließt das LogFile
        /// </summary>
        public void Close()
        {
            if (m_strwrLogFile != null)
            {
                WriteTail();
                m_strwrLogFile.Close();
            }
        }

        /// <summary>
        /// Schließt das LogFile ohne Abschluss
        /// </summary>
        public void CloseWithoutTail()
        {
            if (m_strwrLogFile != null)
            {
                m_strwrLogFile.Close();
            }
        }


        /// <summary>
        /// WriteLine wie in der Klasse StreamWriter
        /// </summary>
        public void WriteLine()
        {
            WriteLine("");
        }

        /// <summary>
        /// WriteLine wie in der Klasse StreamWriter
        /// </summary>
        /// <param name="strString"></param>
        public void WriteLine(string strString)
        {
            if(LogAll)
            WriteLineIntern(ConvToColFormat(strString));
        }

        public void WriteLineFromException(string strString)
        {
                WriteLineIntern(ConvToColFormat(strString));
        }
        /// <summary>
        /// WriteLine wie in der Klasse StreamWriter
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="oObject"></param>
        public void WriteLine(string strString, object oObject)
        {
            if (LogAll)
                WriteLine(string.Format(strString, oObject));
        }


        /// <summary>
        /// Gibt die übergebenen Strings aus 
        /// </summary>
        /// <param name="strarString"></param>
        public void WriteLine(string[] strarString)
        {
            if (LogAll)
                foreach (string strMsg in strarString)
            {
                WriteLine(strMsg);
            }

        }

        public void WriteLine(string strString, params object[] args)
        {
            if (LogAll)
                WriteLine(string.Format(strString, args));
        }

        /// <summary>
        /// WriteLine wie in der Klasse StreamWriter
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="oObject1"></param>
        /// <param name="oObject2"></param>
        public void WriteLine(string strString, object oObject1, Object oObject2)
        {
            if (LogAll)
                WriteLine(string.Format(strString, oObject1, oObject2));
        }

        //*********************************************************************
        // Spalten-Zeile ausgeben
        private string ConvToColFormat(string strLine)
        {
            if (!m_bExcelMode)
            {
                return strLine;
            }

            // Konvertierung
            string strNewLine = "";
            for (int i = 0; i < m_strarColTitle.Length; i++)
            {
                strNewLine += (i > 0) ? ";" : "";
                strNewLine += (i == m_iArtCol) ? "\"Log\"" : "";
                strNewLine += (i == m_iFirstBemCol) ? "\"" + strLine + "\"" : "";
            }
            return strNewLine;
        }

        /// <summary>
        /// Spalten-Zeile ausgeben  (Art und Bemerkung werden einzeln übergeben)
        /// </summary>
        /// <param name="strArt">Bemerkungsart</param>
        /// <param name="Bemerkung">Bemerkungsinformation</param>
        //public void WriteColLine(string strArt, string Bemerkung)
        //{
        //    string strNewLine = "";
        //    for (int i = 0; i < m_strarColTitle.Length; i++)
        //    {
        //        strNewLine += (i > 0) ? ";" : "";
        //        strNewLine += (i == m_iArtCol) ? "\"" + strArt + "\"" : "";
        //        strNewLine += (i == m_iFirstBemCol) ? "\"" + Bemerkung + "\"" : "";
        //    }
        //    WriteLineIntern(strNewLine);
        //}

        /// <summary>
        /// Spalten-Zeile ausgeben (Spaltentrenner ist '|')
        /// </summary>
        /// <param name="strLine">Zeile</param>
        public void WriteColLine(string strLine)
        {
            string strNewLine = "\"" + strLine + "\"";
            strNewLine = strNewLine.Replace("|", "\";\"");
            WriteLineIntern(strNewLine);
        }


        //*********************************************************************
        // Interne WriteLine.Fkt. zum Schreiben von Zeilen in das Logfile
        private void WriteLineIntern(string strLine)
        {
            if (m_strwrLogFile == null)
                throw new InvalidOperationException("Logfile is closed");

            if (!m_bExcelMode && m_bPrependCurrentTime)
            {
                strLine = string.Format("{0} {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:ffff"), strLine);
            }

            if (!m_bExcelMode && !m_bPrependCurrentTime)
            {
                strLine = string.Format("{0} {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss:ffff"), strLine);
            }

            m_strwrLogFile.WriteLine(strLine);

            if (m_bWriteToConsole)
            {
                Console.WriteLine(strLine);
            }
            if (m_bWriteToTextBuffer)
            {
                m_strcolTextBuffer.Add(strLine);
            }

            System.Diagnostics.Debug.WriteLine(strLine);
        }

        //*********************************************************************
        // Spaltenüberschriften ausgaben
        private void WriteExcelHeader()
        {
            string strColHeader = "";
            foreach (string strTitle in m_strarColTitle)
            {
                strColHeader += strTitle + "|";
            }
            strColHeader = strColHeader.Substring(0, strColHeader.Length - 1);
            WriteColLine(strColHeader);
        }


        /// <summary>
        /// Schreibt den Anfangssatz des Logfiles (Zeitstempel)
        /// </summary>
        private void WriteHead()
        {
            if (m_bExcelMode)
            {
                WriteExcelHeader();
            }

            if (m_bWriteHeadAndTail)
            {
                WriteLine("************************************************************");
            }
        }

        /// <summary>
        /// Schreibt den Abschlusssatz des Logfiles (Zeitstempel, Statistik)
        /// </summary>
        public void WriteTail()
        {
            if (m_bWriteHeadAndTail)
            {
                end = DateTime.Now;
                dauer = end - start;
                WriteLine("************************************************************");
                WriteLine("Dauer der Laufzeit: {0}", dauer);
                WriteLine("************************************************************");
            }
        }

        //*********************************************************************
        // Properties

        /// <summary>
        /// Liefert den Stream der Log-Datei zurück
        /// </summary>
        public StreamWriter Stream
        {
            get { return m_strwrLogFile; }
        }

        /// <summary>
        /// Liefert die aufgetretene  Fehlermeldung zurück
        /// </summary>
        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        /// <summary>
        /// Setzt die Propertie, ob die Log-Meldungen zusätzlich auf die Console geschrieben werden
        /// </summary>
        public bool WriteToConsole
        {
            get { return m_bWriteToConsole; }
            set { m_bWriteToConsole = value; }
        }

        /// <summary>
        /// Setzt die Propertie, ob in der Datei die Kopf und Fußzeile geschrieben wird
        /// </summary>
        public bool WriteHeadAndTail
        {
            get { return m_bWriteHeadAndTail; }
            set { m_bWriteHeadAndTail = value; }
        }

        /// <summary>
        /// Setzt die Propertie, ob die Log-Meldungen zusätzlich in einem Textbuffer gespeichert werden
        /// </summary>
        public bool WriteToTextBuffer
        {
            get { return m_bWriteToTextBuffer; }
            set { m_bWriteToTextBuffer = value; }
        }

        /// <summary>
        /// Legt fest, ob vor dem Eintrag der aktuelle Timestamp ausgegeben wird.
        /// </summary>
        public bool PrependCurrentTime
        {
            get { return m_bPrependCurrentTime; }
            set { m_bPrependCurrentTime = value; }
        }

        /// <summary>
        /// Liefert den Textbuffer
        /// </summary>
        public StringCollection GetTextBuffer
        {
            get { return m_strcolTextBuffer; }
        }

        /// <summary>
        /// Liefert den Textbuffer als Text mit Zeichenumbrüchen
        /// </summary>
        /// <returns></returns>
        public String GetTextBufferText()
        {
            StringBuilder oSBLog = new StringBuilder();
            foreach (string strLine in m_strcolTextBuffer)
            {
                oSBLog.AppendLine(strLine);
            }

            return oSBLog.ToString();
        }

        /// <summary>
        /// Ausgabe der Exception in das LogFile
        /// </summary>
        /// <param name="oExc"></param>
        public void LogException(Exception oExc)
        {

            WriteLineFromException("Exception-Log:");
            WriteLineFromException("==============");

            Exception oCurExc = oExc;
            while (oCurExc != null)
            {
                WriteLineFromException("Message: " + oCurExc.Message);
                if (oCurExc is OleDbException)
                {
                    OleDbException dbExc = (OleDbException)oCurExc;
                    WriteLineFromException("ErrorCode: " + dbExc.ErrorCode);
                    foreach (OleDbError err in dbExc.Errors)
                    {
                        WriteLineFromException("Error:" + err.Message + ", NativeError: " + err.NativeError.ToString() + ", Source: " + err.Source + ", SQLState: " + err.SQLState);
                    }
                }
                WriteLineFromException("Stack: " + oCurExc.StackTrace);

                oCurExc = oCurExc.InnerException;
            }
            WriteLineFromException("==============");
        }


        /// <summary>
        /// 
        /// </summary>
        public string LogFileName
        {
            get { return m_strLogFileName; }
        }


        //*********************************************************************
        // Variablen
        private string m_strLogFileName;
        private string m_strErrorMessage;
        private StreamWriter m_strwrLogFile = null;
        DateTime start = DateTime.Now;
        DateTime end;
        TimeSpan dauer;

        // Excel Mode: Darstellung des Log-Files in Spalten
        private bool m_bExcelMode = false;
        private int m_iArtCol;				// Spalte, in der "Log" ausgegeben wird
        private int m_iFirstBemCol;			// Spalte, in der der Text ausgegeben wird
        private string[] m_strarColTitle;	// Spaltenüberschriften

        // Properties:
        private bool m_bWriteHeadAndTail = true;
        private bool m_bWriteToConsole = true;
        private bool m_bWriteToTextBuffer = false;
        private bool m_bPrependCurrentTime = false;
        public bool LogAll { get; set; }
    
        private StringCollection m_strcolTextBuffer = new StringCollection();
    }
}
