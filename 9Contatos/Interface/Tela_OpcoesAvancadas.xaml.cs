using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Popups;
using _9Contatos.API.PeopleAPP;
using _9Contatos.API.Outlook;
using _9Contatos.Contatos.Carrega;
using _9Contatos.globais;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OpcoesAvancadas : Page
    {
        public OpcoesAvancadas()
        {
            InitializeComponent();
            if(Globais.VERSAO_TESTES == false)
            {
//                Arrumar_Com_Modificacao.Visibility = Visibility.Collapsed;
                Arrumar_Email.Visibility = Visibility.Collapsed;
            }
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
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

        private async void listView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListView lista = (ListView)sender;
            int indice = lista.SelectedIndex;
            if (indice >= 0)
            {
                bool Carregar;
                switch (indice)
                {
                    case 0:
                        Globais.api_usada = QualAPI.PeopleAPI_COM_Alteracao;
                        Carregar = await CarregaContatos.Carrega(QualAPI.PeopleAPI_COM_Alteracao);
                        if (Carregar == true)
                        {
                            Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                        }
                        break;
                    case 1:
/*                        Globais.api_usada = QualAPI.PeopleAPI_COM_Alteracao;
                        Carregar = await CarregaContatos.Carrega(QualAPI.PeopleAPI_SemNono);
                        if (Carregar == true)
                        {
                            Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                        }
                        break;
                        */
                        case 2:
#if MODO_TESTES || DEBUG
                        Globais.api_usada = QualAPI.OutlookAPI;
                        try
                        {
                            if (await SignInCurrentUserAsync())
                            {
                                Carregar = await CarregaContatos.Carrega(QualAPI.OutlookAPI);
                                if (Carregar == true)
                                {
                                    Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                                }
                            }
                            else
                            {
                                var pergunta = new MessageDialog("Não podemos conectar ao serviço da Microsoft. Verfifique sua conexão de rede ou tente novamente mais tarde.");
                                pergunta.Title = "Problemas em contactar o servidor da microsoft.";
                                pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                                pergunta.ShowAsync();
                            }
                        }
                        catch (Microsoft.Identity.Client.MsalServiceException)
                        {
                            //faz nada já que não fez login....
                        }
#else
                        var pergunta = new MessageDialog("Esta funcionalidade ainda está em desenvolvimento.");
                        pergunta.Title = "Funcionalidade em construção.";
                        pergunta.Commands.Add(new UICommand { Label = "OK", Id = 0 });
                        await pergunta.ShowAsync();
#endif
                        break;
                    case 3:
                        PeopleAPI X = new PeopleAPI();
                        X.Limpa_Contatos_Temporarios();
                        var pergunta2 = new MessageDialog("");
                        pergunta2.Content = "Todos os contatos temporários criados pelo app Arruma Contatos foram excluidos.";
                        pergunta2.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                        await pergunta2.ShowAsync();
                        break;
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            /*
                        ProgressBar.Name = "";
                        ProgressBar = null;
                        Titulo.Name = "";
                        Titulo = null;
                        Descricao.Name = "";
                        Descricao = null;
                        Voltar.Name = "";
                        Voltar = null;
                        Arrumar_Com_Modificacao.Name = "";
                        Arrumar_Com_Modificacao = null;
                        Arrumar_Com_Modificacao_icone.Name = "";
                        Arrumar_Com_Modificacao_icone = null;
                        Arrumar_Com_Modificacao_texto.Name = "";
                        Arrumar_Com_Modificacao_texto = null;
                        Arrumar_Sem_Nono.Name = "";
                        Arrumar_Sem_Nono = null;
                        Arrumar_Sem_Nono_icone.Name = "";
                        Arrumar_Sem_Nono_icone = null;
                        Arrumar_Sem_Nono_texto.Name = "";
                        Arrumar_Sem_Nono_texto = null;
                        Arrumar_Email.Name = "";
                        Arrumar_Email = null;
                        Arrumar_Email_icone.Name = "";
                        Arrumar_Email_icone = null;
                        Arrumar_Email_texto.Name = "";
                        Arrumar_Email_texto = null;
                        Limpar_Contatos.Name = "";
                        Limpar_Contatos = null;
                        Limpar_Contatos_icone.Name = "";
                        Limpar_Contatos_icone = null;
                        Limpar_Contatos_texto.Name = "";
                        Limpar_Contatos_texto = null;
                        listView.Name = "";
                        listView.ItemsSource = null;
                        listView = null;
             */
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
