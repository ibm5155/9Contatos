﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _9Contatos.Contatos.Contato;
using Windows.Storage;
using Microsoft.Identity.Client;
using _9Contatos.Contatos.Carrega;
using System.Net.Http;
//using Windows.Data.Json;
using _9Contatos.globais;
using _9Contatos.InternetTools;
using System.Threading;
using System.Diagnostics;
using _9Contatos.Contatos.Salvar;
using System.Reflection;

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
        public static string TokenForUser = null;
        public static DateTimeOffset Expiration;
//        public static ApplicationDataContainer _settings = ApplicationData.Current.RoamingSettings;
        


        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        public static async Task<string> GetTokenForUserAsync()
        {
            if (TokenForUser != null)
            {
                /*
                 * Se já temos o Token então não precisamos checa-lo.
                 */
            }
            else
            {
                //Adquire o Token
                string[] Scopes = { "https://graph.microsoft.com/Contacts.ReadWrite" };
                string clientId = App.Current.Resources["ida:ClientID"].ToString();
                PublicClientApplication IdentityClientApp = new PublicClientApplication(clientId);
                AuthenticationResult authResult = await IdentityClientApp.AcquireTokenAsync(Scopes);
                TokenForUser = authResult.Token;
                Expiration = authResult.ExpiresOn;
                authResult = null;
                IdentityClientApp = null;
                clientId = null;
            }
            return TokenForUser;
        }

        public static async Task<bool> Get_Contacts()
        {
            Globais.Outlook_contatos = new List<Contatos_Outlook>();
            await GetTokenForUserAsync(); //pega token
            CancellationTokenSource CancelaTaskGetURL;// usado para matar a TaskGetURL quando há uma falha.
            Task<HttpResponseMessage> TaskGetURL;
            if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5))
            {
                Globais.Contatos_Carregados = false;
                return false;
            }
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
                    if (Requisicao.Content != null)
                    {
                        var responseString = await Requisicao.Content.ReadAsStringAsync();
                        TotalContatos = int.Parse(responseString);
                    }
                }
                #endregion
                #region Carrega todos os contatos...
                CarregaContatos.Carregando.Hide(); // gambiarra
                CarregaContatos.Carregando.ShowAsync(); //Roda em paralelo ao código
                int i = 0;
                CarregaContatos.Carregando.RemoveErro();

                while (TotalContatos > proximo_grupo)
                {
                    if (Internet.CheckInternetConectivity() == true)
                    {
                        CancelaTaskGetURL = new CancellationTokenSource();

                        //Só iremos continuar carregando caso tenha conexão com a internet
                        TaskGetURL = client.GetAsync(new Uri("https://graph.microsoft.com/v1.0/me/contacts?$skip=" + proximo_grupo.ToString()) + "&$select=Id,displayName,homePhones,MobilePhone,businessPhones", CancelaTaskGetURL.Token);
                        //aguarda a task ser finalizada
                        while (Internet.CheckInternetConectivity() == true && TaskGetURL.IsCompleted == false)
                        {
                            await System.Threading.Tasks.Task.Delay(20); // espera por 20ms
                        }
                        //Requisicao = await client.GetAsync(new Uri("https://graph.microsoft.com/v1.0/me/contacts?$skip=" + proximo_grupo.ToString()) + "&$select=Id,displayName,homePhones,MobilePhone,businessPhones");
                        //client.GetAsync tem um erro que se a conexão cair durante a sua execução, o programa vai ficar em loop.
                        //se ela cair no inicio da execução ela vai dar uma Exception System.Net.Http.HttpRequestException
                        if (Internet.CheckInternetConectivity() == false)
                        {
                            //cancela a requisição pois houve um problema durante o recebimento da task
                            CancelaTaskGetURL.Cancel();
                            CancelaTaskGetURL.Dispose();
                            await System.Threading.Tasks.Task.Delay(200); // espera por 20ms
                        }
                        else
                        {
                            if (TaskGetURL.IsCompleted == true)
                            {
                                Requisicao = TaskGetURL.Result;
                            }
                        }
                        Debug.WriteLine("Part " + i.ToString() + " Completed:" + TaskGetURL.IsCompleted.ToString()
                                        + " Faulted:" + TaskGetURL.IsFaulted.ToString()
                                        + " Canceled:" + TaskGetURL.IsCanceled.ToString()
                                        + " Status:" + TaskGetURL.Status.ToString()
                        );
                        if (!Requisicao.IsSuccessStatusCode || TaskGetURL.IsCompleted == false || TaskGetURL.IsCanceled == true || Internet.CheckInternetConectivity() == false)
                        {
                            //não faz nada, assim iremos percorrer o while novamente e iremos cair no caso de sem internet.
                        }
                        else
                        {
                            if (Requisicao.Content != null)
                            {
                                var responseString = await Requisicao.Content.ReadAsStringAsync();
                                Globais.Outlook_contatos.Add(new Contatos_Outlook());
                                Globais.Outlook_contatos[i].Carrega(responseString);
                                proximo_grupo += 10;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        CarregaContatos.Carregando.AdicionaErro("ms-appx:///Assets/NoInternet.png", "Aplicativo perdeu a conexão com a internet, por favor verifique sua conexão.");
                        //aguardamos a conexão em um loop
                        while (Internet.CheckInternetConectivity() == false && CarregaContatos.Carregando.Fechar == false)
                        {
                            await System.Threading.Tasks.Task.Delay(200); // espera por 20ms
                            CarregaContatos.Carregando.IncrementaTimeout(200);
                        }
                        if (CarregaContatos.Carregando.Fechar == true)
                        {
                            return false;
                        }
                        CarregaContatos.Carregando.RemoveErro();
                    }
                }
                #endregion
            }
            Globais.Contatos_Carregados = true;
            return true;
        }


        public Contato CarregaContato(int IndiceY, int IndiceX)
        {
            Contato NovoContato = new Contato();

            NovoContato.NomeCompleto = Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].displayName;

            if (Globais.Outlook_contatos[IndiceY].Contatos[IndiceX].mobilePhone != "")
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

            NovoContato.ID_OUTLOOK = Globais.Outlook_contatos[IndiceY].Contatos[IndiceX];



            return NovoContato;
        }

        public async Task<bool> PATCH_Contact(Contato contato, string NewMobilePhone, List<string> NewbusinessPhones, List<string> NewhomePhones)
        {
            /**
             * Curioso não ter um caso pronto de PATCH na estrutura do httpclient 
             */
            CancellationTokenSource CancelaTaskSetURL;// usado para matar a TaskGetURL quando há uma falha.
            Task<HttpResponseMessage> TaskSetURL;
            HttpResponseMessage response = new HttpResponseMessage();
            bool Status = true;
            if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5)) ;
            else
            {
                HttpClient client = new HttpClient();
                StringContent MensagemDadosAlterados;
                string content = "";
                #region MENSAGEM de atualização
                //uma mensagem Json lindamente formatada a mão

                content = "{ " + '"' + "MobilePhone" + '"' + " : " + '"' + NewMobilePhone + '"';

                content = content + ", " + '"' + "homePhones" + '"' + " : [ ";
                for (int i = 0; i < NewhomePhones.Count(); i++)
                {
                    content = content + '"' + NewhomePhones[i] + '"';
                    if (i < NewhomePhones.Count() - 1)
                    {
                        content = content + ", ";
                    }
                }
                content = content + " ], " + '"' + "businessPhones" + '"' + " : [";
                for (int i = 0; i < NewbusinessPhones.Count(); i++)
                {
                    content = content + '"' + NewbusinessPhones[i] + '"';
                    if (i < NewbusinessPhones.Count() - 1)
                    {
                        content = content + ", ";
                    }
                }
                content = content + " ] }";

                MensagemDadosAlterados = new StringContent(content);
                MensagemDadosAlterados.Headers.ContentType.MediaType = "application/json";
                #endregion




                #region Enviar MENSAGEM
                HttpRequestMessage Requisicao;/* = new HttpRequestMessage(new HttpMethod("PATCH"), new Uri("https://graph.microsoft.com/v1.0/me/contacts/" + contato.ID_OUTLOOK.Id + "/"))
                {
                    Content = MensagemDadosAlterados
                };*/
                var token = await OutlookAPI.GetTokenForUserAsync();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token); //não sei o que é mas é requisitado...
                Status = false;
                do
                {
                    if (Internet.CheckInternetConectivity() == true)
                    {
                        CancelaTaskSetURL = new CancellationTokenSource();
                        /* O HttpRequestMessage odeia ter que enviar uma Request mais de uma vez,
                         * se você tentar fazer isso, ele vai criar um exception (System.InvalidOperationException)
                         * Para contornar isso a gente faz uma função que sempre envia uma request clonada,
                         * assim toda vez que é chamado a RequisicaoClonada() é criado uma copia nova da Requisicao.
                         * É uma pena ter que fazer isso, pois se cair a internet no meio do processo você tem chances
                         * de perder a mensagem e o programa achar que você enviou a mensagem e como sou um programador de elite
                         * eu preciso burlar a matrix para garantir que essa bendita mensagem seja enviada sem falhas de 
                         * internet no caminho.
                        */
                        Requisicao = new HttpRequestMessage(new HttpMethod("PATCH"), new Uri("https://graph.microsoft.com/v1.0/me/contacts/" + contato.ID_OUTLOOK.Id + "/"))
                        {
                            Content = MensagemDadosAlterados
                        };
                        TaskSetURL = client.SendAsync(Requisicao, CancelaTaskSetURL.Token);
                        while (Internet.CheckInternetConectivity() == true && TaskSetURL.IsCompleted == false)
                        {
                            await System.Threading.Tasks.Task.Delay(20); // espera por 20ms
                        }
                        if (Internet.CheckInternetConectivity() == false)
                        {
                            //cancela a requisição pois houve um problema durante o recebimento da task
                            CancelaTaskSetURL.Cancel();
                            CancelaTaskSetURL.Dispose();
                            Requisicao.Dispose();
                            Requisicao = null;
                            await System.Threading.Tasks.Task.Delay(200); // espera por 20ms
                        }
                        else /*Internet.CheckInternetConectivity() == true*/
                        {
                            response = TaskSetURL.Result;
                            Status = true;
                        }
                    }
                    else
                    {
                        SalvaContatos.Carregando.AdicionaErro("ms-appx:///Assets/NoInternet.png", "Aplicativo perdeu a conexão com a internet, por favor verifique sua conexão.");
                        //aguardamos a conexão em um loop
                        while (Internet.CheckInternetConectivity() == false && SalvaContatos.Carregando.Fechar == false)
                        {
                            await System.Threading.Tasks.Task.Delay(200); // espera por 20ms
                            SalvaContatos.Carregando.IncrementaTimeout(200);
                        }
                        if (SalvaContatos.Carregando.Fechar == true)
                        {
                            return false;
                        }
                        SalvaContatos.Carregando.RemoveErro();
                    }
                } while (Status == false);

                //                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                #endregion
                #region checa retorno se foi validado

                if (response.StatusCode == System.Net.HttpStatusCode.UnsupportedMediaType)
                {
                    Status = false;
                    // formatação errada.
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    Status = false;
                    // Chamada errado
                }

                #endregion
                /*
                #region GRAPH CODE
                                HttpRequestMessage authResult = new HttpRequestMessage();
                                authResult.Headers.Add("Authorization", "Bearer " + token);

                                var cliente = new GraphServiceClient(authResult);
                                var orgRequest = cliente.Me.Contacts[contato.ID_OUTLOOK.Id].Request();
                                var org = orgRequest.Select(contato.ID_OUTLOOK.Id).GetAsync().Result;
                                org.BusinessPhones = NewbusinessPhones;
                                org.HomePhones = NewhomePhones;
                                org.MobilePhone = NewMobilePhone;

                                orgRequest.UpdateAsync(org).Wait();
                                #endregion
                                */
            }

            /* REQUEST BODY:
             * {
             *   "homePhones" : [
             *      "+3 777 777 7777"
             *      ],
             *   "MobilePhone" : "...",
             *   "businessPhones : [] //in case of void
             * }  
             */

            /* Respostas --- id invalido
            cache-control: private
            content-type: application/json
            request-id: 0921f6ca-e7c6-4a7b-b4cb-65b946c73993
            client-request-id: 0921f6ca-e7c6-4a7b-b4cb-65b946c73993
            Status Code: 400
            {
            "error": {
            "code": "ErrorInvalidIdMalformed",
            "message": "Id is malformed.",
            "innerError": {
            "request-id": "0921f6ca-e7c6-4a7b-b4cb-65b946c73993",
            "date": "2017-02-07T13:47:14"
            }
            }
            }             * */

            /* RESPOSTA CORRETA: retorna o contato na integra
            cache-control: private
            content-type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
            etag: W/"EQAAABYAAAA2tDFF5C6uQIKQtImL3KseAAL5s5Sf"
            request-id: 6564e88c-a7cf-406f-95eb-3cad3e37c48f
            client-request-id: 6564e88c-a7cf-406f-95eb-3cad3e37c48f
            Status Code: 200
            {
            "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users('62657533-a3a3-49dc-9655-f2bfd93f2767')/contacts/$entity",
            "@odata.etag": "W/\"EQAAABYAAAA2tDFF5C6uQIKQtImL3KseAAL5s5Sf\"",
            "id": "AAMkAGVmMDc3YjMyLWQzYmYtNDhjNy1hNWE2LTYxZWE3YjZiY2Y0NwBGAAAAAAB-G3fldksVTJEtahBgGpCFBwAtNrImCnjbQoZ_tj_QRKx0AAAAR1jtAAA2tDFF5C6uQIKQtImL3KseAAL5gyI0AAA=",
            "createdDateTime": "2017-02-07T13:38:33Z",
            "lastModifiedDateTime": "2017-02-07T13:48:05Z",
            "changeKey": "EQAAABYAAAA2tDFF5C6uQIKQtImL3KseAAL5s5Sf",
            "categories": [],
            "parentFolderId": "AAMkAGVmMDc3YjMyLWQzYmYtNDhjNy1hNWE2LTYxZWE3YjZiY2Y0NwAuAAAAAAB-G3fldksVTJEtahBgGpCFAQAtNrImCnjbQoZ_tj_QRKx0AAAAR1jtAAA=",
            "birthday": null,
            "fileAs": "",
            "displayName": "Pavel Bansky",
            "givenName": "Pavel",
            "initials": null,
            "middleName": null,
            "nickName": null,
            "surname": "Bansky",
            "title": null,
            "yomiGivenName": null,
            "yomiSurname": null,
            "yomiCompanyName": null,
            "generation": null,
            "imAddresses": [],
            "jobTitle": null,
            "companyName": null,
            "department": null,
            "officeLocation": null,
            "profession": null,
            "businessHomePage": null,
            "assistantName": null,
            "manager": null,
            "homePhones": [
            "+3 777 777 7777"
            ],
            "mobilePhone": "(41) 5555-5555",
            "businessPhones": [],
            "spouseName": null,
            "personalNotes": null,
            "children": [],
            "emailAddresses": [
            {
            "name": "Pavel Bansky",
            "address": "pavelb@fabrikam.onmicrosoft.com"
            }
            ],
            "homeAddress": {},
            "businessAddress": {},
            "otherAddress": {}
            }
            */
            return Status;
        }
    }

}
