namespace Econocom.ECSCHANGEService
{
    partial class ECSCHANGEInstaller
    {
        /// <summary>
        /// Variable requise
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Disposer les ressources
        /// </summary>
        /// <param name="disposing">vrai si les ressources gérées doivent être disposées;sinon faux.</param>
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
        /// Méthode requise
        /// ne pas modifier le contenu de cette méthode avec le rédacteur(l'éditeur) de code
        /// </summary>
        private void InitializeComponent()
        {
            this.ECSCHANGEServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.ECSCHANGEServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // ECSCHANGEServiceInstaller
            // 
            this.ECSCHANGEServiceInstaller.ServiceName = "Econocom.ECSCHANGEService";
            this.ECSCHANGEServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.ECSCHANGEServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.ECSCHANGEServiceInstaller_AfterInstall);
            // 
            // ECSCHANGEServiceProcessInstaller
            // 
            this.ECSCHANGEServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ECSCHANGEServiceProcessInstaller.Password = null;
            this.ECSCHANGEServiceProcessInstaller.Username = null;
            this.ECSCHANGEServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.ECSCHANGEServiceProcessInstaller_AfterInstall);
            // 
            // ECSCHANGEInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ECSCHANGEServiceInstaller,
            this.ECSCHANGEServiceProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller ECSCHANGEServiceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller ECSCHANGEServiceProcessInstaller;
    }
}