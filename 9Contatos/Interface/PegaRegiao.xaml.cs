using System;
using Windows.UI.Xaml.Controls;
using _9Contatos.Classe;
using Windows.UI.Popups;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class PegaRegiao : ContentDialog
    {
        public PegaRegiao()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void ContentDialog_PrimaryButtonClick_1(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Botão OK, OK?
            if (Regiao.ValidaRegiao(Entrada.Text) > 0)
            {
                int MeuDDD_Inteiro = Int32.Parse(Entrada.Text);
                Globais.MinhaRegiao = MeuDDD_Inteiro.ToString(); //assim garantimos que não tem um 0XX onde o XX é o DDD do usuário (só server por enquanto para mostrar como vai ficar a formatação de um DDD local)
                this.Hide();
            }

            else
            {
                Entrada.Text = "";
                Globais.MinhaRegiao = "";
                var pergunta = new MessageDialog("Essa Região é invalida");
                pergunta.Title = "Algo deu errado";
                pergunta.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                await pergunta.ShowAsync();
            }

        }
    }
}
