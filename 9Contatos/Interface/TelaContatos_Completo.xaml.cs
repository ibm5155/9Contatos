using _9Contatos.Classe;
using _9Contatos.Codigo;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TelaContatos_Completo : Page
    {
        private int QualContato = 0;
        private List<Contatos_ModeloLista> ListadeContatos = new List<Contatos_ModeloLista>();


        public TelaContatos_Completo()
        {
            this.QualContato = Globais.Indice_Contato_Completo;
            this.InitializeComponent();
            AtualizaLista();
            Nome.Text = Globais.contatos[QualContato].NomeCompleto;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                int i = 0;
                // TODO: Go back to the previous page
            };
        }

        private async void ListaContatosView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListView lista = (ListView)sender;
            int indice = lista.SelectedIndex;
            if (indice >= 0)
            {
                string Tmp_NomeCompleto = Globais.contatos[QualContato].NomeCompleto;

                var dialog = new Janela_EditaContato(Tmp_NomeCompleto, Globais.contatos[QualContato].Telefones_Antigos[indice]);
                await dialog.ShowAsync();
                string result = dialog.Result;
                if (result != "X")
                {
                    Globais.contatos[QualContato].Telefones_Antigos[indice] = result;
                    Globais.contatos[QualContato].NumeroAlterado[indice] = 0;
                    Globais.contatos[QualContato].Telefones_Formatados[indice].SetTelefone(result);
                    Globais.contatos[QualContato].NumeroAlterado[indice] = 0;
                    ListadeContatos[indice].Nome = Tmp_NomeCompleto;
                    ListadeContatos[indice].TelefoneNovo = Globais.contatos[QualContato].Telefones_Formatados[indice].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais);
                    string ImagemEnd;
                    if (Globais.contatos[QualContato].NumeroAlterado[indice] == 1)
                    {
                        ImagemEnd = "ms-appx:///Assets/nono.png";
                    }
                    else if (Globais.contatos[QualContato].NumeroAlterado[indice] == 0)
                    {
                        ImagemEnd = "ms-appx:///Assets/ok.png";
                    }
                    else
                    {
                        ImagemEnd = "ms-appx:///Assets/duvida.png";
                    }
                    ListadeContatos[indice].ImagePath = ImagemEnd;
                    AtualizaLista();
                }
            }
        }

        private void AtualizaLista()
        {
            int i = 0;
            string NomeCompleto;
            string Situacao = "";
            string sTelefoneNovo = "";
            string sTelefoneAntigo = "";
            string sTelefoneNotas = "";
            ListadeContatos.Clear();
            for (i = 0; i < Globais.contatos[QualContato].NumeroAlterado.Count; i++)
            {
            NomeCompleto = Globais.contatos[QualContato].NomeCompleto;
                if (Globais.contatos[QualContato].Flag_Recebeu_Nono_Digito[i] == true)
                {
                    Situacao = "ms-appx:///Assets/nono.png";
                }
                else if (Globais.contatos[QualContato].Flago_Numero_Eh_Internacional[i] == true)
                {
                    Situacao = "ms-appx:///Assets/internacional.png";
                }
                else if (Globais.contatos[QualContato].NumeroAlterado[i] == 0)
                {
                    Situacao = "ms-appx:///Assets/ok.png";
                }
                else
                {
                    Situacao = "ms-appx:///Assets/duvida.png";
                }
                sTelefoneNovo = Globais.contatos[QualContato].Telefones_Formatados[i].Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais);
                sTelefoneAntigo = Globais.contatos[QualContato].Telefones_Antigos[i];
                sTelefoneNotas = "";
                if (sTelefoneAntigo != sTelefoneNovo)
                {
                    sTelefoneNotas = "↴";
                }
                else
                {
                    sTelefoneNovo = "";
                }
                ListadeContatos.Add(new Contatos_ModeloLista { ImagePath = Situacao, TelefoneNovo = sTelefoneNovo, TelefoneAntigo = sTelefoneAntigo, TelefoneNotas = sTelefoneNotas });
            }
            ListaContatosView.ItemsSource = null;
            ListaContatosView.ItemsSource = ListadeContatos;
        }

        private async void Bt_Editar_Click(object sender, RoutedEventArgs e)
        {
            FormatacaoContato dialog = new FormatacaoContato();
            await dialog.ShowAsync();
            AtualizaLista();
        }

        private void bt_Voltar_Click(object sender, RoutedEventArgs e)
        {
            ListaContatosView.ItemsSource = null;
            this.Frame.GoBack();
//            this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
        }

        private async void Bt_Ajuda_Click(object sender, RoutedEventArgs e)
        {
            Globais.Aviso_Abrir_Contatos = false;
            var dialog = new Popup_Explicacao_Contatos();
            await dialog.ShowAsync();
        }
    }
}
