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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class Popup_Explicacao_Contatos : ContentDialog
    {
        public Popup_Explicacao_Contatos()
        {
            InitializeComponent();
            switch(globais.Globais.api_usada)
            {
                case Contatos.Carrega.QualAPI.OutlookAPI:
                    this.MetodoUsado.Text = "-Você selecionou a opção de alteração de contatos de uma conta da microsoft, esse método permite você editar todos os seus contatos de sua conta microsoft." + Environment.NewLine + "AVISO: poderá demorar algum tempo até o aplicativo pessoas sincronizar as alterações feitas em sua conta.";
                    break;
                case Contatos.Carrega.QualAPI.PeopleAPI:
                    this.MetodoUsado.Text = "-Você selecionou a opção de alteração de contatos por vinculação, essa opção permite você vincular um contato temporário a todos os seus contatos do aplicativo pessoas e salvar nele os números alterados." + Environment.NewLine + "Por se tratar de contatos temporários, quando você desinstalar o aplicativo arruma contatos as alterações salvas desses contatos temporários irão sumir, por isso mantenha o aplicativo arruma contatos instalado." + Environment.NewLine + "AVISO: não apague os seus contatos originais pois você irá ao mesmo tempo apagar os contatos temporários, caso queira uma opção mais robusta de alteração de contatos, selecione outra opção de alteração de contatos no inicio do programa na opção 'opções avançadas'.";
                    break;
                case Contatos.Carrega.QualAPI.PeopleAPI_COM_Alteracao:
                    this.MetodoUsado.Text = "-Você selecionou a opção de alteração de contatos por edição, esse método permite você editar todos os seus contatos do aplicativo pessoas e salvar nele os números alterados." + Environment.NewLine + "-Nessa opção não serão criados contatos temporários.";
                    break;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
