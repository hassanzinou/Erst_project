using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLExecuter
{
    public class Setting
    {
       
        public Setting()
        {

            ConfigFile applicationConfigFile = new ConfigFile(Directory.GetCurrentDirectory() + "//" + "Options.ini");
            ExecutionOrder = applicationConfigFile.GetValue("Scripts", "ExecutionOrder", true);
            Dsn = applicationConfigFile.GetValue("IKA1", "DSN", true);
            User = applicationConfigFile.GetValue("IKA1", "User", true);
            Password = applicationConfigFile.GetValue("IKA1", "Password", true);
            Password = Crypter.DecryptConfigValue(ref applicationConfigFile, "IKA1", "Password", User);
            ConnectionString = $"DSN={Dsn};Uid={User};Pwd={Password};";
            ScriptPath = applicationConfigFile.GetValue("Options", "ScriptPath", true);
            BreakOnError= applicationConfigFile.GetValue("Options", "BreakOnError", true);
            ExecutionMode = applicationConfigFile.GetValue("Scripts", "ExecutionMode", true);
            ExecutionFolder = applicationConfigFile.GetValue("Scripts", "ExecutionFolder", true);
            CheckExecutionType();
        }

        private void CheckExecutionType()
        { 
            switch (ExecutionMode) {
                case "2":
                    if (string.IsNullOrEmpty(ExecutionFolder))
                        throw new Exception("Setting Modus:"+ExecutionMode + " benötigt eine Verzeichnissangabe  \r\n \\NeuErzeugenCaluculated\r\n \\AuslesenHistorie");
                    
                    break;
                case "3":
                    if (string.IsNullOrEmpty(ExecutionOrder))
                        throw new Exception("Setting Modus:" + ExecutionMode + " benötigt eine vordefinierte Scriptreihenfolge. 00001,0002,...\r\nOptions.ini - property 'ExecutionOrder'");
                    break;

            }
        }

        private string Dsn { get; set; }
        private string User { get; set; }
        private string Password { get; set; }
        public string ConnectionString { get; set; }
        public string ExecutionOrder { get; set; }
        public string ScriptPath { get; set; }
        public string BreakOnError { get; set; }
        public string ExecutionMode { get; set; }
        public string ExecutionFolder { get; set; }


        
    }
}
