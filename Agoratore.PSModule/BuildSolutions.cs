using Microsoft.Build.BuildEngine;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace AgorastorePSModule
{

    public class BuildSolutions : PSCmdlet
    {
        private string rootDirectoryPath;



        [Parameter(
            Mandatory = false)]
        [Alias("root")]
        public string RootDirectory { get; set; }

        public void BuildCmdlet()
        {
            var currentDirectory = Environment.CurrentDirectory.Replace("\\", "/");

            rootDirectoryPath = Regex.Replace(currentDirectory, "bin/Debug/*.*", "rootDirectory.txt");

            if (!string.IsNullOrEmpty(this.RootDirectory))
            {
                File.WriteAllBytes(rootDirectoryPath, new byte[0]);
                File.WriteAllBytes(rootDirectoryPath, Encoding.ASCII.GetBytes(RootDirectory));
            }
            var outputHeaderRow = new List<string>();
            var outputItemRow = new List<string>();
            var repos = this.GetRepositories();
            if (repos != null && repos.Any())
            {
                FileLogger logger = new FileLogger();
                string logFilePath = string.Empty;
                foreach (var repo in repos)
                {
                    try
                    {
                        logFilePath = repo.Remove(repo.LastIndexOf("\\") + 1) + "build.log";

                        logger.Parameters = @"logFile=" + logFilePath;

                        ProjectCollection pc = new ProjectCollection();

                        Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                        GlobalProperty.Add("Configuration", "Debug");
                        GlobalProperty.Add("Platform", "Any CPU");

                        BuildRequestData buildRequest = new BuildRequestData(repo, GlobalProperty, null, new string[] { "Build" }, null);

                        //register file logger using BuildParameters
                        BuildParameters bp = new BuildParameters(pc);
                        bp.Loggers = new List<Microsoft.Build.Framework.ILogger> { logger }.AsEnumerable();

                        //build solution
                        BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);

                        //Unregister all loggers to close the log file               
                        pc.UnregisterAllLoggers();

                        //read lines from log file having project build output details
                        string[] solutionBuildOutputs = File.ReadAllLines(logFilePath);

                        //write the result of solution build to html report
                        outputHeaderRow.Add(string.Format("{0};Build Result", repo));

                        //split the contents of logger file to retrieve project build details
                        string[] splitter = { "__________________________________________________" };
                        string loggerOutput = File.ReadAllText(logFilePath);
                        string[] projectResults = loggerOutput.Split(splitter, StringSplitOptions.None);
                        foreach (string projectBuildDetails in projectResults)
                        {
                            if (projectBuildDetails.Contains("(default targets):"))
                            {
                                if (projectBuildDetails.Contains("Done building project \""))
                                {
                                    //write the result of failed projects build to html report
                                    string[] lines = projectBuildDetails.Split("\n".ToCharArray());
                                    string buildFailedProjectName = lines.Where(x => x.Contains("Done building project \"")).FirstOrDefault();
                                    buildFailedProjectName = buildFailedProjectName.Replace("Done building project ", string.Empty).Trim();
                                    buildFailedProjectName = buildFailedProjectName.Replace("\"", string.Empty);
                                    buildFailedProjectName = buildFailedProjectName.Replace(" -- FAILED.", string.Empty);
                                    outputItemRow.Add(buildFailedProjectName + ";FAILED");
                                }
                                else
                                {
                                    //write the result of successfully built projects to html report
                                    string[] lines = projectBuildDetails.Split("\n".ToCharArray());
                                    string buildSuccededProjectName = lines.Where(x => x.Contains(" (default targets):")).FirstOrDefault().Replace("\" (default targets):", "");
                                    string finalProjectName = buildSuccededProjectName.Substring(buildSuccededProjectName.LastIndexOf("\\") + 1);
                                    outputItemRow.Add(finalProjectName + ";SUCCEEDED");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                throw new Exception("No repos found");
            }

        }

        private IEnumerable<string> GetRepositories()
        {
            var rootDirectoryFile = File.ReadAllLines(rootDirectoryPath).FirstOrDefault();
            if (rootDirectoryFile != null)
            {
                return new List<string>
            {
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.Common\\Agorastore.Common.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "agorastore.Admin\\Agorastore.Admin.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.API\\Agorastore.API.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.OAuth\\Agorastore.OAuth.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "agorastore\\AgorastoreWeb\\AgorastoreWeb.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.DAC\\Agorastore.DAC\\Agorastore.DAC.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.AzureWorkers\\Agorastore.AzureWorkers.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.BiddingEngine\\Agorastore.BiddingEngine.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.MailsService\\Agorastore.MailsService.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.MobileWeb\\Agorastore.MobileWeb.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.Monitoring\\Agorastore.Monitoring.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.TaskLauncher\\Agorastore.TaskLauncher.sln"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.MobileWeb\\Agorastore.MobileWeb.sln")
        };
            }
            return null;
        }
    }
}
