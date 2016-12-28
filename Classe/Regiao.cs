using System;


namespace _9Contatos.Classe
{
    class Regiao
    {

        public static int ValidaRegiao(string Numero)
        {
            //Saidas :
            // 2 É uma região e não precisa colocar o nono digito
            // 1 É uma região e precisa colocar o nono digito
            // 0 Não é uma  região
            //
            //
            //
            bool Validacao = true;//Valida se o numero é uma região
            int Saida = 1;
            int numero =0;
            if(Int32.TryParse(Numero, out numero) == false)
            {
                Validacao = false;
                Saida = 0;
            }
            else
            {
                //Valida se o número em questão é uma região

                switch (numero)
                {
                    case 51:
                    case 53:
                    case 54:
                    case 55:
                        //Saida = 2;
                        //Rio Grande do Sul
                        break;
                    case 47:
                    case 48:
                    case 49:
                        //Saida = 2;
                        //Santa Catarina
                        break;
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                        //Saida = 2;
                        break;
                    case 41:
                        //parana
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                        //sao paulo
                        break;
                    case 63:
                        //Saida = 2;
                        //Tocantins
                        break;
                    case 21:
                    case 22:
                    case 24:
                        //Rio de Janeiro
                        break;
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 37:
                    case 38:
                        // Minas Gerais
                        break;
                    case 27:
                    case 28:
                        //Espirito Santo
                        break;
                    case 71:
                    case 73:
                    case 74:
                    case 75:
                    case 77:
                        //Bahia
                        break;
                    case 79:
                        //Sergipe
                        break;
                    case 82:
                        //Alagoas
                        break;
                    case 81:
                    case 87:
                        //Pernambuco
                        break;
                    case 83:
                        //Paraiba
                        break;
                    case 84:
                        //Rio Grande do Norte
                        break;
                    case 85:
                    case 88:
                        //Ceara
                        break;
                    case 86:
                    case 89:
                        //Piaui
                        break;
                    case 67:
                        //Saida = 2;
                        //Mato Grosso do Sul
                        break;
                    case 62:
                    case 64:
                        //Saida = 2;
                        //Goias
                        break;
                    case 61:
                        //Saida = 2;
                        //Distrito Federal
                        break;
                    case 98:
                    case 99:
                        //Maranhão
                        break;
                    case 65:
                    case 66:
                        //Saida = 2;
                        //Mato Grosso
                        break;
                    case 91:
                    case 93:
                    case 94:
                        //Para
                        break;
                    case 96:
                        //Amapa
                        break;
                    case 69:
                        //Saida = 2;
                        //Rondonia
                        break;
                    case 68:
                        //Saida = 2;
                        //Acre
                        break;
                    case 92:
                    case 97:
                        //Amazonas
                        break;
                    case 95:
                        //Roraima
                        break;
                    default:
                        Validacao = false;
                        Saida = 0;
                        break;
                }
            }
            if (Validacao == true)
            {
                if (DateTime.Now.Year >= 2017)
                { // a partir de 31 de dezembro de 2016, todos os números de celulares terão o nono digito.
                    Saida = 1;
                }
            }
            return Saida;
        }
    }
}
