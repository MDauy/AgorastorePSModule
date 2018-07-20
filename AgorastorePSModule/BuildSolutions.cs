using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AgorastorePSModule
{
    
    public class BuildSolutions : PSCmdlet
    {
        private const string rootDirectoryPath = "./rootDirectory.txt";


        [Parameter(
            Mandatory = false)]
        [Alias ("root")]
        public string RootDirectory { get; set; }

        public void BuildCmdlet ()
        {
            if (!string.IsNullOrEmpty (this.RootDirectory))
            {
                File.WriteAllBytes(rootDirectoryPath, new byte[0]);
                File.WriteAllBytes(rootDirectoryPath, Encoding.ASCII.GetBytes(RootDirectory));                
            }


        }

        private IEnumerable<string> GetRepositories()
        {
            var rootDirectoryFile = File.ReadAllLines(rootDirectoryPath);

            return new List<string>
        {
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.Common"),
            string.Format("{0}\\{1}", rootDirectoryFile, "agorastore.Admin"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.API"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.OAuth"),
            string.Format("{0}\\{1}", rootDirectoryFile, "agorastore\\AgorastoreWeb"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.DAC\\Agorastore.DAC"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.AzureWorkers"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.BiddingEngine"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.MailsService"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.MobileWeb"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.Monitoring"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.TaskLauncher"),
            string.Format("{0}\\{1}", rootDirectoryFile, "Agorastore.MobileWeb")
        };
        }
    }    
}
