using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections;
using System.IO;


namespace Library
{
    public class IniFile
    {
        ///// <summary>
        ///// Ermöglicht die Auswahl des gewünschten INI-Files
        ///// </summary>
        //public string SelectAndRead(IWin32Window iOwner)
        //{
        //    return SelectAndRead(iOwner, false);
        //}


        ///// <summary>
        ///// Ermöglicht die Auswahl des gewünschten INI-Files
        ///// Falls nur eine INI-Datei existiert ist ggf. eine automatische Auswahl möglich
        ///// </summary>
        //public string SelectAndRead(IWin32Window iOwner, bool bOneIniAutomatic)
        //{
        //    // INI-Dateien ermitteln
        //    string[] strarIniFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ini");
        //    IniFileSelectDlg oDlg = new IniFileSelectDlg();
        //    foreach (string strIniFileName in strarIniFiles)
        //    {
        //        FileInfo oFileInfo = new FileInfo(strIniFileName);
        //        oDlg.IniFileListBox.Items.Add(oFileInfo.Name);
        //    }

        //    // Falls nur eine INI-Datei vorhanden ist, ggf. diese INI-Datei automatisch auswählen
        //    if (oDlg.IniFileListBox.Items.Count == 1 && bOneIniAutomatic)
        //    {
        //        // Einziges INI-File einlesen
        //        Read(oDlg.IniFileListBox.Items[0].ToString());
        //        return oDlg.IniFileListBox.Items[0].ToString();
        //    }

        //    // User auswählen lassen
        //    if (DialogResult.OK == oDlg.ShowDialog(iOwner) && oDlg.IniFileListBox.SelectedItem != null)
        //    {
        //        // Ausgewähltes INI-File einlesen
        //        Read(oDlg.IniFileListBox.SelectedItem.ToString());
        //        return oDlg.IniFileListBox.SelectedItem.ToString();
        //    }
        //    else
        //    {
        //        throw new Exception("No Ini-File selected!");
        //    }
        //}
        /// <summary>
        /// Liest die Daten des INI-Files ein
        /// </summary>
        /// <param name="strFileName"></param>
        public void Read(string strFileName)
        {
            m_strIniFileName = strFileName;
            m_oSectionHashtable = new Hashtable();
            m_strcoSection = new StringCollection();
            m_iEntryListCount = 1;
            try
            {
                ReadIntern(strFileName);
            }
            catch (Exception oExc)
            {
                throw new Exception("Error during reading INI-File!", oExc);
            }
        }

        /// <summary>
        /// Liest die Daten des INI-Files aus dem übergenenen String ein
        /// </summary>
        /// <param name="strContent">Inhalt der Datei als String</param>
        public void ReadString(string strContent)
        {
            m_strIniFileName = "CONTENT";
            m_oSectionHashtable = new Hashtable();
            m_strcoSection = new StringCollection();
            m_iEntryListCount = 1;

            try
            {
                using (TextReader oTextReader = new StringReader(strContent))
                {
                    ReadInternFromStreamReader(oTextReader);
                }
            }
            catch (Exception oExc)
            {
                throw new Exception("Error during reading INI-File from string!", oExc);
            }

        }

