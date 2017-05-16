using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace _9Contatos.Store
{
    class License
    {
        public static LicenseInformation LicenseInformation;

        public static void CheckLicense()
        {
#if !DEBUG
            // Initialize the license info for use in the app that is uploaded to the Store.
            // Uncomment the following line in the release version of your app.
            LicenseInformation = CurrentApp.LicenseInformation;
#else
            // Initialize the license info for testing.
            // Comment the following line in the release version of your app.
            LicenseInformation = CurrentAppSimulator.LicenseInformation;
#endif
        }
    }
}
