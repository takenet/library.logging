namespace Takenet.Library.Logging.LogConsumer
{
    /// <summary>
    /// Project installer
    /// </summary>
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logConsumerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.logConsumerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // logConsumerServiceProcessInstaller
            // 
            this.logConsumerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.logConsumerServiceProcessInstaller.Password = null;
            this.logConsumerServiceProcessInstaller.Username = null;
            // 
            // logConsumerServiceInstaller
            // 
            this.logConsumerServiceInstaller.DelayedAutoStart = true;
            this.logConsumerServiceInstaller.Description = "Consume log messages from application queues and send it to the log database";
            this.logConsumerServiceInstaller.DisplayName = "Takenet Log Consumer";
            this.logConsumerServiceInstaller.ServiceName = "LogConsumerService";
            this.logConsumerServiceInstaller.ServicesDependedOn = new string[] {
        "MSMQ"};
            this.logConsumerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.logConsumerServiceProcessInstaller,
            this.logConsumerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller logConsumerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller logConsumerServiceInstaller;
    }
}