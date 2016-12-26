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
            this.InitializeComponent();
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
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
                            this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                        }
                        break;
                    case 1:
                        Globais.api_usada = QualAPI.PeopleAPI_COM_Alteracao;
                        Carregar = await CarregaContatos.Carrega(QualAPI.PeopleAPI_SemNono);
                        if (Carregar == true)
                        {
                            this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                        }
                        break;
                    case 2:
#if MODO_TESTES || DEBUG
                        Globais.api_usada = QualAPI.OutlookAPI;
                        ProgressBar.Visibility = Visibility.Visible;
                        try
                        {
                            if (await SignInCurrentUserAsync())
                            {
                                Carregar = await CarregaContatos.Carrega(QualAPI.OutlookAPI);
                                if (Carregar == true)
                                {
                                    this.Frame.Navigate(typeof(_9Contatos.Interface.TelaContatos));
                                }
                            }
                            else
                            {
                            }
                        }
                        catch (Microsoft.Identity.Client.MsalServiceException)
                        {
                            //faz nada já que não fez login....
                        }

                        ProgressBar.Visibility = Visibility.Collapsed;

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
                        pergunta2.Content = "Todos os contatos criados pelo app Arruma Contatos foram excluidos.";
                        pergunta2.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                        await pergunta2.ShowAsync();
                        break;
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            /*
                        this.ProgressBar.Name = "";
                        this.ProgressBar = null;
                        this.Titulo.Name = "";
                        this.Titulo = null;
                        this.Descricao.Name = "";
                        this.Descricao = null;
                        this.Voltar.Name = "";
                        this.Voltar = null;
                        this.Arrumar_Com_Modificacao.Name = "";
                        this.Arrumar_Com_Modificacao = null;
                        this.Arrumar_Com_Modificacao_icone.Name = "";
                        this.Arrumar_Com_Modificacao_icone = null;
                        this.Arrumar_Com_Modificacao_texto.Name = "";
                        this.Arrumar_Com_Modificacao_texto = null;
                        this.Arrumar_Sem_Nono.Name = "";
                        this.Arrumar_Sem_Nono = null;
                        this.Arrumar_Sem_Nono_icone.Name = "";
                        this.Arrumar_Sem_Nono_icone = null;
                        this.Arrumar_Sem_Nono_texto.Name = "";
                        this.Arrumar_Sem_Nono_texto = null;
                        this.Arrumar_Email.Name = "";
                        this.Arrumar_Email = null;
                        this.Arrumar_Email_icone.Name = "";
                        this.Arrumar_Email_icone = null;
                        this.Arrumar_Email_texto.Name = "";
                        this.Arrumar_Email_texto = null;
                        this.Limpar_Contatos.Name = "";
                        this.Limpar_Contatos = null;
                        this.Limpar_Contatos_icone.Name = "";
                        this.Limpar_Contatos_icone = null;
                        this.Limpar_Contatos_texto.Name = "";
                        this.Limpar_Contatos_texto = null;
                        this.listView.Name = "";
                        this.listView.ItemsSource = null;
                        this.listView = null;
             */
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
