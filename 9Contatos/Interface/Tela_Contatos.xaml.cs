using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using _9Contatos.globais;
using _9Contatos.Contatos.ModeloLista;
using _9Contatos.Telefones.telefone;
using _9Contatos.Contatos.Salvar;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{

    enum Icones
    {
        OK,
        NonoDigito,
        Duvida,
        Internacional
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TelaContatos : Page
    {
        private List<Contatos_ModeloLista> ListadeContatos = new List<Contatos_ModeloLista>();
        private int[] Indice_Filtro;

        public TelaContatos()
        {
            this.InitializeComponent();
            Indice_Filtro = new int[Globais.contatos.Count];
            AtualizaLista();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                // TODO: Go back to the previous page
            };
            Mostra_Dialogo();

        }

        private async void Mostra_Dialogo()
        {
            if (Globais.Aviso_Abrir_Contatos == true)
            {
                Globais.Aviso_Abrir_Contatos = false;
                var dialog = new Popup_Explicacao_Contatos();
                await dialog.ShowAsync();
            }
        }

        private void Popula_Listview()
        {

        }

        private void Botao_Exportar_CSV_Click(object sender, RoutedEventArgs e)
        {
            e = null;

        }

        private void ListaContatosView_ItemClick(object sender, ItemClickEventArgs e)
        {
            e = null;

        }

        private async void ListaContatosView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListView lista = (ListView)sender;
            int indice = lista.SelectedIndex;
            if (indice >= 0)
            {
                indice = Indice_Filtro[indice];
                if (Globais.contatos[indice].Telefones_Antigos.Count > 1)
                {
                    Globais.Indice_Contato_Completo = indice;
                    this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos_Completo));
                }
                else
                {
                    string Tmp_NomeCompleto = Globais.contatos[indice].NomeCompleto;

                    if(Globais.contatos[indice].Telefones_Antigos.Count == 0)
                    {//Temos algo aqui?
                        Globais.contatos[indice].Telefones_Antigos.Add("");
                        Globais.contatos[indice].Telefones_Formatados.Add(new Telefone());
                        Globais.contatos[indice].NumeroAlterado.Add(0);
                    }
                    var dialog = new Janela_EditaContato(Tmp_NomeCompleto, Globais.contatos[indice].Telefones_Antigos[0]);
                    await dialog.ShowAsync();
                    string result = dialog.Result;
                    if (result != "X")
                    {
                        Globais.contatos[indice].Telefones_Antigos[0] = result;
                        Globais.contatos[indice].Telefones_Formatados[0].SetTelefone(result);
                        Globais.contatos[indice].NumeroAlterado[0] = 0;
//                        ListadeContatos[indice].Nome = Tmp_NomeCompleto = Globais.contatos[indice].Telefones_Formatados[0].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia);
//                        ListadeContatos[indice].TelefoneNovo = result;
                        AtualizaLista();
                    }
                    else
                    {
                        if (Globais.contatos[indice].Telefones_Antigos.Count == 1 && Globais.contatos[indice].Telefones_Antigos[0] == "")
                        {
                            Globais.contatos[indice].Telefones_Antigos.Clear();
                            Globais.contatos[indice].Telefones_Formatados.Clear();
                            Globais.contatos[indice].NumeroAlterado.Clear();

                        }
                    }
                }
            }
            e = null;
        }

        private void AtualizaLista()
        {
            bool Adiciona = true;
            int i = 0, k = 0;
            int ID_Primeiro = -1; // é o endereço inicial da lista de números do cliente, assim o primeiro número a ser mostrado será o primeiro número filtrado
            Icones Icone = Icones.OK;
            string NomeCompleto;
            string Situacao = "";
            string sTelefoneNovo = "";
            string sTelefoneAntigo = "";
            string sTelefoneNotas = "";
            int SituacaoPrioridade = 0;
            ListadeContatos.Clear();
            for (i = 0; i < Globais.contatos.Count; i++)
            {
                Adiciona = true;
                ID_Primeiro = -1; // remove filtro

                SituacaoPrioridade = 0;
                NomeCompleto = Globais.contatos[i].NomeCompleto;
                if (Globais.contatos[i].NumeroAlterado.Count() == 0) //contato vazio
                {
                    Situacao = "ms-appx:///Assets/ok.png";
                    sTelefoneAntigo = "Nenhum número Encontrado";
                    sTelefoneNovo = "";
                    sTelefoneNotas = "";
                    Adiciona = Globais.Filtrar_Sem_Numero/* & Globais.Filtrar_OK*/;
                }
                else //contato com um ou mais números
                {
                    Icone = Icones.OK;
                    for (int j = 0; j < Globais.contatos[i].NumeroAlterado.Count; j++) //verificamos todos os números e ordenamos qual o icone deverá ser mostrado.
                    {
                        /* === ORDEM DE VISUALIZAÇÃO
                         *  OK
                         *  NONO DIGITO
                         *  INTERNACIONAL/DESCONHECIDO
                         */
                        #region Filtra os Icones e o primeiro contato a ser mostrado 
                        if (Globais.contatos[i].Flag_Numero_Eh_Desconhecido[j] == true)
                        {
                            Icone = Icones.Duvida;
                        }
                        else if(Globais.contatos[i].Flag_Recebeu_Nono_Digito[j] == true)
                        {                            
                            if(Icone != Icones.Duvida)
                            {
                                Icone = Icones.NonoDigito;
                            }
                        }
                        else if(Globais.contatos[i].Telefones_Formatados[j].Numero_Internacional.Count() > 0)
                        {
                            if (Icone != Icones.Duvida && Icone != Icones.NonoDigito)
                            {
                                Icone = Icones.Internacional;
                            }
                        }
                        else
                        {
                            if (Icone != Icones.OK)
                            {
                                Icone = Icones.OK;
                            }
                        }
                        #endregion

                    }
                    //ESCOLHE O ICONE A SER UTILIZADO
                    if (Icone == Icones.NonoDigito)
                    {
                        Situacao = "ms-appx:///Assets/nono.png";
                    }
                    else if (Icone == Icones.OK)
                    {
                        Situacao = "ms-appx:///Assets/ok.png";
                    }
                    else if(Icone == Icones.Internacional)
                    {
                        Situacao = "ms-appx:///Assets/internacional.png";
                    }
                    else
                    {
                        Situacao = "ms-appx:///Assets/duvida.png";

                    }
                    Adiciona = false;
                    if (Globais.contatos[i].Flag_Numero_Eh_Desconhecido[0] == true && Globais.Filtrar_Desconhecido == true)
                    {
                            Adiciona = true;
                    }
                    else if (Globais.contatos[i].Flago_Numero_Eh_Internacional[0] == true && Globais.Filtrar_Internacional == true)
                    {
                        Adiciona = true;
                    }
                    else if (Globais.contatos[i].Flag_Numero_Alterado[0] == true && Globais.Filtrar_Numero_Alterado == true)
                    {   // true false
                        // false true
                        // nesse caso só oculta quando o usuário desejar ver somente os números alterados
                        // e o número em questão não foi alterado.
                        if (Globais.contatos[i].Flag_Recebeu_Nono_Digito[0] == true && Globais.Filtrar_Nono_Digito == false)
                        {
                            Adiciona = false;
                        }
                        else
                        {
                            Adiciona = true;
                        }
                    }
                    else if (Globais.contatos[i].Flag_Numero_Alterado[0] == false && Globais.Filtrar_Numero_Nao_Alterado == true)
                    {   // true false
                        // false true
                        // nesse caso só oculta quando o usuário desejar ver somente os números alterados
                        // e o número em questão não foi alterado.
                        if (Globais.contatos[i].Flag_Numero_Eh_Servico[0] == true && Globais.Filtrar_Servico == false)
                        {
                            Adiciona = false;
                        }
                        else if (Globais.contatos[i].Flag_Numero_Eh_Desconhecido[0] == true && Globais.Filtrar_Desconhecido == false)
                        {
                            Adiciona = false;
                        }
                        else if (Globais.contatos[i].Flago_Numero_Eh_Internacional[0] == true && Globais.Filtrar_Internacional== false)
                        {
                            Adiciona = false;
                        }
                        else
                        {
                            Adiciona = true;
                        }
                    }
                    else if (Globais.contatos[i].Flag_Numero_Eh_Servico[0] == true && Globais.Filtrar_Servico == true)
                    {
                        Adiciona = true;
                    }
                    else if (Globais.contatos[i].Flag_Recebeu_Nono_Digito[0] == true && Globais.Filtrar_Nono_Digito == true)
                    {
                        Adiciona = true;
                    }
                    /*if (Globais.contatos[i].NumeroAlterado.Count() == 1 && Globais.Filtrar_Ocultar_Um_Contato == false)
                    {
                        Adiciona = false;
                    }*/
 

                    sTelefoneNovo = Globais.contatos[i].Telefones_Formatados[0].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais);
                    sTelefoneAntigo = Globais.contatos[i].Telefones_Antigos[0];
                    sTelefoneNotas = "";
                    if (sTelefoneAntigo != sTelefoneNovo)
                    {
                        sTelefoneNotas = "↴"; 
                    }
                    else
                    {
                        sTelefoneNovo = "";
                    }
                    if (Globais.contatos[i].Telefones_Antigos.Count() > 1)
                    {
                        sTelefoneNotas += " (+" + (Globais.contatos[i].Telefones_Antigos.Count() - 1) + " Número" + ((Globais.contatos[i].Telefones_Antigos.Count() - 1) == 1 ? "" : "s") +  ")";
                    }
                }

                if (Adiciona == true)
                {
                    Indice_Filtro[k] = i;
                    k++;
                    ListadeContatos.Add(new Contatos_ModeloLista { Nome = NomeCompleto, ImagePath = Situacao, TelefoneNovo = sTelefoneNovo, TelefoneAntigo = sTelefoneAntigo,TelefoneNotas = sTelefoneNotas });
                }
            }
            //            ListaContatosView.Add(new Contatos_ModeloLista { })
            ListaContatosView.ItemsSource = null;
            ListaContatosView.ItemsSource = ListadeContatos;
            Titulo.Text = "Arruma Contatos  - mostrando " + ListadeContatos.Count.ToString() + " contatos";
        }

        private async void Bt_Salvar_Click(object sender, RoutedEventArgs e)
        {
            if (await SalvaContatos.Salvar() == true)
                this.Frame.GoBack();
            e = null;
        }

        private void Bt_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            // Botar popup confirmando
            ListadeContatos.Clear();
            ListaContatosView.ItemsSource = null;
            ListaContatosView = null;

            Globais.contatos.Clear();
            this.Frame.GoBack();
            e = null;
        }

        private async void Bt_Filtrar_Click(object sender, RoutedEventArgs e)
        {
            Popup_Filtra_Contato dialog = new Popup_Filtra_Contato();
            await dialog.ShowAsync();
            AtualizaLista();
            e = null;
        }

        private async void Bt_Editar_Click(object sender, RoutedEventArgs e)
        {
            FormatacaoContato dialog = new FormatacaoContato();
            await dialog.ShowAsync();
            AtualizaLista();
            e = null;
        }

        private async void Bt_Ajuda_Click(object sender, RoutedEventArgs e)
        {
            Globais.Aviso_Abrir_Contatos = false;
            var dialog = new Popup_Explicacao_Contatos();
            await dialog.ShowAsync();
            e = null;
        }
    }
}
