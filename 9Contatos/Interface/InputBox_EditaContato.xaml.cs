using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using _9Contatos.Codigo;
using _9Contatos.Classe;
using System.Threading.Tasks;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class Janela_EditaContato : ContentDialog
    {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
    "NumeroEditado", typeof(string), typeof(Janela_EditaContato), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private string Nome = "";
        private string Celular = "";

        public string Result = "X";
        public Janela_EditaContato()
        {
            this.InitializeComponent();
        }

        public Janela_EditaContato(string Nome, string Celular)
        {
            this.Nome = Nome;
            this.Celular = Celular;
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Cancelar
            Result = NumeroEditado.Text;
            NumeroEditado.Text = "X";
            this.Hide();


        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Editar
            this.Hide();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            NomeEditado.Text = this.Nome;
            NumeroEditado.Text = this.Celular;
        }
    }
}
