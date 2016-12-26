using System.Collections.Generic;
using _9Contatos.Contatos.Contato;
using _9Contatos.Contatos.Carrega;

namespace _9Contatos.globais
{
    class Globais
    {
        public static List<Contato> contatos = new List<Contato>();
        public static string MinhaRegiao = "";
        public static int OffsetCelular = -2;
        public static List<int> OffsetNome = new List<int>(); // Indica os Offsets do contato
        public static TipoEmail EmailSelecionado = TipoEmail.Outlook;

        public static bool Formatacao_Aspas = true;
        public static bool Formatacao_Original = false;
        public static bool Formatacao_Traco = true;
        public static bool Formatacao_Espaco = true;
        public static bool Formatacao_Distancia = true;
        public static bool Formatacao_Ocultar_Meu_DDD = false;
        public static bool Formatacao_Ocultar_Pais = true;

        public static QualAPI api_usada;

        public static bool Aviso_Exportar = true;
        public static bool Aviso_Abrir_Contatos = true;
        public static int Indice_Contato_Completo = 0;

        //public static bool Filtrar_OK = true;
        //public static bool Filtrar_Duvida = true;
        //public static bool Filtrar_Aviso = true;

        public static bool Filtrar_Sem_Numero = false;
        public static bool Filtrar_Ocultar_Um_Contato = false;
        public static bool Filtrar_Numero_Alterado = true;
        public static bool Filtrar_Numero_Nao_Alterado = true;
        public static bool Filtrar_Nono_Digito = true;
        public static bool Filtrar_Servico = true;
        public static bool Filtrar_Desconhecido = true;
        public static bool Filtrar_Internacional = true;

    }
}
