using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Library
{
    public class ConfigFile
    {
        private string _Filename;
        private string _CommentCharacters;
        private string _RegCommentStr;
        private Regex _RegEntry;
        private Regex _RegCaption;
        private List<string> _Lines = new List<string>();

        public string Filename
        {
            get
            {
                return _Filename;
            }
        }
        public string CommentCharacters
        {
            get
            {
                return _CommentCharacters;
            }
            set
            {
                _CommentCharacters = value;
                _RegCommentStr = @"(\s*[" + _CommentCharacters + "](?<comment>.*))?";
                _RegEntry = new Regex(@"^[ \t]*(?<entry>([^=])+)=(?<value>([^=" + _CommentCharacters + "])+)" + _RegCommentStr + "$");
                _RegCaption = new Regex(@"^[ \t]*(\[(?<caption>([^\]])+)\]){1}" + _RegCommentStr + "$");
            }
        }
        public ConfigFile(string filename)
        {
            CommentCharacters = "#;";
            this._Filename = filename;
            this.Open(this._Filename);
        }
        public bool Open(string filename)
        {
            if (!System.IO.File.Exists(filename))
                throw new IOException("File " + filename + " not found");
            _Filename = filename;
            _Lines = new List<string>();

            StreamReader reader = new StreamReader(_Filename);
            while (!reader.EndOfStream)
                _Lines.Add(reader.ReadLine().TrimEnd());
            reader.Close();

            return true;
        }
        public bool Save()
        {
            bool Result = false;

            if (!string.IsNullOrEmpty(_Filename))
            {
                StreamWriter writer = new StreamWriter(_Filename, false);
                foreach (string line in _Lines)
                    writer.WriteLine(line);
                writer.Close();

                Result = true;
            }

            return Result;
        }
        private int SearchCaptionLine(string caption, bool caseSensitive)
        {
            if (!caseSensitive)
                caption = caption.ToLower();

            bool multiline = false;
            if (_Lines != null)
            {
                for (var i = 0; i <= _Lines.Count - 1; i++)
                {
                    string line = _Lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (!multiline && line.EndsWith("<<EOT"))
                        multiline = true;
                    else if (multiline && line.EndsWith("EOT"))
                        multiline = false;
                    else
                    {
                        if (!caseSensitive)
                            line = line.ToLower();
                        if (line == "[" + caption + "]")
                            return i;
                    }
                }
            }
            // Überschrift nicht gefunden
            return -1;
        }
        private int SearchEntryLine(string caption, string entry, bool caseSensitive)
        {
            if (!caseSensitive)
            {
                caption = caption.ToLower();
                entry = entry.ToLower();
            }

            int captionStart = SearchCaptionLine(caption, caseSensitive);
            if (captionStart < 0)
                return -1;

            bool multiline = false;

            for (var i = captionStart + 1; i <= _Lines.Count - 1; i++)
            {
                string line = _Lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                    continue;
                if (!multiline && line.EndsWith("<<EOT") && line.StartsWith(entry))
                    // Multiline-Eintrag gefunden
                    return i;
                else if (!multiline && line.EndsWith("<<EOT") && !line.StartsWith(entry))
                    multiline = true;
                else if (multiline && line.EndsWith("EOT"))
                    multiline = false;
                else
                {
                    if (!caseSensitive)
                        line = line.ToLower();
                    if (line.StartsWith("["))
                        return -1;
                    if (Regex.IsMatch(line, @"^[ \t]*[" + _CommentCharacters + "]"))
                        // Kommentar
                        continue;
                    if (line.StartsWith(entry))
                        // Eintrag gefunden
                        return i;
                }
            }

            // Eintrag nicht gefunden
            return -1;
        }
        public bool CommentValue(string caption, string entry, bool caseSensitive)
        {
            int line = SearchEntryLine(caption, entry, caseSensitive);

            if (line < 0)
                return false;

            _Lines[line] = CommentCharacters[0] + _Lines[line];

            return true;
        }
        public bool DeleteValue(string caption, string entry, bool caseSensitive)
        {
            int i = SearchEntryLine(caption, entry, caseSensitive);

            if (i < 0)
                return false;

            string line = _Lines[i];
            _Lines.RemoveAt(i);

            string oldValue = line.Substring(line.IndexOf("=") + 1).Trim();
            if (oldValue == "<<EOT")
            {
                while (i < _Lines.Count && oldValue != "EOT")
                {
                    oldValue = _Lines[i];
                    _Lines.RemoveAt(i);
                }
            }

            return true;
        }
        public string GetValue(string caption, string entry, bool caseSensitive, string defaultValue)
        {
            var result = GetValue(caption, entry, caseSensitive);

            if (string.IsNullOrEmpty(result))
                result = defaultValue;

            return result;
        }
        public string GetValue(string caption, string entry, bool caseSensitive, bool throwException)
        {
            var result = GetValue(caption, entry, caseSensitive);

            if (string.IsNullOrEmpty(result) && throwException)
                throw new Exception(string.Format("Value not found for entry '{0}' in section '{1}' in file {2}", entry, caption, _Filename));

            return result;
        }
        public string GetValue(string caption, string entry, bool caseSensitive)
        {
            int line = SearchEntryLine(caption, entry, caseSensitive);

            if (line < 0)
                return string.Empty;

            int pos = _Lines[line].IndexOf("=");

            if (pos < 0)
                return string.Empty;

            string value = _Lines[line].Substring(pos + 1).Trim();

            if (value == "<<EOT")
            {
                value = string.Empty;
                line += 1;
                if (line < _Lines.Count)
                {
                    string part = _Lines[line];
                    if (part != "EOT")
                        value = part;
                    while (line < _Lines.Count - 1 && part != "EOT")
                    {
                        line += 1;
                        part = _Lines[line];
                        if (part != "EOT")
                            value = value + "\r\n" + part;
                    }
                }
            }

            return value;
        }
        public bool SetValue(string caption, string entry, string value, bool caseSensitive)
        {
            if (!caseSensitive)
            {
                caption = caption.ToLower();
                entry = entry.ToLower();
            }

            int lastCommentedFound = -1;
            int captionStart = SearchCaptionLine(caption, caseSensitive);

            if (captionStart < 0)
            {
                _Lines.Add("[" + caption + "]");
                if (value.Contains("\r\n"))
                    _Lines.Add(entry + "=<<EOT" + "\r\n" + value + "\r\n" + "EOT");
                else
                    _Lines.Add(entry + "=" + value);

                return true;
            }

            int entryLine = SearchEntryLine(caption, entry, caseSensitive);

            for (int i = captionStart + 1; i <= _Lines.Count - 1; i++)
            {
                string line = _Lines[i].Trim();

                if (string.IsNullOrEmpty(line))
                    continue;
                if (!caseSensitive)
                    line = line.ToLower();
                // Ende, wenn der nächste Abschnitt beginnt
                if (line.StartsWith("["))
                {
                    if (value.Contains("\r\n"))
                        _Lines.Insert(i, entry + "=<<EOT" + "\r\n" + value + "\r\n" + "EOT");
                    else
                        _Lines.Insert(i, entry + "=" + value);
                    return true;
                }

                // Suche auskommentierte, aber gesuchte Einträge
                // (evtl. per Parameter bestimmen können?), falls
                // der Eintrag noch nicht existiert.
                if (entryLine < 0)
                {
                    if (Regex.IsMatch(line, @"^[ \t]*[" + _CommentCharacters + "]"))
                    {
                        string tmpLine = line.Substring(1).Trim();
                        if (tmpLine.StartsWith(entry))
                        {
                            // Werte vergleichen, wenn gleich,
                            // nur Kommentarzeichen löschen
                            int pos = tmpLine.IndexOf("=");
                            if (pos > 0)
                            {
                                if (value == tmpLine.Substring(pos + 1).Trim())
                                {
                                    _Lines[i] = tmpLine;
                                    return true;
                                }
                            }
                            lastCommentedFound = i;
                        }
                    }
                }

                if (line.StartsWith(entry))
                {
                    string oldValue = line.Substring(line.IndexOf("=") + 1).Trim();
                    if (value.Contains("\r\n"))
                        _Lines[i] = entry + "=<<EOT" + "\r\n" + value + "\r\n" + "EOT";
                    else
                        _Lines[i] = entry + "=" + value;
                    if (oldValue == "<<EOT")
                    {
                        i += 1;
                        while (i < _Lines.Count && oldValue != "EOT")
                        {
                            oldValue = _Lines[i];
                            _Lines.RemoveAt(i);
                        }
                    }
                    return true;
                }
            }

            int j = captionStart + 1;
            if (lastCommentedFound > 0)
                j = lastCommentedFound + 1;

            if (value.Contains("\r\n"))
                _Lines.Insert(j, entry + "=<<EOT" + "\r\n" + value + "\r\n" + "EOT");
            else
                _Lines.Insert(j, entry + "=" + value);

            return true;
        }
        public SortedList<string, string> GetCaption(string caption)
        {
            SortedList<string, string> result = new SortedList<string, string>();
            bool captionFound = false;
            bool multiline = false;

            for (var i = 0; i <= _Lines.Count - 1; i++)
            {
                string line = _Lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                    continue;
                if (!multiline && line.EndsWith("<<EOT"))
                    multiline = true;
                else if (multiline && line.EndsWith("EOT"))
                    multiline = false;
                else
                {
                    // Erst den gewünschten Abschnitt suchen
                    if (!captionFound)
                    {
                        if (line.ToLower() != "[" + caption + "]")
                            continue;
                        else
                        {
                            captionFound = true;
                            continue;
                        }
                    }
                    // Ende, wenn der nächste Abschnitt beginnt
                    if (line.StartsWith("["))
                        break;
                    // Kommentar
                    if ((Regex.IsMatch(line, @"^[ \t]*[" + _CommentCharacters + "]")))
                        continue;
                    // Endlich den Wert ermitteln
                    int pos = line.IndexOf("=");
                    if ((pos < 0))
                        result.Add(line, "");
                    else
                        result.Add(line.Substring(0, pos).Trim(), line.Substring(pos + 1).Trim());
                }
            }

            return result;
        }
        public List<string> GetAllCaptions()
        {
            List<string> result = new List<string>();
            bool multiline = false;

            for (var i = 0; i <= _Lines.Count - 1; i++)
            {
                string line = _Lines[i].Trim();
                if (!multiline && line.EndsWith("<<EOT"))
                    multiline = true;
                else if (multiline && line.EndsWith("EOT"))
                    multiline = false;
                else
                {
                    Match mCaption = _RegCaption.Match(line);
                    if (mCaption.Success)
                        result.Add(mCaption.Groups["caption"].Value.Trim());
                }
            }

            return result;
        }
        public XmlDocument ExportToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement(Path.GetFileNameWithoutExtension(_Filename));
            doc.AppendChild(root);
            XmlElement caption = null;

            for (var i = 0; i <= _Lines.Count - 1; i++)
            {
                string line = _Lines[i].Trim();
                Match mEntry = _RegEntry.Match(line);
                Match mCaption = _RegCaption.Match(line);

                if (mCaption.Success)
                {
                    caption = doc.CreateElement(mCaption.Groups["caption"].Value.Trim());
                    root.AppendChild(caption);
                    continue;
                }
                if (mEntry.Success)
                {
                    XmlElement xe = doc.CreateElement(mEntry.Groups["entry"].Value.Trim());
                    xe.InnerXml = mEntry.Groups["value"].Value.Trim();
                    if (caption == null)
                        root.AppendChild(xe);
                    else
                        caption.AppendChild(xe);
                }
            }

            return doc;
        }

    }
}
