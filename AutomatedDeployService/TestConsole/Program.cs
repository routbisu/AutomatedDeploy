using AutomatedDeployService;
using AutomatedDeployService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace TestConsole
{
    class Program
    {
        public static AutomatedDeploy automatedDeploy = new AutomatedDeploy();

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            automatedDeploy.PerformDeployment();
        }

        static void Main(string[] args)
        {

            Timer deploymentTimer = new Timer();
            deploymentTimer.Interval = automatedDeploy.PollingDuration;
            deploymentTimer.Elapsed += timer_Elapsed;
            deploymentTimer.Start();

            Console.ReadLine();
        }
    }
}