        private void ReadInternFromStreamReader(TextReader oTextReader)
        {
            // Zeilen einlesen und analysieren
            Hashtable oCurEntryHashtable = null;
            string strCurSection = "";
            while (oTextReader.Peek() != -1)
            {
                string strLine = oTextReader.ReadLine();
                strLine = strLine.Trim();

                // Leerzeilen überspringen
                if (strLine == string.Empty)
                {
                    continue;
                }

                // Kommentarzeilen überspringen
                if (strLine[0] == ';')
                {
                    continue;
                }

                // Auf Include überprüfen
                if (strLine.IndexOf("@Include ") == 0)
                {
                    try
                    {
                        string strIncludeFile = strLine.Substring(9);
                        ReadIntern(strIncludeFile);
                        continue;
                    }
                    catch (Exception oExc)
                    {
                        throw new Exception("Error in Include-Rownumber (" + strLine + ")!", oExc);
                    }
                }

                // Auf Section überprüfen
                if (strLine[0] == '[' && strLine[strLine.Length - 1] == ']')
                {
                    // Section bestimmen
                    string strSectionOriginalCase = strLine.Substring(1, strLine.Length - 2);
                    string strSection = strSectionOriginalCase.ToLower();
                    if (!m_oSectionHashtable.ContainsKey(strSection))
                    {
                        m_oSectionHashtable.Add(strSection, new Hashtable());
                        m_strcoSection.Add(strSection);
                        m_strcoSectionOriginalCase.Add(strSectionOriginalCase);
                    }
                    else
                    {
                        throw new Exception("Section (" + strSection + ") already defined!");
                    }
                    oCurEntryHashtable = (Hashtable)m_oSectionHashtable[strSection];
                    strCurSection = strSection;
                    continue;
                }

                // Auf Entry überprüfen
                if (strLine.IndexOf('=') >= 1 && oCurEntryHashtable != null)
                {
                    string[] strarStrings = strLine.Split(new char[] { '=' }, 2);
                    string strEntry = strarStrings[0].Trim();
                    string strValue = strarStrings[1];

                    // Auf List-Funktion überprüfen
                    if (strEntry[0] == '@')
                    {
                        strEntry += m_iEntryListCount.ToString().PadLeft(6, '0');
                        m_iEntryListCount++;
                    }

                    // Auf Textblock überprüfen
                    if (strValue.Trim() == "@{")
                    {
                        try
                        {
                            strValue = ReadTextBlock(oTextReader);
                        }
                        catch (Exception oExc)
                        {
                            throw new Exception("Error in paragraph (" + strLine + ")!", oExc);
                        }
                    }

                    // Auf Verschlüsselung überprüfen
                    if (strEntry[0] == '#')
                    {
                        strEntry = strEntry.Substring(1);
                        if (string.IsNullOrEmpty(strValue))
                        {
                            throw new ApplicationException(string.Format("Kein Verschlüsselungswert beim Schlüssel '{0}' angegeben!", strEntry));
                        }
                        strValue = EncDec.Decrypt(strValue);
                    }

                    if (!oCurEntryHashtable.ContainsKey(strEntry.ToLower()))
                    {
                        oCurEntryHashtable[strEntry.ToLower()] = strValue;
                    }
                    else
                    {
                        throw new Exception("duplicate Entry (" + strEntry + ") Section (" + strCurSection + ") !");
                    }
                    continue;
                }

                // Fehler, Eintrag wurde nicht behandelt
                throw new Exception("Invalid INI-Entry (" + strLine + ")!");
            }
        }

