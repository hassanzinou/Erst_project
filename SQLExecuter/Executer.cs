using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SQLExecuter
{
    public class Executer
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        List<string> filelist;
        List<string> shortcuts;
        public void Execute()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
                log.Info("Reading settings file");
                Setting setting = new Setting();

                filelist = GetFiles(setting.ScriptPath, "*.sql");
                filelist = filelist.OrderBy(s => s).ToList();

                log.Info($"Loaded scripts {filelist.Count}");

                if (setting.ExecutionMode == "1")
                {
                    log.Info("Executing all Scripts");
                    ExecuteSqlQueries(filelist, setting.ConnectionString, setting.BreakOnError);
                }
                else if (setting.ExecutionMode == "2")
                {
                    log.Info("Executing Scripts from: " + setting.ExecutionFolder);
                    shortcuts = GetFiles(setting.ExecutionFolder, "*.Lnk");
                    shortcuts = shortcuts.OrderBy(s => s).ToList();
                    List<string> sql = new List<string>();

                    for (int i = 0; i < shortcuts.Count; i++)
                    {
                        string splitName = Path.GetFileName(shortcuts[i]).Split('_')[0];
                        sql.Add(filelist.FirstOrDefault(x => x.Contains(splitName)));
                    }
                    ExecuteSqlQueries(sql, setting.ConnectionString, setting.BreakOnError);
                }
                else if (setting.ExecutionMode == "3")
                {

                    log.Info($"Scripts to execute: {setting.ExecutionOrder}");
                    string[] order = setting.ExecutionOrder.Split(',');

                    List<string> sql = new List<string>();
                    for (int i = 0; i < order.Length; i++)
                    {
                        sql.Add(filelist.FirstOrDefault(x => x.Contains(order[i])));
                    }

                    ExecuteSqlQueries(sql, setting.ConnectionString, setting.BreakOnError);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
        void ExecuteSqlQueries(List<string> sqlQueries, string connectionString, string breakOnError)
        {
            try
            {
                bool breakSet = false;
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    OdbcTransaction transaction = null;
                    for (int i = 0; i < sqlQueries.Count; i++)
                    {
                        try
                        {
                            if (breakSet) { break; }
                            string script = ReadFileContent(sqlQueries[i]);
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            log.Info($"Start execution of Script {Path.GetFileName(sqlQueries[i])}");
                            transaction = connection.BeginTransaction();
                            Console.WriteLine($"Start execution of Script {Path.GetFileName(sqlQueries[i])}");
                            using (OdbcCommand command = new OdbcCommand(script, connection))
                            {
                                command.CommandType = CommandType.Text;
                                command.Transaction = transaction;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            watch.Stop();
                            var elapsedMs = watch.ElapsedMilliseconds * 0.001;
                            log.Info($"Duration of execution {elapsedMs} seconds");
                            Console.WriteLine($"Duration of execution {elapsedMs} seconds");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            breakSet = breakOnError == "1" ? true : false;
                            Console.WriteLine(ex.Message);
                            log.Error(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        string ReadFileContent(string filePath)
        {
            string fileContents = "";
            try
            {

                try
                {
                    fileContents = File.ReadAllText(filePath);

                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return fileContents;
        }

        List<string> GetFiles(string directoryPath, string extension)
        {
            List<string> filesList = new List<string>();
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath, extension);
                    filesList.AddRange(files);

                }
                else
                {
                    throw new ArgumentException($"Directory path {directoryPath} does not exist.");
                }
            }
            catch (Exception ex)
            { log.Error(ex.Message); }

            return filesList;
        }
    }
}
