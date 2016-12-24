using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using _9Contatos.Classe;

namespace _9Contatos.Codigo
{
    class ParserNonoDigito
    {
        private bool ECelular(string Numero)
        {
            bool Saida = false;
            //com base na fonte da  Inteligencia em Telecomunicações. http://www.teleco.com.br/num.asp
            if (Numero.Count() >= 8)
            {
                switch (Numero[0])
                {
                    // Telefonia Fixa
                    /*              
                     case '2':
                     case '3':
                     case '4':
                     case '5':                    
                     break;
                     */
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        Saida = true;
                        break;
//                  default:

                }
            }

            return Saida;
        }

        private bool ChecaRegiao(string regiao)
        {
            bool Saida = false;
            switch(Regiao.ValidaRegiao(regiao))
            {
                case 0:
                case 2:
                    Saida = false;
                    break;
                case 1:
                    Saida = true;
                    break;
            }
            return Saida;
        }

        public int ChecaNumero(ref Telefone _Celular, string MinhaRegiao)
        {

            string Celular = _Celular.Numero;
            string Regiao = MinhaRegiao;
            int saida = 0; // 0 = não alterado, 1 = alterado, -1 = serviço, -2 não sei o que é|ERRO
            if (_Celular.Regiao != "")
            {
                Regiao = _Celular.Regiao;
            }
            if (Celular != "")
            {
                if (Celular.Count() == 8)
                {
                    if (ECelular(Celular) == true)
                    {
                        if (ChecaRegiao(Regiao) == true)
                        {
                            _Celular.Numero = _Celular.Numero.Insert(0, "9");
                            saida = 1;
                        }
                    }
                }
            }
            else if(_Celular.Servico != "")
            {
                saida = -1;
            }
            else if(_Celular.Numero_Nao_Reconhecido != "")
            {
                saida = -2;
            }
            return saida;
        }
    }
}
