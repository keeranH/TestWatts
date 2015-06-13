using System.ComponentModel;
using System.Configuration.Install;

namespace Econocom.ECSCHANGEService
{
    [RunInstaller(true)]
    public partial class ECSCHANGEInstaller : Installer
    {
        public ECSCHANGEInstaller()
        {
            InitializeComponent();
        }

        private void ECSCHANGEServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void ECSCHANGEServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
