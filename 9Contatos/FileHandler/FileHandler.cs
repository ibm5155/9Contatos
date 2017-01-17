using System;
using _9Contatos.globais;
using Windows.Storage;

namespace _9Contatos.filehandler
{
    class FileHandler
    {
        public static async void Carrega_Dados_9Contatos()
        {
            StorageFile arquivo;
            StorageFolder raiz_arquivo = ApplicationData.Current.LocalFolder;
            try
            {
                arquivo = await raiz_arquivo.GetFileAsync(Globais.ARQUIVO_TEMPORARIO);
                //se o arquivo existe então continuamos a sua leitura abaixo
                Globais.ARQUIVO_PRIMEIRA_ABERTURA = false;
                Globais.Aviso_Abrir_Contatos = false;

            }
            catch (System.IO.FileNotFoundException)
            {
                arquivo = await raiz_arquivo.CreateFileAsync(Globais.ARQUIVO_TEMPORARIO);
                //o arquivo não existe, logo criamos ele.
                //  arquivo = File.Create(Globais.ARQUIVO_TEMPORARIO);
            }           
        }
    }
}
