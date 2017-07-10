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


        /// <summary>
        /// Serve para ajustar o tamanho maximo que o texto pode ter na horizontal a fim de não ficar fora da janela
        /// assim podendo ajustar o mesmo para qualquer tipo de dispositivo.
        /// </summary>
        private void SetMaxTextWidth()
        {
//            this.Normal_Texto.Width = this.MaxWidth - this.Normal_Imagem.Width - 120 /*ContentDialog Border*/;
 //           this.Erro_Texto.Width = this.MaxWidth - this.Erro_Imagem.Width - 120 /*ContentDialog Border*/;
   //         this.Normal_Texto.MaxWidth = this.MaxWidth - this.Normal_Imagem.Width - 120 /*ContentDialog Border*/;
     //       this.Erro_Texto.MaxWidth = this.MaxWidth - this.Erro_Imagem.Width - 120 /*ContentDialog Border*/;
        }

        public ContentDialog_Processando()
        {
            this.InitializeComponent();
            ProgressBar.Visibility = Visibility.Visible;
        }

        public ContentDialog_Processando(string Titulo,string Mensagem,string Icone)
        {
            this.InitializeComponent();
            SetMaxTextWidth();
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
                SetMaxTextWidth();
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
                SetMaxTextWidth();
                Erro_Texto.TextWrapping = TextWrapping.Wrap;
            }
            Erro_Imagem.Visibility = Visibility.Visible;
            Erro_Texto.Visibility = Visibility.Visible;
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
                Erro_Imagem.Visibility = Visibility.Collapsed;
                Erro_Texto.Visibility = Visibility.Collapsed;
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
