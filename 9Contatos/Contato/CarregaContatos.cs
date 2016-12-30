using System;
using System.Linq;
using System.Threading.Tasks;
using _9Contatos.Interface;
using _9Contatos.API.PeopleAPP;
using _9Contatos.API.Outlook;
using _9Contatos.Telefones.ParserNonoDigito;
using _9Contatos.Telefones.telefone;
using _9Contatos.globais;
using Windows.UI.Popups;
//test
namespace _9Contatos.Contatos.Carrega
{
    enum QualAPI
    {
        PeopleAPI,
        PeopleAPI_COM_Alteracao,
        OutlookAPI,
        GmailAPI
    }

    class CarregaContatos
    {

        private static async Task<bool> Carrega_PeopleAPI(QualAPI api)
        {
            int Saida = 0;
            bool PodeCarregarOutraPagina = true;
            PeopleAPI PeopleData = new PeopleAPI(); // onde está toda a lógica de get/set com o app Pessoas/People
            ParserNonoDigito ParserNove = new ParserNonoDigito();
            Telefone TelefoneBuffer = new Telefone();

            try
            {
                if(api == QualAPI.PeopleAPI_COM_Alteracao)
                {
                    if (await PeopleData.LoadBuffer(api) == false)
                    {
                        PodeCarregarOutraPagina = false;
                        var pergunta = new MessageDialog("Se você ler essa mensagem, quer dizer que o aplicativo ainda não foi autorizado pela microsoft para alterar diretamente os seus contatos do app pessoas :(");
                        pergunta.Title = "Algo deu errado";
                        pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                        await pergunta.ShowAsync();
                    }
                }
                else
                {
                    PeopleData.LoadBuffer(api); // Carregamos os dados enquanto o usuário digita a região.
//                                                 Com isso,tempos de 3 -  6 segundos de sobra pra fazer o serviço sem que o usuário perceba.
                }
            }
            catch (System.UnauthorizedAccessException)
            {
                // adicionar a seguinte linha no código do Package.appmanifest
                //     <uap:Capability Name="contacts" />
                PodeCarregarOutraPagina = false;
                var pergunta = new MessageDialog("Parece que o desenvolvedor esqueceu de dizer ao sistema que este app precisa ler seus contatos, de uma cutucada nesse desenvolvedor nos comentários da loja para ele arrumar isso!");
                pergunta.Title = "Algo deu errado";
                pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                await pergunta.ShowAsync();
            }
            if (PodeCarregarOutraPagina == true)
            {
                PegaRegiao dialog = new PegaRegiao();
                await dialog.ShowAsync();
//              quando sair daqui, talvez os contatos a Função "LoadBuffer" pode não ter terminado de carregar os contatos
//              então vamos checar se ele terminou, caso contrario, espere ele carregar.
                while (Globais.Contatos_Carregados == false)
                {
                    //só sai do loop quando o app ter finalizado de carregar todos os seus contatos.
                    await System.Threading.Tasks.Task.Delay(200); //dorme por 200ms (0,2 segundos) 
                    //se não botarmos um Delay, o programa vai usar 100% de um core do dispositivo, com um delay de somente 1ms isso cai pra quase 0%
                }
                if (Globais.MinhaRegiao != "")
                {
                    for (int i = 0, fim = PeopleData.TotalContatos(); i < fim; i++)
                    {
                        Globais.contatos.Add(PeopleData.CarregaContato(i));
                    }
                    //Etapa 2: adicionar o nono digito
                    for (int i = 0, fim = Globais.contatos.Count(); i < fim; i++)
                    {

                        for (int j = 0, fim2 = Globais.contatos[i].Telefones_Antigos.Count(); j < fim2; j++)
                        {
                            TelefoneBuffer = new Telefone();
                            TelefoneBuffer.SetTelefone(Globais.contatos[i].Telefones_Antigos[j]);
                            Saida = ParserNove.ChecaNumero(ref TelefoneBuffer, Globais.MinhaRegiao);
                            Globais.contatos[i].Telefones_Formatados.Add(TelefoneBuffer);
                            if (Saida == -2)
                            {
                                Saida = -1;
                            }
                            Globais.contatos[i].NumeroAlterado.Add(Saida);
                            Globais.contatos[i].Flag_Numero_Eh_Servico.Add(Globais.contatos[i].Telefones_Formatados[j].Servico.Count() > 0);
                            Globais.contatos[i].Flag_Recebeu_Nono_Digito.Add(Saida == 1);
                            Globais.contatos[i].Flag_Numero_Eh_Desconhecido.Add(Globais.contatos[i].Telefones_Formatados[j].Numero_Nao_Reconhecido.Count() > 0);
                            Globais.contatos[i].Flago_Numero_Eh_Internacional.Add(Globais.contatos[i].Telefones_Formatados[j].Numero_Internacional.Count() > 0);
                            Globais.contatos[i].Flag_Numero_Alterado.Add(Globais.contatos[i].Telefones_Formatados[j].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais) != Globais.contatos[i].Telefones_Antigos[j]);
                        }
                    }
                }
                else
                {
                    PodeCarregarOutraPagina = false;
                    // a mensagem já é mostrado na janela do xaml
/*                    var pergunta = new MessageDialog("Não encontramos a região requisitada em nossos registros, então para evitar problemas estamos requisitando para que você repita novamente esta operação informando sua região (caso tenha digitado errado)");
                    pergunta.Title = "Hmm que região é essa?";
                    pergunta.Commands.Add(new UICommand { Label = "Entendi chefe", Id = 0 });
                    await pergunta.ShowAsync();
 */
                 }
            }
            return PodeCarregarOutraPagina;
        }

        private static async Task Carrega_OutlookAPI()
        {
            ParserNonoDigito ParserNove = new ParserNonoDigito();
            Telefone TelefoneBuffer = new Telefone();

            await OutlookAPI.Get_Contacts();

        }

        private static void Carrega_GmailAPI()
        {

        }


        public static async Task<bool> Carrega(QualAPI api)
        {
            Globais.Contatos_Carregados = false;
            bool PodeCarregarOutraPagina = true;
            switch(api)
            {
                case QualAPI.PeopleAPI:
                case QualAPI.PeopleAPI_COM_Alteracao:
                    PodeCarregarOutraPagina = await Carrega_PeopleAPI(api);
                    break;
                case QualAPI.OutlookAPI:
                    await Carrega_OutlookAPI();
                    break;
                case QualAPI.GmailAPI:
                    break;
            }
            Globais.Filtrar_Sem_Numero = false;
            Globais.Filtrar_Ocultar_Um_Contato = false;
            Globais.Filtrar_Numero_Alterado = true;
            Globais.Filtrar_Nono_Digito = true;
            Globais.Filtrar_Servico = true;
            Globais.Filtrar_Desconhecido= true;
            Globais.Filtrar_Internacional = true;

            return PodeCarregarOutraPagina;
        }
    }
}
