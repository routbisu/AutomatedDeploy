using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDeployService.Models
{
    public class DeploymentParams
    {
        public string DeploymentID { get; set; }
        public string BatchFileLocation { get; set; }
        public string MonitorFileLocation { get; set; }
    }
}