        /// <summary>
        /// Liest die Daten des INI-Files ein (rekursiv aufrufbar)
        /// </summary>
        /// <param name="strFileName"></param>
        private void ReadIntern(string strFileName)
        {
            // INI-File öffnen
            StreamReader oStream;
            try
            {
                oStream = new StreamReader(File.OpenRead(strFileName), System.Text.Encoding.Default);
            }
            catch (Exception oExc)
            {
                string errorMessage = string.Format("Can't open INI-File ({0}).", oExc.ToString());
                throw new Exception(errorMessage, oExc);
            }

            try
            {
                ReadInternFromStreamReader(oStream);
            }
            finally
            {
                oStream.Close();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oStream"></param>
        /// <returns></returns>
        private string ReadTextBlock(TextReader oTextReader)
        {
            string strTextBlock = "";

            while (oTextReader.Peek() != -1)
            {
                string strLine = oTextReader.ReadLine();
                if (strLine == "@}")
                {
                    return strTextBlock;
                }
                if (strTextBlock != "")
                {
                    strTextBlock += "\n\r";
                    //Console.WriteLine(strTextBlock);
                }
                strTextBlock += strLine;
            }

            throw new Exception("End of Text-Paragraph not correct defined!");
        }


        /// <summary>
        /// Überprüft, ob der Eintrag existiert
        /// </summary>
        /// <param name="strSection"></param>
        /// <returns></returns>
        public bool ExistsSection(string strSection)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Liefert die Collection aller Sektionen
        /// </summary>
        /// <returns></returns>
        public ICollection GetSectionList()
        {
            return m_strcoSection;
        }

        /// <summary>
        /// Liefert die Collection aller Sektionen in originaler Groß-/Kleinschreibung
        /// </summary>
        /// <returns></returns>
        public ICollection GetSectionListOriginalCase()
        {
            return m_strcoSectionOriginalCase;
        }

        /// <summary>
        /// Liefert die Collection aller Elemente einer Sektion
        /// </summary>
        /// <param name="strSection"></param>
        /// <returns></returns>
        public ICollection GetEntryList(string strSection)
        {

            return ((Hashtable)m_oSectionHashtable[strSection.ToLower()]).Keys;
        }



        /// <summary>
        /// Überprüft, ob der Eintrag existiert
        /// </summary>
        /// <param name="strSection"></param>
        /// <param name="strEntry"></param>
        /// <returns></returns>
        public bool ExistsEntry(string strSection, string strEntry)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];
                if (oEntryHashTable.ContainsKey(strEntry))
                {
                    return true;
                }
                else
                {
                    // auf Liste prüfen
                    if (strEntry.StartsWith("@"))
                    {
                        foreach (string sEntry in oEntryHashTable.Keys)
                        {
                            if (sEntry.StartsWith(strEntry))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Liefert den String der mit dem Eintrag verknüpft ist.
        /// Existiert der Eintrag nicht, wird der Default-Wert zurückgegeben.
        /// </summary>
        /// <param name="strSection"></param>
        /// <param name="strEntry"></param>
        /// <param name="strDefault"></param>
        /// <returns></returns>
        public string GetString(string strSection, string strEntry, string strDefault)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];
                if (oEntryHashTable.ContainsKey(strEntry))
                {
                    return (string)oEntryHashTable[strEntry];
                }
            }
            // Eintrag wurde nicht gefunden
            return strDefault;
        }

        /// <summary>
        /// Liefert den String der mit dem Eintrag verknüpft ist.
        /// Existiert der Eintrag nicht, wird eine Fehlermeldung ausgegeben.
        /// </summary>
        /// <param name="strSection"></param>
        /// <param name="strEntry"></param>
        /// <returns></returns>
        public string GetString(string strSection, string strEntry)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];
                if (oEntryHashTable.ContainsKey(strEntry))
                {
                    return (string)oEntryHashTable[strEntry];
                }
            }

