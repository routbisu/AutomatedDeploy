using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDeployService.Models
{
    public class AutomatedDeploy
    {
        /// <summary>
        /// Configuration parameters used by the service
        /// </summary>
       
        // Location of log file
        private string LogFileLocation;
        // Location of DB File
        private string DBFileLocation;
        // Polling Duration in seconds
        public int PollingDuration = 5000;

        // All Deployment Jobs
        private List<DeploymentParams> AllDeploymentJobs;
        
        public AutomatedDeploy()
        {
            ReadAllSettings();
            // Create LogFile

            if (File.Exists(LogFileLocation) == false)
            {
                File.Create(LogFileLocation);
            }

            // Create DBFile
            if (File.Exists(DBFileLocation) == false)
            {
                File.Create(DBFileLocation);
            }
            
        }

        public IEnumerable<SettingFileParam> ReadAllSettings()
        { 
            try
            {
                List<string> settings = new List<string>();
                IEnumerable<string> allSettings = File.ReadLines("settings.config");
                
                foreach(string settingLine in allSettings)
                {
                    // Discard comments
                    if(settingLine != null)
                    {
                        if(settingLine.Substring(0, 1).Equals("#") == false)
                        {
                            string[] settingLineParams = settingLine.Split('|');
                            switch(settingLineParams[0].ToLower())
                            {
                                case "logfilelocation":
                                    LogFileLocation = settingLineParams[1];
                                    break;

                                case "dbfilelocation":
                                    DBFileLocation = settingLineParams[1];
                                    break;

                                case "pollingduration":
                                    PollingDuration = Convert.ToInt32(settingLineParams[1]);
                                    break;

                                case "deploymentparams":
                                    AllDeploymentJobs.Add(new DeploymentParams
                                    {
                                        DeploymentID = settingLineParams[1],
                                        BatchFileLocation = settingLineParams[2],
                                        MonitorFileLocation = settingLineParams[3]
                                    });
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Write Exception to Default Log File
                StreamWriter writer = File.AppendText("default_log.txt");
                writer.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                writer.Close();
            }
            return null;
        }
        
        public FileProperties GetFileProperties(string fileLocation)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileLocation);
                DateTime datetime = File.GetLastWriteTime(fileLocation);
                return new FileProperties
                {
                    FileSize = fileInfo.Length,
                    LastModifiedTime = datetime.ToString()
                };
            }
            catch (Exception ex)
            {   
                // Write Exception to Default Log File
                StreamWriter writer = File.AppendText(LogFileLocation);
                writer.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                writer.Close();
                return null;
            }
        }

        public bool UpdateFileProperties(string deployID, DeploymentParams deploymentParam, FileProperties fileProperties)
        {
            try
            {
                IEnumerable<string> allLines = File.ReadLines(DBFileLocation);
                bool DataFound = false;
                foreach (string line in allLines)
                {
                    if (line.Length > 0)
                    {
                        string[] allParams = line.Split('|');
                        if (allParams[0] == deployID)
                        {
                            allParams[1] = deploymentParam.MonitorFileLocation;
                            allParams[2] = fileProperties.FileSize.ToString();
                            allParams[3] = fileProperties.LastModifiedTime;

                            DataFound = true;
                        }
                    }
                }
                // Update the file
                if (DataFound)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (string line in allLines)
                    {
                        stringBuilder.Append(line);
                        stringBuilder.Append(Environment.NewLine);
                    }
                    File.WriteAllText(DBFileLocation, stringBuilder.ToString());
                    return true;
                }
                else
                {
                    // Add new line to the db file
                    StreamWriter writer = File.AppendText(DBFileLocation);
                    string newLine = deploymentParam.DeploymentID + "|" + deploymentParam.MonitorFileLocation
                        + "|" + fileProperties.FileSize + "|" + fileProperties.LastModifiedTime;
                    writer.WriteLine(newLine);
                    writer.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Write Exception to Default Log File
                StreamWriter writer = File.AppendText(LogFileLocation);
                writer.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                writer.Close();
                return false;
            }
            
        }

        public FileProperties GetFilePropertiesFromDB(string deployID)
        {
            try
            {
                IEnumerable<string> allLines = File.ReadLines(DBFileLocation);
                foreach (string line in allLines)
                {
                    string[] allParams = line.Split('|');
                    if (allParams[0] == deployID)
                    {
                        return new FileProperties
                        {
                            FileSize = allParams[2],
                            LastModifiedTime = allParams[3]
                        };
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                // Write Exception to Default Log File
                StreamWriter writer = File.AppendText(LogFileLocation);
                writer.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                writer.Close();
                return null;
            }
            
        }

        public void PerformDeployment()
        {
            // Perform deployment for each job
            foreach(DeploymentParams deploymentParam in AllDeploymentJobs)
            {

            }
        }
    }
}
