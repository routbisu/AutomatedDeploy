using AutomatedDeployService;
using AutomatedDeployService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            AutomatedDeploy automatedDeploy = new AutomatedDeploy();
            //automatedDeploy.ReadAllSettings();
            Console.WriteLine(automatedDeploy.CheckFileSize("settings.config"));
            Console.WriteLine(automatedDeploy.GetLastModifiedDate("settings.config"));

            Console.ReadKey();
        }
    }
}
