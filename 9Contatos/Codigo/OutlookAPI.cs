using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Identity.Client;
using System.Net.Http;
//using Windows.Data.Json;

namespace _9Contatos.Codigo
{
    internal class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }

    class OutlookAPI
    {
        // The Client ID is used by the application to uniquely identify itself to the v2.0 authentication endpoint.
        public static string[] Scopes = { "https://graph.microsoft.com/Contacts.ReadWrite" };
        public static string TokenForUser = null;
        public static DateTimeOffset Expiration;
        public static ApplicationDataContainer _settings = ApplicationData.Current.RoamingSettings;
        static string clientId = App.Current.Resources["ida:ClientID"].ToString();
        public static PublicClientApplication IdentityClientApp = new PublicClientApplication(clientId);

        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        public static async Task<string> GetTokenForUserAsync()
        {
            AuthenticationResult authResult;
            try
            {
                authResult = await IdentityClientApp.AcquireTokenSilentAsync(Scopes);
                TokenForUser = authResult.Token;
                // save user ID in local storage
                //_settings.Values["userID"] = authResult.User.UniqueId;
                //_settings.Values["userEmail"] = authResult.User.DisplayableId;
                //_settings.Values["userName"] = authResult.User;
            }

            catch (Exception)
            {
                if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    authResult = await IdentityClientApp.AcquireTokenAsync(Scopes);

                    TokenForUser = authResult.Token;
                    Expiration = authResult.ExpiresOn;
                }
            }
            return TokenForUser;
        }


        public static async Task<List<Contato>> Get_Contacts()
        {
            List<Contato> contatos = new List<Contato>();
            if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5)) ;
            else
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage Requisicao;
                var token = await GetTokenForUserAsync();
                int TotalContatos = 0;
                int proximo_grupo = 0;

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token); //não sei o que é mas é requisitado...
#region Recebe contagem de contatos
                Requisicao = await client.GetAsync(new Uri("https://graph.microsoft.com/v1.0/me/contacts/$count"));

                if (!Requisicao.IsSuccessStatusCode)
                {
                    throw new Exception("We could not send the message: " + Requisicao.StatusCode.ToString());
                }
                else
                {
                    if(Requisicao.Content != null)
                    {
                        var responseString = await Requisicao.Content.ReadAsStringAsync();
                        TotalContatos = int.Parse(responseString);
                    }
                }
                #endregion
#region Carrega todos os contatos...
            while (TotalContatos > proximo_grupo)
            {
                    Requisicao = await client.GetAsync(new Uri("https://graph.microsoft.com/v1.0/me/contacts?$skip=" + proximo_grupo.ToString()));

                if (!Requisicao.IsSuccessStatusCode)
                {

                    //                    throw new Exception("We could not send the message: " + response.StatusCode.ToString());
                }
                else
                {
                    if (Requisicao.Content != null)
                    {
                        var responseString = await Requisicao.Content.ReadAsStringAsync();
                            proximo_grupo += 10;
                            //var odata = JsonConvert.DeserializeObject<ODataResponse<Product>>(responseString);


                            //                            JsonArray root = JsonValue.Parse(responseString).GetArray();
                        }
                    }
            }
            #endregion

            }
            return contatos;
        }

        private void Set_Contact(Contato contato)
        {

        }
    }
}
