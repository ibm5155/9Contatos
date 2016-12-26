/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Ude;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace _9Contatos.Codigo
{
    class CSV
    {
        public string[] linha;
        public bool Valido = false;
  
        public async   
        Task
Carrega_Novo_CSV()
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".csv");


            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    var read = await FileIO.ReadTextAsync(file);
                    string BufferTexto = read;
                    linha = BufferTexto.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    Valido = true;
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    ICharsetDetector cdet = new CharsetDetector();
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    byte[] bytes = buffer.ToArray();
                    cdet.Feed(bytes, 0, bytes.Length);
                    cdet.DataEnd();
                    if (cdet.Charset != null)
                    {
                        string text = Portable.Text.Encoding.GetEncoding(cdet.Charset).GetString(bytes, 0, bytes.Length);
                        linha = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        Valido = true;
                    }
                }
            }
        }
    }
}
*/