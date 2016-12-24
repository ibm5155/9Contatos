using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9Contatos.Codigo
{
    /**
     *  Estrutura de dados que será utilizada pela listview dos contatos
     */
    public class Contatos_ModeloLista
    {
        public string ImagePath /*Situacao*/ { get; set; }
        public string TelefoneNovo { get; set; }
        public string TelefoneAntigo { get; set; }
        public string TelefoneNotas { get; set; }
        public string Nome { get; set; }
    }
}
