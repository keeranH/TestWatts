using System.Configuration;
using System.Threading;

namespace Econocom.ECSCHANGEService
{
    partial class ECSCHANGEService
    {
        /// <summary> 
        /// Variable requise
        /// </summary>
        private System.ComponentModel.IContainer components = null;    

        /// <summary>
        ///  Disposer les ressources
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
            this._ecschangeWatcher = new System.IO.FileSystemWatcher();
            this._importationWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this._ecschangeWatcher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._importationWatcher)).BeginInit();
            // 
            // _ecschangeWatcher
            // 
            this._ecschangeWatcher.EnableRaisingEvents = true;
            this._ecschangeWatcher.IncludeSubdirectories = true;
            this._ecschangeWatcher.Created += new System.IO.FileSystemEventHandler(this.ECSCHANGEWatcher_Created);
            // 
            // _importationWatcher
            // 
            this._importationWatcher.EnableRaisingEvents = true;
            this._importationWatcher.IncludeSubdirectories = true;
            this._importationWatcher.Created += new System.IO.FileSystemEventHandler(this.ImportationWatcher_Created);
            // 
            // ECSCHANGEService
            // 
            this.ServiceName = "Econocom.ECSCHANGEService";
            ((System.ComponentModel.ISupportInitialize)(this._ecschangeWatcher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._importationWatcher)).EndInit();

        }

        #endregion    

    }
}
