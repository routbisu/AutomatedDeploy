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
        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AutomatedDeployment.PerformDeployment();
        }

        static void Main(string[] args)
        {
            AutomatedDeployment.AutoDeployInit();

            Timer deploymentTimer = new Timer();
            deploymentTimer.Interval = AutomatedDeployment.PollingDuration;
            deploymentTimer.Elapsed += timer_Elapsed;
            deploymentTimer.Start();

            Console.ReadLine();
        }
    }
}
