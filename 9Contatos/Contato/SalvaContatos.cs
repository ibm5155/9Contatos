using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _9Contatos.Contatos.Carrega;
using _9Contatos.API.PeopleAPP;
using _9Contatos.API.Outlook;
using _9Contatos.globais;
using Windows.UI.Popups;
using _9Contatos.Interface;
using System.Diagnostics;

namespace _9Contatos.Contatos.Salvar
{
    class SalvaContatos
    {
        private static Janela_Carregando Carregando;

        public static async Task<bool> Salvar_PeopleAPI_Com_LINK()
        {
            bool Saida = true;
            List<string> Telefone_Novo = new List<string>();
            PeopleAPI Link = new PeopleAPI();
            Carregando.Altera_Maximo(Globais.contatos.Count);
            for (int i = 0; Saida == true && i < Globais.contatos.Count; i++)
            {
                Debug.Write("Salvando o contato " + Globais.contatos[i].NomeCompleto);
                Telefone_Novo.Clear();
                for (int j = 0; j < Globais.contatos[i].Telefones_Formatados.Count; j++)
                {
                    Telefone_Novo.Add(Globais.contatos[i].Telefones_Formatados[j].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD,Globais.Formatacao_Ocultar_Pais));

                }
                try
                {
                    await Link.AlterarContato_Link(Globais.contatos[i].ID, Globais.contatos[i].NomeCompleto, Telefone_Novo, Globais.contatos[i].Telefones_Antigos);
                }
                catch (System.UnauthorizedAccessException) //não é para acontecer isso, mas vai que...
                {
                    Saida = false;
                    Carregando.Hide()
                    var pergunta = new MessageDialog("Parece que o sistema negou que seus contatos sejam alterados, isso se deve ao fato do programa não ter sido autorizado pela microsoft a mudar estes dados, esperamos que no futuro isso seja arrumado.");
                    pergunta.Title = "Problemas ao Contactar a api de contatos";
                    pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    await pergunta.ShowAsync();
                }
                Debug.WriteLine(" - SALVO");
                Carregando.Incrementa_Barra();
            }
            return Saida;
        }

        public static async Task<bool> Salvar_PeopleAPI_Com_Alteracao()
        {
            bool Saida = true;
            List<string> Telefone_Novo = new List<string>();
            PeopleAPI Link = new PeopleAPI();
            Carregando.Altera_Maximo(Globais.contatos.Count);
            for (int i = 0; Saida == true && i < Globais.contatos.Count; i++)
            {
                Telefone_Novo.Clear();
                for (int j = 0; j < Globais.contatos[i].Telefones_Formatados.Count; j++)
                {
                    Telefone_Novo.Add(Globais.contatos[i].Telefones_Formatados[j].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais));
                }
                try
                {
                    await Link.AlterarContato(Globais.contatos[i].ID, Globais.contatos[i].NomeCompleto, Telefone_Novo, Globais.contatos[i].Telefones_Antigos);
                }
                catch (System.UnauthorizedAccessException)
                {
                    Saida = false;
                    var pergunta = new MessageDialog("Parece que o sistema negou que seus contatos sejam alterados, isso se deve ao fato do programa não ter sido autorizado pela microsoft a mudar estes dados, esperamos que no futuro isso seja arrumado.");
                    pergunta.Title = "Problemas ao Contactar a api de contatos";
                    pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    await pergunta.ShowAsync();
                }
                Carregando.Incrementa_Barra();
            }
            return Saida;
        }

        public static async Task<bool> Salvar_OutlookAPI()
        {
            #region variaveis
            bool TodosContatosSalvos = true;
            List<string> New_Business_Phones = new List<string>();
            List<string> New_Home_Phones = new List<string>();
            bool Carrega_Business_Phones = true;
            string New_MobilePhone;
            OutlookAPI Link = new OutlookAPI();
            #endregion
            #region código
            Carregando.Altera_Maximo(Globais.contatos.Count);
            for (int i = 0; TodosContatosSalvos == true && i < Globais.contatos.Count; i++)
            {
                Debug.Write("Salvando o contato " + Globais.contatos[i].NomeCompleto);
                New_Business_Phones.Clear();
                New_Home_Phones.Clear();
                if (Globais.contatos[i].Telefones_Formatados.Count > 0)
                {
                    New_MobilePhone = Globais.contatos[i].Telefones_Formatados[0].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais);
                }
                else
                {
                    New_MobilePhone = "";
                }
                Carrega_Business_Phones = true;
                for (int j = 1; j < Globais.contatos[i].Telefones_Formatados.Count; j++) 
                    //- O primeiro número formatado vem do MobilePhone
                    //- Nesse for filtramos os dados que estavam em uma lista unificada, separando cada número formatado em suas devidas classes.
                {                
                    if(Globais.contatos[i].ID_OUTLOOK.businessPhones.Count == i)
                    {
                        // chegamos ao último número de business
                        Carrega_Business_Phones = false;
                    }

                    if(Carrega_Business_Phones == true)
                    {
                        New_Business_Phones.Add(Globais.contatos[i].Telefones_Formatados[j].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais));
                    }
                    else
                    {
                        New_Home_Phones.Add(Globais.contatos[i].Telefones_Formatados[j].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais));
                    }
                }
                try
                {
                    await Link.PATCH_Contact(Globais.contatos[i], New_MobilePhone, New_Business_Phones, New_Home_Phones);
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    Debug.Write(" <PROBLEMA> ");
                    Carregando.Hide();
                    TodosContatosSalvos = false;
                    var pergunta = new MessageDialog("Não podemos conectar ao serviço da Microsoft. Verfique sua conexão de rede ou tente novamente mais tarde.");
                    pergunta.Title = "Problemas em contactar o servidor da microsoft.";
                    pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    await pergunta.ShowAsync();
                }
                Debug.WriteLine(" - SALVO");

                Carregando.Incrementa_Barra();
            }
            #endregion
            return TodosContatosSalvos;
        }


        public static async Task<bool> Salvar()
        {
            bool Saida = false;
            Debug.WriteLine("Iniciando processo de salvar contatos com a API " + Globais.api_usada);
            Carregando = new Janela_Carregando();
            Carregando.ShowAsync(); //Roda em paralelo ao código

            switch (Globais.api_usada)
            {
                case QualAPI.PeopleAPI:
                    Saida = await Salvar_PeopleAPI_Com_LINK();
                    break;
                case QualAPI.PeopleAPI_COM_Alteracao:
                    Saida = await Salvar_PeopleAPI_Com_Alteracao();
                    break;
                case QualAPI.OutlookAPI:
                    Saida = await Salvar_OutlookAPI();
                    break;
            }
            Carregando.Hide();
            Debug.WriteLine("Fim do Processo de alteracao de contatos");
            if (Saida == true)
            {
                MessageDialog pergunta;
                if (Globais.api_usada == QualAPI.OutlookAPI)
                {
                    pergunta = new MessageDialog("Os números de seus contatos foram atualizados com sucesso!." + '\n' + "vai alguns minutos para você ver as atualizações na sua conta");
                }
                else
                {
                    pergunta = new MessageDialog("Os números de seus contatos foram atualizados com sucesso!.");
                }
               
                pergunta.Title = "Atualização concluida";
                pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                await pergunta.ShowAsync();
            }
            return Saida;
        }
    }
}
