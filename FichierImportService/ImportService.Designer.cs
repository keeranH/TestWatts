using System.Configuration;

namespace FichierImportService
{
    partial class ImportService
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
            string LienFichiers = ServiceUtility.ConfigFichiers;
            string LienInitial = ServiceUtility.ConfigInitial;
            
            this._ecschangeWatcher = new System.IO.FileSystemWatcher();
            this._importationWatcher = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this._ecschangeWatcher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._importationWatcher)).BeginInit();
            // 
            // _ecschangeWatcher
            // 
            this._ecschangeWatcher.EnableRaisingEvents = true;
            this._ecschangeWatcher.Path = LienFichiers;//"E:\\WattsFiles\\FICHIERS";
            this._ecschangeWatcher.Created += new System.IO.FileSystemEventHandler(this._ecschangeWatcher_Created);
            // 
            // _importationWatcher
            // 
            this._importationWatcher.EnableRaisingEvents = true;
            this._importationWatcher.Path = LienInitial;//"E:\\WattsFiles\\FICHIER_INITIAL";
            this._importationWatcher.Created += new System.IO.FileSystemEventHandler(this._importationWatcher_Created);
            // 
            // ImportService
            // 
            this.ServiceName = "ImportService";
            ((System.ComponentModel.ISupportInitialize)(this._ecschangeWatcher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._importationWatcher)).EndInit();

        }

        #endregion

        public System.IO.FileSystemWatcher _ecschangeWatcher;
        private System.IO.FileSystemWatcher _importationWatcher;


    }
}
