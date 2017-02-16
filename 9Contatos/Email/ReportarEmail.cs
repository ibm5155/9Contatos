using System;
using Windows.ApplicationModel.Email;
using Windows.Storage;



namespace _9Contatos.Email
{


    class ReportarEmail
    {
        private string EmailDeveloper = "lzfstudio@hotmail.com";
        private string Titulo = "";
        private string Mensagem = "";
        private string Log = "";

        public void AddTitulo(string titulo)
        {
            Titulo = titulo;
        }
        public void AddMensagem(string mensagem)
        {
            Mensagem = mensagem;
        }
        public void AdicionaLogAnexo(string log)
        {
            Log = log;
        }
        public async void Enviar()
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient(EmailDeveloper));
            emailMessage.Body = Mensagem;
            //Como o outro aplicativo não pode de nenhuma forma acessar a memória deste arquivo, temos que criar um arquivo para poder anexar um arquivo e mandar ele por email.
            StorageFolder MyFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile attachmentFile = await MyFolder.CreateFileAsync("LogErro.txt");

            if (attachmentFile != null)
            {
                var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);

                var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
                         attachmentFile.Name,
                         stream);
                await Windows.Storage.FileIO.WriteTextAsync(attachmentFile, Log);

                emailMessage.Attachments.Add(attachment);

            }
            
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
            //e-mail enviado ou não, segue depois abaixo...
            await attachmentFile.DeleteAsync(); //agora vamos apagar esse email para não deixar lixo no app.
        }

    }
}
