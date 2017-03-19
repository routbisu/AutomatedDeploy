using AD.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AD.RunConsole
{
    class Program
    {
        public static AutomatedDeployment automatedDeploy = new AutomatedDeployment();

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
