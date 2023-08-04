using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Program
    {
        private static LogFile logger;
        static void Main(string[] args)
        {
            logger = new LogFile();
            logger.WriteHeadAndTail = true;
            logger.WriteToConsole = true;
            logger.LogAll = true;

            //logger.OpenWithoutHead(Directory.GetCurrentDirectory() + "\\Log.log");
            logger.Open(Directory.GetCurrentDirectory() + "\\Log.log");
            logger.WriteLine("Test log","T");
            logger.WriteLine("Test log");
            logger.WriteLine( new string[] { "Test log", "Test log", "Test log" });
            try
            {
                int zahl1 = 0;
                int zahl = 4 / zahl1;
            }
            catch (Exception ex)
            { 
                logger.LogException(ex); 
            }
     
            //logger.WriteColLine("Das ist ein TestLog", "Das ist eine Bemerkung");

            //logger.CloseWithoutTail();
            logger.Close();
            
        }
    }
}
