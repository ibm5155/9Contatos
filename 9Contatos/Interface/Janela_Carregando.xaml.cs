using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class Janela_Carregando : ContentDialog
    {
        public Janela_Carregando()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public void Altera_Maximo(int Max)
        {
            Barra_Progresso.Maximum = Max;
        }

        public void Incrementa_Barra()
        {
            Barra_Progresso.Value = Barra_Progresso.Value + 1;
        }
    }
}
