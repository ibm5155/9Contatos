using Windows.Networking.Connectivity;

namespace _9Contatos.InternetTools
{
    class Internet
    {
        public static bool CheckInternetConectivity()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool isInternetConnected = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return isInternetConnected;
        }
    }
}
