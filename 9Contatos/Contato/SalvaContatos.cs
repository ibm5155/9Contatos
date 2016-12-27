using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _9Contatos.Contatos.Carrega;
using _9Contatos.API.PeopleAPP;
using _9Contatos.globais;
using Windows.UI.Popups;
using _9Contatos.Interface;

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
                    var pergunta = new MessageDialog("Parece que o sistema negou que seus contatos sejam alterados, isso se deve ao fato do programa não ter sido autorizado pela microsoft a mudar estes dados, esperamos que no futuro isso seja arrumado.");
                    pergunta.Title = "Problemas ao Contactar a api de contatos";
                    pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    await pergunta.ShowAsync();
                }
                Carregando.Incrementa_Barra();
            }
            return Saida;
        }

        public static async Task<bool> Salvar_PeopleAPI_Com_Alteracao()
        {
            bool Saida = true;
            List<string> Telefone_Novo = new List<string>();
            PeopleAPI Link = new PeopleAPI();
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
            }
            return Saida;
        }

        public static async Task<bool> Salvar()
        {
            bool Saida = false;

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
                    break;
            }
            Carregando.Hide();
            var pergunta = new MessageDialog("Os números de seus contatos foram atualizados com sucesso!.");
            pergunta.Title = "Atualização concluida";
            pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
            await pergunta.ShowAsync();
            return Saida;
        }
    }
}
