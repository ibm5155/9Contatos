//FONTE: http://www.anatel.gov.br/grandeseventos/en/component/content/article?id=103:lista-de-codigos-de-selecao-de-prestadora-csp.html
//FONTE: http://www.teleco.com.br/opfixald.asp

/* CSP   OPERADORA
 * 12 Algar
 * 13
 * 14 Brasil Telecom    
 * 15 Vivo, Telefonica
 * 16
 * 17
 * 28
 * 21 Embratel,Claro
 * 23 Intelig
 * 24
 * 25 GVT
 * 26
 * 27
 * 29
 * 31 Oi,Telemar
 * 32
 * 34
 * 35
 * 36
 * 37
 * 38
 * 41 Tim
 * ...
 * 61 Nexus
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9Contatos.Telefones.operadora
{
    class Operadora
    {
        public static readonly int[] TabelaCSP = { 12, 13, 14, 15, 16, 17, 18, 21, 23, 24, 25, 26, 27, 29, 31, 32, 34, 35, 36, 37, 38, 41, 42, 43, 45, 46, 47, 49, 53, 56, 57, 58, 61, 62, 63, 65, 71, 72, 73, 74, 75, 76, 81, 85, 89, 91, 96, 98 };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Prestadora"></param>
        /// <returns>CSP da operadora, 0 se não existe</returns>
        public static int ValidaPrestadora(string Prestadora)
        {
            
            int Saida = 0;
            int  CSP = 0;
            int i = 0;
            int total = 0;
            int metade;
            if(Prestadora.Length == 2)
            {
                bool Parser = Int32.TryParse(Prestadora, out CSP);
                if(Parser == true)
                {
                    total = TabelaCSP.Count();
                    metade = total / 2;
                    /* = BUSCA BINARIA = */
                    i = metade;//  ; // i = total / 2
                    while(metade > 0)
                    {
                        if(TabelaCSP[i] < CSP) // Tabela[i] ----  DADO ----...
                        {
                            metade = metade / 2; // ou /2
                            i += metade;
                        }
                        else if(TabelaCSP[i] > CSP)
                        {
                            metade = metade / 2; // ou /2
                            i -= metade;
                        }
                        else
                        {
                            Saida = CSP;
                            metade = 0;
                        }
                    }
                    if(Saida == 0)
                    {
                        if(i > 0)
                        {
                            if(TabelaCSP[i-1] == CSP)
                            {
                                Saida = TabelaCSP[i - 1];
                            }
                        }
                        else if(i < total)
                        {
                            if(TabelaCSP[i + 1] == CSP)
                            {
                                Saida = TabelaCSP[i + 1];
                            }
                        }
                    }
                }
            }
            return Saida;
        }
/*
        public static string GetNomePRestadora(string Prestadora)
        {
            int CSP = ValidaPrestadora(Prestadora);
            string Nome = "";
            if(CSP > 0)
            {
                switch(CSP)
                {
                    case 21:
                        Nome = "Embratel";
                        break;
                    case 23:
                        Nome = "Intelig";
                        break;
                    case 43:
                        Nome = "Sercomtel";
                        break;
                    case 31:
                        Nome = "Telemar";
                        break;
                    case 15:
                        Nome = "Telefonica";
                        break;
                    case 36:
                        Nome = "Claro";
                        break;
                    case 41:
                        Nome = "Tim";
                        break;
                }
            }
            return Nome;
        }
        */

        /// <summary>
        /// Recebe uma lista contendo dois strings representados pelo CSP e o resto sendo o nome da operadora
        /// </summary>
        /// <returns></returns>
        public static List<string> Get_Lista_Operadoras()
        {
            List<string> ls = new List<string>();
            ls.Add("12Algar");
            ls.Add("14Brasil Telecom");
            ls.Add("15Vivo");
            ls.Add("16Telefonica");
            ls.Add("21Embratel");
            ls.Add("21Claro");
            ls.Add("23Intelig");
            ls.Add("25GVT");
            ls.Add("31Oi");
            ls.Add("31Telemar");
            ls.Add("41Telemar");
            return ls;
        }

    }
}
