using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class CSVHelper
    {
        private static FileHelperEngine CreateFileHelperEngine(Type type)
        {
            if (!type.IsClass)
                throw new InvalidCastException("Cannot use '" + type.FullName + "' as it is not a class");

            return new FileHelperEngine(type);
        }
        public static bool WriteCSVFile<T>(List<T> content,string fullPath) {
            if(!string.IsNullOrEmpty(fullPath))
            if (content != null || content.Count > 0)
            {
                var engine = CreateFileHelperEngine(content[0].GetType());
                engine.HeaderText = engine.GetFileHeader();
                engine.WriteFile(fullPath, (IEnumerable<object>)content);
                return true;
            }
            return false;
        }
    }
}
