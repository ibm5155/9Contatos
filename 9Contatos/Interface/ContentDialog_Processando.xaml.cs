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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class ContentDialog_Processando : ContentDialog
    {
        private bool ErroRemovido = false;
        private const int Timeout = 25000; // 25 segundos
        private int TimeoutCnt = 0;
        public bool Fechar = false;

        public ContentDialog_Processando()
        {
            this.InitializeComponent();
            ProgressBar.Visibility = Visibility.Visible;
        }

        public ContentDialog_Processando(string Titulo,string Mensagem,string Icone)
        {
            this.InitializeComponent();
            this.Title = Titulo;
            if(Icone != null)
            {
                if (Icone.Count() == 0)
                {
                    this.Normal_Imagem.Source = null;
                }
                else
                {
                    this.Normal_Imagem.Source = new BitmapImage(new Uri(Icone));
                }
            }
            if (Mensagem != null)
            {
                this.Normal_Texto.Text = Mensagem;
                Normal_Texto.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                Normal_Texto.Text = "";
            }
            ProgressBar.Visibility = Visibility.Visible;
        }

        public void AdicionaErro(string Icone, string Mensagem)
        {
            RemoveErro();
            ErroRemovido = false;
            if (Icone != null)
            {
                if (Icone.Count() == 0)
                {
                }
                else
                {
                    this.Erro_Imagem.Source = new BitmapImage(new Uri(Icone));
                }
            }
            if (Mensagem != null)
            {
                this.Erro_Texto.Text = Mensagem;
                Erro_Texto.TextWrapping = TextWrapping.Wrap;
            }
        }

        public void RemoveErro()
        {
            if (ErroRemovido == false)
            {
                this.Erro_Imagem.Source = null;
                this.Erro_Texto.Text = "";
                ErroRemovido = true;
                TimeoutCnt = 0;
                button_Cancelar.Visibility = Visibility.Collapsed;
            }
        }

        public void IncrementaTimeout(int tempoMs)
        {
            TimeoutCnt += tempoMs;
            if(TimeoutCnt >= Timeout)
            {
                button_Cancelar.Visibility = Visibility.Visible;
            }
        }

        public void Destroi()
        {
            Erro_Imagem.Source = null;
            Normal_Imagem.Source = null;
            Fechar = true;
            ProgressBar.Visibility = Visibility.Collapsed;
            this.Visibility = Visibility.Collapsed;
            this.Hide();
        }

        private void button_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Destroi();
        }
    }
}
