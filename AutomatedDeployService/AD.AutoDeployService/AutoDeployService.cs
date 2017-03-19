using AD.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AD.AutoDeployService
{
    public class AutoDeployService : ServiceBase
    {
        private Timer autoDeployTimer = null;

        public AutoDeployService()
        {
            //Setup logging
            this.AutoLog = true;
            
            this.ServiceName = "AutoDeployService";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        static void Main()
        {
            ServiceBase.Run(new AutoDeployService());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
                AutomatedDeployment.AutoDeployInit();

                autoDeployTimer = new Timer();
                this.autoDeployTimer.Interval = AutomatedDeployment.PollingDuration;
                this.autoDeployTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.autoDeployTimer_Tick);
                this.autoDeployTimer.Enabled = true;
                this.autoDeployTimer.Start();

                AutomatedDeployment.LogData(DateTime.Now.ToString() + ": Auto Deploy Service Started");

            }
            catch (Exception ex)
            {
                StreamWriter writer = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "\\debug_log.txt");
                writer.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                writer.Close();
            }
            
        }

        private void autoDeployTimer_Tick(object sender, ElapsedEventArgs e)
        {
            AutomatedDeployment.PerformDeployment();
            AutomatedDeployment.LogData("Timer ran");
        }

        protected override void OnStop()
        {
            base.OnStop();
            this.autoDeployTimer.Stop();
            AutomatedDeployment.LogData(DateTime.Now.ToString() + ": Auto Deploy Service Stopped");
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(
                  SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }
    }
}
