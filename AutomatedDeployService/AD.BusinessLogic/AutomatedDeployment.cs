using AD.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.BusinessLogic
{
    public class AutomatedDeployment
    {
        /// <summary>
        /// Configuration parameters used by the service
        /// </summary>

        // Location of log file
        private string LogFileLocation;
        // Location of DB File
        private const string DBFileLocation = "repo.db";
        // Location of Default Log File
        private const string DefaultLogFileLocation = "app_log.txt";

        // Polling Duration in seconds - Default 5 seconds
        public int PollingDuration = 5000;

        // All Deployment Jobs
        private List<DeploymentParams> AllDeploymentJobs = new List<DeploymentParams>();

        public AutomatedDeployment()
        {
            try
            {
                ReadAllSettings();

                // Create DBFile
                if (File.Exists(DBFileLocation) == false)
                {
                    FileStream fs = File.Create(DBFileLocation);
                    fs.Close();
                }

                // Create Log File
                if(LogFileLocation == null)
                {
                    LogFileLocation = "log.txt";
                }

                if (File.Exists(LogFileLocation) == false)
                {
                    FileStream fs = File.Create(LogFileLocation);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                LogData(ex.Message, DefaultLogFileLocation);
            }
        }

        public void ReadAllSettings()
        {
            try
            {
                List<string> settings = new List<string>();
                IEnumerable<string> allSettings = File.ReadLines("settings.config");

                foreach (string settingLine in allSettings)
                {
                    // Discard comments
                    if (settingLine != null)
                    {
                        if (settingLine.Substring(0, 1).Equals("#") == false)
                        {
                            string[] settingLineParams = settingLine.Split('|');
                            switch (settingLineParams[0].ToLower())
                            {
                                case "logfilelocation":
                                    LogFileLocation = settingLineParams[1];
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
                LogData(ex.Message, DefaultLogFileLocation);
            }
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
                LogData(ex.Message);
                return null;
            }
        }

        public bool UpdateFileProperties(DeploymentParams deploymentParam, FileProperties fileProperties)
        {
            try
            {
                List<string> allLines = File.ReadLines(DBFileLocation).ToList();

                List<string> allNewLines = new List<string>();

                bool DataFound = false;
                foreach (string line in allLines)
                {
                    if (line.Length > 0)
                    {
                        string[] allParams = line.Split('|');
                        if (allParams[0] == deploymentParam.DeploymentID)
                        {
                            allNewLines.Add(deploymentParam.DeploymentID + "|" + fileProperties.FileSize.ToString() + "|" + fileProperties.LastModifiedTime);
                            DataFound = true;
                        }
                        else
                        {
                            allNewLines.Add(line);
                        }
                    }
                }
                // Update the file
                if (DataFound)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (string line in allNewLines)
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
                    string newLine = deploymentParam.DeploymentID + "|" + fileProperties.FileSize + "|" + fileProperties.LastModifiedTime;
                    writer.WriteLine(newLine);
                    writer.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Write Exception to Default Log File
                LogData(ex.Message);
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
                            FileSize = Convert.ToInt64(allParams[1]),
                            LastModifiedTime = allParams[2]
                        };
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                // Write Exception to Default Log File
                LogData(ex.Message);
                return null;
            }

        }

        public void PerformDeployment()
        {
            try
            {
                // Perform deployment for each job
                foreach (DeploymentParams deploymentParam in AllDeploymentJobs)
                {
                    // Get File Properties
                    FileProperties fileProperties = GetFileProperties(deploymentParam.MonitorFileLocation);
                    FileProperties dbFileProperties = GetFilePropertiesFromDB(deploymentParam.DeploymentID);

                    if (fileProperties == null)
                    {
                        throw new Exception("Deployment File " + deploymentParam.MonitorFileLocation + " not found at specified location");
                    }

                    if (dbFileProperties == null)
                    {
                        UpdateFileProperties(deploymentParam, fileProperties);
                        RunBatchJob(deploymentParam.BatchFileLocation);


                    }
                    else
                    {
                        // Perform deployment if the file size or last modified time has changed
                        if (fileProperties.FileSize != dbFileProperties.FileSize ||
                            fileProperties.LastModifiedTime != fileProperties.LastModifiedTime)
                        {
                            RunBatchJob(deploymentParam.BatchFileLocation);

                            // Update new file size and time in db
                            UpdateFileProperties(deploymentParam, fileProperties);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Write Exception to Default Log File
                LogData(ex.Message);
            }
        }

        public void RunBatchJob(string batchLocation)
        {
            try
            {
                Process.Start(batchLocation);
                LogData(batchLocation + ": Executed Successfully");
            }
            catch (Exception ex)
            {
                LogData("File Name: " + batchLocation + " " + ex.Message);
            }

        }

        public void LogData(string logMessage, string logFileLocation = "DEFAULT")
        {
            if (logFileLocation == "DEFAULT")
            {
                logFileLocation = LogFileLocation;
            }

            StreamWriter writer = File.AppendText(logFileLocation);
            writer.WriteLine(DateTime.Now.ToString() + ": " + logMessage);
            writer.Close();
        }
    }
}
