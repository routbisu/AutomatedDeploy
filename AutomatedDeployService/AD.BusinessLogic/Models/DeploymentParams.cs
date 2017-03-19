using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.BusinessLogic.Models
{
    public class DeploymentParams
    {
        public string DeploymentID { get; set; }
        public string BatchFileLocation { get; set; }
        public string MonitorFileLocation { get; set; }
    }
}
