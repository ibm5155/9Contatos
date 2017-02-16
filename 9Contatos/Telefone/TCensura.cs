namespace _9Contatos.Telefones.Censura
{
    class TCensura
    {
        /// <summary>
        /// Objetivo dessa função é preservar os dados importantes do número e removendo os dados que
        /// </summary>
        /// <param name="Numero"></param>
        /// <returns></returns>
        public static string Censura_Telefone(string Numero)
        {
            string x = "";
            if(Numero != "")
            {
                x = Numero.Replace('0', 'A');
                x = x.Replace('1', 'A');
                x = x.Replace('2', 'A');
                x = x.Replace('3', 'A');
                x = x.Replace('4', 'A');
                x = x.Replace('5', 'A');
                x = x.Replace('6', 'B');
                x = x.Replace('7', 'B');
                x = x.Replace('8', 'B');
                x = x.Replace('9', 'B');
            }
            return x;
        }
    }
}
