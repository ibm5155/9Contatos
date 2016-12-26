using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using _9Contatos.Classe;
using _9Contatos.Codigo;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class FormatacaoContato : ContentDialog
    {
        private bool Devo_Fechar = false;
        private string Exemplo_Atual = "";

        private Telefone Exemplo = new Telefone();




        public FormatacaoContato()
        {
            this.InitializeComponent();
            Gera_Exemplos();
            Atualiza_Exemplo();
            checkBox_Parentes.IsChecked = Globais.Formatacao_Aspas; // oops é aspas.
            checkBox_Espaco.IsChecked = Globais.Formatacao_Espaco;
            checkBox_Original.IsChecked = Globais.Formatacao_Original;
            checkBox_Traco.IsChecked = Globais.Formatacao_Traco;
            checkBox_LongDist.IsChecked = Globais.Formatacao_Distancia;
            if(Globais.Formatacao_Ocultar_Pais == false)
            {
                Internacional.IsChecked = true;
            }
            else if(Globais.Formatacao_Ocultar_Meu_DDD == true)
            {
                Nacional_Sem_DDD.IsChecked = true;
            }
            else
            {
                Nacional.IsChecked = true;
            }
        }

        private void checkBox_Original_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox X = (CheckBox)sender;
            Globais.Formatacao_Original = (bool)X.IsChecked;
            Atualiza_Exemplo();
        }

        private void checkBox_Espaco_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox X = (CheckBox)sender;
            Globais.Formatacao_Espaco = (bool)X.IsChecked;
            Atualiza_Exemplo();
        }

        private void checkBox_Parentes_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox X = (CheckBox)sender;
            Globais.Formatacao_Aspas = (bool)X.IsChecked;
            Atualiza_Exemplo();
        }

        private void checkBox_Traco_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox X = (CheckBox)sender;
            Globais.Formatacao_Traco = (bool)X.IsChecked;
            Atualiza_Exemplo();
        }

        private void checkBox_LongDist_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox X = (CheckBox)sender;
            Globais.Formatacao_Distancia = (bool)X.IsChecked;
            Atualiza_Exemplo();
        }

        private void Atualiza_Exemplo()
        {
            Numero_Exemplo_Formatado.Text = Exemplo.Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia,ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais); ;
            Numero_Exemplo_Nao_Formatado.Text = Exemplo_Atual;
        }

        private void Botao_Formatar_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Devo_Fechar = true;

        }

        private void Botao_Outro_Exemplo_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Devo_Fechar = false;
            Gera_Exemplos();
            Atualiza_Exemplo();
        }

        private void Gera_Exemplos()
        {
            if(Exemplo_Atual == "")
            {
                Exemplo_Atual = "+55" + Globais.MinhaRegiao  +  "982341234";
            }
            else
            {
                Random rnd = new Random(); // o random do C é mais fácil :|
                string Espaco = " ";
                if (rnd.Next(1) == 1)
                {
                    Espaco = "";
                }
                switch (rnd.Next(4))
                {
                    case 0: // somente número com ou sem o '-'
                        Exemplo_Atual = Gera_Numero();
                        break;
                    case 1: // número com DDD
                        Exemplo_Atual = Gera_Regiao() + Espaco + Gera_Numero();
                        break;
                    case 2: // número com DDD + código da operadora
                        Exemplo_Atual = Gera_Operadora() + Espaco + Gera_Regiao() + Espaco + Gera_Numero();
                        break;
                    case 3: // pais + DDD + número
                        Exemplo_Atual = Gera_Pais() + Espaco + Gera_Regiao() + Espaco + Gera_Numero();
                        break;
                    case 4: // pais + operadora + DDD + número
                        Exemplo_Atual = Gera_Pais() + Espaco + Gera_Operadora() + Espaco + Gera_Regiao() + Espaco + Gera_Numero();
                        break;
                }
            }
            Exemplo.SetTelefone(Exemplo_Atual);
        }

        private string Gera_Numero()
        {
            Random rnd = new Random(); // o random do C é mais fácil :|
            string Numero = rnd.Next(1000, 5999).ToString();

            if (rnd.Next(1) == 1) // número normal de casa
            {
                Exemplo_Atual += "-";
            }
            Numero += rnd.Next(1000, 5999).ToString();

            return Numero;
        }

        private string Gera_Regiao()
        {
            Random rnd = new Random(); // o random do C é mais fácil :|
            string reg; //região
            do
            {
                reg = rnd.Next(11, 99).ToString();
            } while (Regiao.ValidaRegiao(reg) == 0);
            return reg;
        }

        private string Gera_Operadora()
        {
            Random rnd = new Random(); // o random do C é mais fácil :|
            string op; //operadora
            do
            {
                op = rnd.Next(12, 98).ToString();
            } while (Operadora.ValidaPrestadora(op) == 0);
            return op;
        }

        private string Gera_Pais()// só o brasil
        {
            Random rnd = new Random(); // o random do C é mais fácil :|
            string Saida;
            if (rnd.Next(2) == 1)
            {
                Saida = "+55";
            }
            else
            {
                Saida = "0";
            }
            return Saida;
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if(Devo_Fechar == false)
            {
                args.Cancel = true;
            }
        }

        private void checkBox_Ocultar_Meu_DDD_Click(object sender, RoutedEventArgs e)
        {
            CheckBox X = (CheckBox)sender;
            Globais.Formatacao_Ocultar_Meu_DDD = (bool)X.IsChecked;
            Atualiza_Exemplo();
        }

        private void Radio_Nacional_Sem_DDD_Click(object sender, RoutedEventArgs e)
        {
            Globais.Formatacao_Ocultar_Meu_DDD = true;
            Globais.Formatacao_Ocultar_Pais = true;
            Atualiza_Exemplo();
        }

        private void Nacional_Com_DDD_Click(object sender, RoutedEventArgs e)
        {
            Globais.Formatacao_Ocultar_Meu_DDD = false;
            Globais.Formatacao_Ocultar_Pais = true;
            Atualiza_Exemplo();
        }

        private void Internacional_Click(object sender, RoutedEventArgs e)
        {
            Globais.Formatacao_Ocultar_Meu_DDD = false;
            Globais.Formatacao_Ocultar_Pais = false;
            Atualiza_Exemplo();
        }
    }

}