            // Eintrag wurde nicht gefunden
            throw new Exception(string.Format("INI-File-Entry ([{0}]{1}) not found!", strSection, strEntry));
        }



        /// <summary>
        /// Liefert den Int-Betrag der mit dem Eintrag verknüpft ist.
        /// Existiert der Eintrag nicht, wird eine Fehlermeldung ausgegeben.
        /// </summary>
        /// <param name="strSection"></param>
        /// <param name="strEntry"></param>
        /// <returns></returns>
        public int GetInt(string strSection, string strEntry)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];
                if (oEntryHashTable.ContainsKey(strEntry))
                {
                    System.IFormatProvider oFormatUs = new System.Globalization.CultureInfo("en-US", true);
                    string strTemp = (string)oEntryHashTable[strEntry];
                    int iTemp = int.Parse(strTemp, oFormatUs);
                    return iTemp;
                }
            }

            // Eintrag wurde nicht gefunden
            throw new Exception(string.Format("INI-File-Entry ([{0}]{1}) not found!", strSection, strEntry));
        }


        /// <summary>
        /// Liefert den Int-Betrag der mit dem Eintrag verknüpft ist.
        /// Existiert der Eintrag nicht, wird der Default-Wert zurückgegeben.
        /// </summary>
        /// <param name="strSection"></param>
        /// <param name="strEntry"></param>
        /// <param name="iDefault"></param>
        /// <returns></returns>
        public int GetInt(string strSection, string strEntry, int iDefault)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];
                if (oEntryHashTable.ContainsKey(strEntry))
                {
                    System.IFormatProvider oFormatUs = new System.Globalization.CultureInfo("en-US", true);
                    string strTemp = (string)oEntryHashTable[strEntry];
                    try
                    {
                        int iTemp = int.Parse(strTemp, oFormatUs);
                        return iTemp;
                    }
                    catch { }
                }
            }

            // Eintrag wurde nicht gefunden
            return iDefault;
        }


        /// <summary>
        /// Liefert den Decimal-Betrag der mit dem Eintrag verknüpft ist.
        /// Existiert der Eintrag nicht, wird eine Fehlermeldung ausgegeben.
        /// </summary>
        /// <param name="strSection"></param>
        /// <param name="strEntry"></param>
        /// <returns></returns>
        public decimal GetDecimal(string strSection, string strEntry)
        {
            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];
                if (oEntryHashTable.ContainsKey(strEntry))
                {
                    System.IFormatProvider oFormatUs = new System.Globalization.CultureInfo("en-US", true);
                    string strTemp = (string)oEntryHashTable[strEntry];
                    decimal dTemp = decimal.Parse(strTemp, oFormatUs);
                    return dTemp;
                }
            }

            // Eintrag wurde nicht gefunden
            throw new Exception(string.Format("INI-File-Entry ([{0}]{1}) not found!", strSection, strEntry));
        }


        /// <summary>
        /// Liefert eine String-Liste zurück
        /// </summary>
        /// <param name="strSection">Sektionsname</param>
        /// <param name="strEntry">Schlüsselname</param>
        /// <returns></returns>
        public StringCollection GetStringListe(string strSection, string strEntry)
        {
            Debug.Assert(strEntry[0] == '@', "@-Character must be infront of the name.");

            // Groß-Klein-Schreibung ignorieren
            strSection = strSection.ToLower();
            strEntry = strEntry.ToLower();

            string strEntryTmp;
            StringCollection strcoListe = new StringCollection();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];

                for (int i = 1; i <= m_iEntryListCount; i++)
                {
                    strEntryTmp = strEntry + i.ToString().PadLeft(6, '0');
                    if (oEntryHashTable.ContainsKey(strEntryTmp))
                    {
                        strcoListe.Add((string)oEntryHashTable[strEntryTmp]);
                    }
                }
            }
            return strcoListe;
        }

        /// <summary>
        /// Liefert eine String-Liste zurück
        /// </summary>
        /// <param name="strSection">Sektionsname</param>
        /// <param name="strEntry">Schlüsselname</param>
        /// <param name="bGrossKlein">Flag für die Berücksichtigung bzw. Nicht- der Groß/Kleinschreibung</param>
        /// <returns></returns>
        public StringCollection GetStringListe(string strSection, string strEntry, bool bGrossKlein)
        {
            Debug.Assert(strEntry[0] == '@', "@-Character must be infront of the name.");

            if (bGrossKlein)
            {
                // Groß-Klein-Schreibung ignorieren
                strSection = strSection.ToLower();
                strEntry = strEntry.ToLower();
            }

            string strEntryTmp;
            StringCollection strcoListe = new StringCollection();

            if (m_oSectionHashtable.ContainsKey(strSection))
            {
                Hashtable oEntryHashTable = (Hashtable)m_oSectionHashtable[strSection];

                for (int i = 1; i <= m_iEntryListCount; i++)
                {
                    strEntryTmp = strEntry + i.ToString().PadLeft(6, '0');
                    if (oEntryHashTable.ContainsKey(strEntryTmp))
                    {
                        strcoListe.Add((string)oEntryHashTable[strEntryTmp]);
                    }
                }
            }
            return strcoListe;
        }

        public string IniFileName
        {
            get { return m_strIniFileName; }
        }
        //*********************************************************************
        // Variablen

        /// <summary>
        /// Hashtable der Sections
        /// Der Value entspricht wiederum einer Hashtable für die Entrys
        /// </summary>
        public Hashtable SectionHashtable
        {
            get { return m_oSectionHashtable; }
            set { m_oSectionHashtable = value; }
        }
        private Hashtable m_oSectionHashtable = new Hashtable();

        /// <summary>
        /// Liste der Sections in der selben Reihenfolge wie in der INI-Datei 
        /// </summary>
        public StringCollection StrcoSection
        {
            get { return m_strcoSection; }
            set { m_strcoSection = value; }
        }
        private StringCollection m_strcoSection = new StringCollection();
        /// <summary>
        /// Speichert die Sektionnamen mit Groß/Kleinschreibung
        /// </summary>
        private StringCollection m_strcoSectionOriginalCase = new StringCollection();
        
        private string m_strIniFileName;

        private int m_iEntryListCount = 1;	// Entries mit einem @ als erstes Zeichen, werden um den Zähler erweitert

    }
}
