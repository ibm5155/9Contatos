using Windows.UI.Xaml.Controls;
using _9Contatos.globais;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class Popup_Filtra_Contato : ContentDialog
    {
        public Popup_Filtra_Contato()
        {
            InitializeComponent();
            checkBox_Nono.IsChecked = Globais.Filtrar_Nono_Digito;
            checkBox_Internacional.IsChecked = Globais.Filtrar_Internacional;
            checkBox_Desconhecido.IsChecked = Globais.Filtrar_Desconhecido;
            checkBox_Alterados.IsChecked = Globais.Filtrar_Numero_Alterado;
            checkBox_NAlterados.IsChecked = Globais.Filtrar_Numero_Nao_Alterado;
            checkBox_sem_numero.IsChecked = Globais.Filtrar_Sem_Numero;
            checkBox_Servico.IsChecked = Globais.Filtrar_Servico;
        }

        private void ContentDialog_PrimaryButtonClick_1(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Globais.Filtrar_Nono_Digito = (bool)checkBox_Nono.IsChecked;
            Globais.Filtrar_Desconhecido = (bool)checkBox_Desconhecido.IsChecked;
            Globais.Filtrar_Internacional = (bool)checkBox_Internacional.IsChecked;
            Globais.Filtrar_Numero_Alterado = (bool)checkBox_Alterados.IsChecked;
            Globais.Filtrar_Numero_Nao_Alterado = (bool)checkBox_NAlterados.IsChecked;
            Globais.Filtrar_Sem_Numero = (bool)checkBox_sem_numero.IsChecked;
            Globais.Filtrar_Servico = (bool)checkBox_Servico.IsChecked;
        }
    }
}
