﻿using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Popups;
using _9Contatos.globais;
using _9Contatos.Telefones.telefone;
using _9Contatos.Contatos.Carrega;
using _9Contatos.filehandler;
using Windows.UI.Xaml.Controls.Primitives;
using _9Contatos.Interface;
using _9Contatos.API.Outlook;
using System.Threading.Tasks;
using _9Contatos.API.PeopleAPP;
using System.Net.NetworkInformation;
using _9Contatos.InternetTools;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _9Contatos
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Telefone X = new Telefone();

            this.InitializeComponent();
            Globais.contatos.Clear();
            FileHandler.Carrega_Dados_9Contatos();
            if(Globais.VERSAO_TESTES == true && Globais.POPUP_TESTES == true)
            {
                Globais.POPUP_TESTES = false;
                var pergunta = new MessageDialog("Eu entendo que esta é uma versão de desenvolvimento e que problemas poderão ocorrer com o seu uso.\n Caso não queira testar esta versão use a versão que está na loja e desinstale este aplicativo.");
                pergunta.Title = "Versão de desenvolvimento";
                pergunta.Commands.Add(new UICommand { Label = "Entendi", Id = 0 });
                pergunta.ShowAsync();
            }
        }

        private async void bt_Sobre_Click(object sender, TappedRoutedEventArgs e)
        {
            if (Globais.MainPage_Bloqueia_Listview == false)
            {
                this.Frame.Navigate(typeof(_9Contatos.Interface.Sobre));
            }
        }

        private async void bt_donate(object sender, TappedRoutedEventArgs e)
        {
            if (Globais.MainPage_Bloqueia_Listview == false)
            {
                OptionBox_Donate dialog = new OptionBox_Donate();
                await dialog.ShowAsync();
            }
        }

        private async void bt_arrumar(object sender, TappedRoutedEventArgs e)
        {

            if (Globais.MainPage_Bloqueia_Listview == false)
            {
                Globais.MainPage_Bloqueia_Listview = true;
                ProgressBar.Visibility = Visibility.Visible;
                ListaOpcoes.SelectionMode = ListViewSelectionMode.None; //bloqueia a listview para evitar duplo cliques
                bool Carregar = false;
                if (Globais.MoroNoBrasil == false)
                {
                    Globais.Formatacao_Ocultar_Pais = false;
                    var pergunta = new MessageDialog("Sua região não é do brasil então só iremos formatar os números com o código do brasil (+55).");
                    pergunta.Title = "Olá estrangeiro.";
                    pergunta.Commands.Add(new UICommand { Label = "Entendi", Id = 0 });
                    pergunta.ShowAsync();

                }
                //teste de crash
                switch (Globais.api_usada)
                    {
                        case QualAPI.PeopleAPI_COM_Alteracao:
                            Carregar = await CarregaContatos.Carrega(QualAPI.PeopleAPI_COM_Alteracao);
                            ProgressBar.Visibility = Visibility.Collapsed;
                            if (Carregar == true)
                            {
                                if (Globais.contatos.Count() == 0)
                                {
                                    var pergunta = new MessageDialog("Como não tem nenhum contato na agenda você não poderá editar nada.");
                                    pergunta.Title = "Nenhum contato encontrado";
                                    pergunta.Commands.Add(new UICommand { Label = "Entendi", Id = 0 });
                                    pergunta.ShowAsync();
                                }
                                else
                                {
                                    this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                                }
                            }
                            break;

                        case QualAPI.PeopleAPI:

                            Carregar = await CarregaContatos.Carrega(QualAPI.PeopleAPI);
                            ProgressBar.Visibility = Visibility.Collapsed;
                            if (Carregar == true)
                            {
                                if (Globais.contatos.Count() == 0)
                                {
                                    var pergunta = new MessageDialog("Como não tem nenhum contato na agenda você não poderá editar nada.");
                                    pergunta.Title = "Nenhum contato encontrado";
                                    pergunta.Commands.Add(new UICommand { Label = "Entendi", Id = 0 });
                                    pergunta.ShowAsync();
                                }
                                else
                                {
                                    this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                                }
                            }
                            else
                            {
                                ProgressBar.Visibility = Visibility.Collapsed;
                            }
                            break;

                        case QualAPI.OutlookAPI:

                            if (Internet.CheckInternetConectivity() == true)
                            {
                                try
                                {
                                    Carregar = await CarregaContatos.Carrega(QualAPI.OutlookAPI);
                                    if (Carregar == true)
                                    {
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos)));
                                        //                                Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                                    }
                                    else
                                    {
                                        ProgressBar.Visibility = Visibility.Collapsed;
                                    }
                                }
                                catch (Microsoft.Identity.Client.MsalServiceException)
                                {
                                    //faz nada já que não fez login....
                                }
                            }
                            else
                            {
                                ProgressBar.Visibility = Visibility.Collapsed;
                                var pergunta = new MessageDialog("Para editar os contatos de uma conta da Microsoft você precisa ter conexão com a internet.");
                                pergunta.Title = "Sem Conexão com a internet";
                                pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                                pergunta.ShowAsync();
                            }
                            break;
                    }
                //libera listview
                ListaOpcoes.SelectionMode = ListViewSelectionMode.Single;
                Globais.MainPage_Bloqueia_Listview = false;
            }
        }

        private async void bt_classificar(object sender, TappedRoutedEventArgs e)
        {
            if (Globais.MainPage_Bloqueia_Listview == false)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
            }
        }

        private void bt_mais(object sender, TappedRoutedEventArgs e)
        {
            if (Globais.MainPage_Bloqueia_Listview == false)
            {
                switch (Globais.api_usada)
                {
                    case QualAPI.PeopleAPI:
                        Arrumar_Sem_Modificacao.IsChecked = true;
                        break;
                    case QualAPI.PeopleAPI_COM_Alteracao:
                        Arrumar_Com_Modificacao.IsChecked = true;
                        break;
                    case QualAPI.OutlookAPI:
                        Arrumar_Email.IsChecked = true;
                        break;
                }
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                //MaisOpcoes_Flyout
                //  this.Frame.Navigate(typeof(_9Contatos.Interface.OpcoesAvancadas), xx);
            }
        }

        private void OpcaoAlteraContatoLista_Selecionado(object sender, TappedRoutedEventArgs e)
        {
        }

        private void Mark_PeopleAPI1(object sender, TappedRoutedEventArgs e)
        {
            Globais.api_usada = QualAPI.PeopleAPI;
        }

        private void Mark_PeopleAPI2(object sender, TappedRoutedEventArgs e)
        {
            Globais.api_usada = QualAPI.PeopleAPI_COM_Alteracao;
        }

        private void Mark_OutlookAPI(object sender, TappedRoutedEventArgs e)
        {
            Globais.api_usada = QualAPI.OutlookAPI;
        }


        /// <summary>
        /// Signs in the current user.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SignInCurrentUserAsync()
        {
            var token = await OutlookAPI.GetTokenForUserAsync();

            if (token != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}

