﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using _9Contatos.Classe;
using System.Linq;

namespace _9Contatos.Codigo
{
    enum TipoEmail
    {
        Outlook,
        Gmail
    }

    enum ContatoSalvo
    {
        UnauthorizedAccessException = -1,
        NaoSalvo,
        Salvo 
    }

    class Contato
    {
        public List<Telefone> Telefones_Formatados = new List<Telefone>();
        public List<string> Telefones_Antigos = new List<string>();
        /// <summary>
        /// 0 não (correto) 1 (alterado) -1 (não sabe o que é isso) -2 (alterado pelo usuário)
        /// </summary>
        public List<int> NumeroAlterado = new List<int>();

        public List<bool> Flag_Numero_Alterado = new List<bool>(); // true se o numero editado for diferente do número alterado
        public List<bool> Flag_Recebeu_Nono_Digito = new List<bool>();
        public List<bool> Flag_Numero_Eh_Servico = new List<bool>();
        public List<bool> Flag_Numero_Eh_Desconhecido = new List<bool>();
        public List<bool> Flago_Numero_Eh_Internacional = new List<bool>();

        public string NomeCompleto = "";
        public Contact ID { set; internal get; } // objeto contato (somente escrita)

        public async Task<ContatoSalvo> SalvaContato()
        {
            PeopleAPI Link = new PeopleAPI();
            ContatoSalvo Status = ContatoSalvo.NaoSalvo;
            IEnumerable<string> tel_formatado = from tel in Telefones_Formatados
                                                select tel.Get_Numero_Formatado(Globais.Formatacao_Original, Globais.Formatacao_Traco, Globais.Formatacao_Espaco, Globais.Formatacao_Aspas, Globais.Formatacao_Distancia, ref Globais.MinhaRegiao, Globais.Formatacao_Ocultar_Meu_DDD, Globais.Formatacao_Ocultar_Pais);
            try
            {
                int saida = 0;
                saida = await Link.AlterarContato(ID, NomeCompleto, tel_formatado.ToList(), Telefones_Antigos);
                if (saida == 0)
                {
                    Status = ContatoSalvo.Salvo;
                }
            }
            catch (System.UnauthorizedAccessException)
            {
                Status = ContatoSalvo.UnauthorizedAccessException;
            }
            return Status;
        }
    }
}
