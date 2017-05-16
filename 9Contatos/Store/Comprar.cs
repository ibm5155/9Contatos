using System;
using System.Collections.Generic;
using System.Linq;
using _9Contatos.Store;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace _9Contatos.Store
{
    class Comprar
    {
        public static async void ComprarDoacao(string Nome)
        {
            if (Nome.Count() > 0 && License.LicenseInformation != null)
            {
                if (!License.LicenseInformation.ProductLicenses[Nome].IsActive)
                {
                    try
                    {
                        // The customer doesn't own this feature, so
                        // show the purchase dialog.
#if DEBUG
                        await CurrentAppSimulator.RequestProductPurchaseAsync(Nome, false);
#else
                        await CurrentApp.RequestProductPurchaseAsync(Nome, false);
#endif

                        //Check the license state to determine if the in-app purchase was successful.
                    }
                    catch (Exception)
                    {
                        // The in-app purchase was not completed because
                        // an error occurred.
                    }
                }
                else
                {
                    // The customer already owns this feature.
                }
            }
        }
    }
}
