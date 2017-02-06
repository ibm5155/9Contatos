using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _9Contatos.Contatos.Contato;
using Windows.Storage;
using Microsoft.Identity.Client;
using _9Contatos.Contatos.Carrega;
using System.Net.Http;
//using Windows.Data.Json;
using _9Contatos.globais;

namespace _9Contatos.API.Outlook
{
    /*
     * Bem, é só um caso de testes, por enquanto ele consegue contactar o outlook, conectar a ele e também carregar todos os contatos (em formato json.
     * TODO: 
     * Filtro desses dados para podermos aproveitar.
     * Função para salvar contatos (creio que iremos precisar de algo que crie um pacote e mande para a ms, porque atualizar um por um parece que vai ser
     * mais demorado que o peopleAPI.
     */
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
//                _settings.Values["userID"] = authResult.User.UniqueId;
//                _settings.Values["userEmail"] = authResult.User.DisplayableId;
//                _settings.Values["userName"] = authResult.User;
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


        public static async Task<bool> Get_Contacts()
        {
            Globais.Outlook_contatos = new List<Contatos_Outlook>();
            if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5)) ;
            else
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage Requisicao;
                var token = await OutlookAPI.GetTokenForUserAsync();
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
                CarregaContatos.Carregando.Hide(); // gambiarra
                CarregaContatos.Carregando.ShowAsync(); //Roda em paralelo ao código
                CarregaContatos.Carregando.Altera_Titulo("Carregando Contatos");
                CarregaContatos.Carregando.Altera_Maximo(TotalContatos);
                int i = 0;

            while (TotalContatos > proximo_grupo)
            {
                    Requisicao = await client.GetAsync(new Uri("https://graph.microsoft.com/v1.0/me/contacts?$skip=" + proximo_grupo.ToString())+ "&$select=Id,displayName,homePhones,MobilePhone,businessPhones");

                    if (!Requisicao.IsSuccessStatusCode)
                    {

                        //                    throw new Exception("We could not send the message: " + response.StatusCode.ToString());
                    }
                    else
                    {
                        if (Requisicao.Content != null)
                        {
                            var responseString = await Requisicao.Content.ReadAsStringAsync();
                            Globais.Outlook_contatos.Add(new Contatos_Outlook());
                            Globais.Outlook_contatos[i].Carrega(responseString);
                            proximo_grupo += 10;
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                            CarregaContatos.Carregando.Incrementa_Barra();
                        }
                        i++;
                    }
            }
            #endregion

            }
            Globais.Contatos_Carregados = true;
            return true;
        }

        private void Set_Contact(Contato contato)
        {

        }

        public Contato CarregaContato(int IndiceY, int IndiceX)
        {
            Contato NovoContato = new Contato();

            NovoContato.NomeCompleto = Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].displayName;

            if(Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].mobilePhone != "")
            {
                NovoContato.Telefones_Antigos.Add(Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].mobilePhone);
            }


            for (int i = 0, fim = Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].homePhones.Count(); i < fim; i++)
            {
                NovoContato.Telefones_Antigos.Add(Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].homePhones[i]);
            }

            for (int i = 0, fim = Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].businessPhones.Count(); i < fim; i++)
            {
                NovoContato.Telefones_Antigos.Add(Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].businessPhones[i]);
            }

            NovoContato.ID_OUTLOOK = Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].Id;



            return NovoContato;
        }
    }
}
