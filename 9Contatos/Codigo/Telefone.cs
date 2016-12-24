//FONTE para estruturação de números: http://www.teleco.com.br/pdfs/tutorialnum.pdf
//
// Parser NÚMERO DE TELEFONE, POR: Lucas Zimerman Fraulob
//

using System.Linq;
using _9Contatos.Classe;

namespace _9Contatos.Codigo
{
    enum ChamadaACobrar
    {
        CobrarLocal,CobrarInterurbano,NaoCobrar
    }
    public class Telefone
    {
        public string Pais { internal set; get; }
        public string Regiao { internal set; get; } // sem o 0
        public string Numero { internal set; get; } // sem o '-'
        public string Numero_Nao_Geografico { internal set; get; } // 0800, 0300, 0500, 0900
        public string Numero_Nao_Reconhecido { internal set; get; }
        public string Numero_Internacional { internal set; get; }
        public string Servico { internal set; get; } // 1414, *100,...
        public string CSP { internal set; get; } // para mais informações chegar o arquivo Operadora.cs
        private bool Formatacao_TracoNumero = false;  // true : 8888-8888 false : 88888888
        private bool Formatacao_EspacoEntreNumero = false;
        private bool Formatacao_AspasEntreRegiao = false;
        private bool Formatacao_PrefixoLongaDistanciaNacional = false;
        private bool Contem_Caracteres_Estranhos = false;
        private bool Contem_Prestadora = false;
        private ChamadaACobrar Contem_Numero_A_Cobrar = ChamadaACobrar.NaoCobrar;

        public string Get_Numero_Formatado(bool ManterFormatacao, bool TracoNumero /* - */, bool EspacoEntreDados, bool AspasEntreRegiao,bool PrefixoLongaDistanciaNacional, ref string DDD_Nacional, bool OcultaDDD_Nacional, bool Oculta_Pais)
        {
            string Numero_Formatado = "";
            string Espaco = "";
            string TRegiao = this.Regiao;
            if(EspacoEntreDados == true || (ManterFormatacao == true && this.Formatacao_EspacoEntreNumero == true))
            {
                Espaco = " ";
            }            
            if(Servico != "")
            {
                Numero_Formatado = Servico;
            }
            else if(Numero_Nao_Reconhecido != "")
            {
                Numero_Formatado = Numero_Nao_Reconhecido;
            }
            else if(Numero_Nao_Geografico != "") // 0800
            {
                Numero_Formatado = this.Numero_Nao_Geografico + Espaco + this.Numero;
                if(TracoNumero == true || (ManterFormatacao == true && this.Formatacao_TracoNumero == true))
                {
                    Numero_Formatado = Numero_Formatado.Insert(Numero_Formatado.Count() - 3, "-");
                }
                if((AspasEntreRegiao == true || (ManterFormatacao == true && this.Formatacao_AspasEntreRegiao == true)) && TRegiao.Count() > 0)
                {
                    Numero_Formatado = Numero_Formatado.Insert(4, ")");
                    Numero_Formatado = "(" + Numero_Formatado;
                }
            }
            else if(Numero_Internacional != "")
            {
                Numero_Formatado = Numero_Internacional;
            }
            else if(Numero != "")
            {
                if (Oculta_Pais == false)
                {
                    if (Pais.Count() == 0)
                    {
                        Numero_Formatado += "+55" +  Espaco;
                    }
                    else
                    {
                        Numero_Formatado += Pais + Espaco;
                    }
                }

                if (this.Contem_Numero_A_Cobrar == ChamadaACobrar.CobrarLocal)
                {
                    Numero_Formatado += "9090" + Espaco;
                }
                else if (this.Contem_Numero_A_Cobrar == ChamadaACobrar.CobrarInterurbano)
                {
                    Numero_Formatado += "90" + Espaco;
                }

                if(this.CSP != "") //se tem o CSP ele quem terá o digito de longa distancia e não no DDD
                {
                    if ((PrefixoLongaDistanciaNacional == true || (ManterFormatacao == true && this.Formatacao_PrefixoLongaDistanciaNacional == true)) && TRegiao.Count() > 0)
                    {
                        Numero_Formatado +=  CSP + Espaco;
                    }
                    else
                    {
                        Numero_Formatado += CSP + Espaco;
                    }
                }
                else if((PrefixoLongaDistanciaNacional == true || (ManterFormatacao == true && this.Formatacao_PrefixoLongaDistanciaNacional == true)) && TRegiao.Count() > 0)
                {
                    TRegiao =Regiao;
                }
                if (OcultaDDD_Nacional == false || (this.Regiao != DDD_Nacional))
                {
                    // só adiciona o ddd se for permitido adicionar o DDD nacional e caso contário, se o DDD for diferente do DDD nacional
                    if ((AspasEntreRegiao == true || (ManterFormatacao == true && this.Formatacao_AspasEntreRegiao == true /* &&  TRegiao.Count() > 0*/)))
                    {
#warning Testar se a remoção do TRegião.Count > 0 pode afetar outros locais
                        if(ManterFormatacao == true && TRegiao.Count() == 0);
                        else if ((OcultaDDD_Nacional == false && TRegiao.Count() == 0))
                        {
                            //Caso o usuário deseje manter a formatação original e o prefixo de longa distancia que estava no número existir
                            //E
                            //Caso o usuário deseje não ocultar o DDD nacional
                            Numero_Formatado += "(" + DDD_Nacional + ")" + Espaco;
                        }
                        else if(TRegiao.Count() > 0)
                        {
                            Numero_Formatado += "(" + TRegiao + ")" + Espaco;
                        }
                    }
                    
                    else
                    {
                        if (OcultaDDD_Nacional == false && TRegiao.Count() == 0)
                        {
                            Numero_Formatado += DDD_Nacional + Espaco;
                        }
                        else if(TRegiao.Count() > 0 && OcultaDDD_Nacional == false)
                        {
                            Numero_Formatado += TRegiao + Espaco;
                        }
                    }
                }

                Numero_Formatado += Numero;
                if (TracoNumero == true || (ManterFormatacao == true && this.Formatacao_TracoNumero == true))
                {
                    Numero_Formatado = Numero_Formatado.Insert(Numero_Formatado.Count() - 4, "-");
                }
            }
            return Numero_Formatado;
        }

        public void SetTelefone(string numero)
        {
            CSP = "";
            Pais = "";
            Regiao = "";
            Numero = "";
            Numero_Nao_Geografico = "";
            Numero_Nao_Reconhecido = "";
            Numero_Internacional = "";
            Servico = "";
            int SizeofCelular = numero.Count();
            int Cnt_Aspas = 0;
            int Cnt_Traco = 0;
            string numero_original_nao_formatado = numero; 
            if (SizeofCelular >= 8)
            {
                #region Filtros (removemos simbolos idevidos e também checamos a presença de outros
                for(int i = 0; i < numero.Count(); i++)
                {
                    if (char.IsWhiteSpace(numero[i]) == false && numero[i] != '+' && numero[i] != '-' && numero[i] != '(' && numero[i] != ')' && !(numero[i] >= '0' && numero[i] <= '9'))
                    {
                        //Se tiver algum caracter que não seja um número ou um caracter válido pelo programa, remova ele e marque que esse número contém caracteres estranhos
                        numero = numero.Remove(i, 1);
                        Contem_Caracteres_Estranhos = true;
                        i--;
                    }
                }
                for (int i = 0; i < numero.Count(); i++)
                {
                    if (numero[i] == '(' || numero[i] == ')')
                    {
                        //removemos as aspas e marcamos a tag do número para que o programa saiba que inicialmente esse programa tinha aspas
                        numero = numero.Remove(i, 1);
                        Formatacao_AspasEntreRegiao = true;
                        Cnt_Aspas++;
                        i--;
                    }

                    else if (char.IsWhiteSpace(numero[i]) == true)
                    {
                        //o número contém espaços, remova eles e informe que o número oringinal contém espaços
                        Formatacao_EspacoEntreNumero = true;
                        numero = numero.Remove(i, 1);
                        i--;
                    }
                    else if (numero[i] == '-')
                    {
                        if (i != numero.Count() - 5)
                        {
                            Cnt_Traco = 10; // local incorreto do traço
                        }
                        Formatacao_TracoNumero = true;
                        numero = numero.Remove(numero.Count() - 5, 1);
                        Cnt_Traco++;
                        i--;
                    }
                }
                SizeofCelular = numero.Count();
                if(SizeofCelular > 2)
                {
                    if(numero[0] == '0' && numero[1] == '0')
                    {
                        //temos um número internacional, que iremos substituir o 00 por + para facilitar o entendimento do nosso parser
                        numero = numero.Remove(0, 2);
                        numero = "+" + numero;
                    }
                }
                #endregion
                // A partir daqui a gente tem um número limpo e pronto para ter o seu interior filtrado em partes.

                #region Parser Número
                SizeofCelular = numero.Count();

                if (Cnt_Traco > 1 || Cnt_Aspas > 2)
                {
                    this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                }
                else if(EhInternacional(ref numero) == true)
                {
                    this.Numero_Internacional = numero_original_nao_formatado;
                }
                else
                {
                    //primerio caso 12341234 -> 1234-1234
                    if (SizeofCelular == 8)
                    {
                        this.Numero = numero;
                    }
                    //segundo caso 912341234 -> 91234-1234
                    else if (SizeofCelular == 9)
                    {
                        if (numero[0] == '9')
                        {
                            if (numero[1] >= '6' && numero[1] <= '9')
                            { //é um celular
                                this.Numero = numero;
                            }
                            else
                            { //ai temos um numero alienigena
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                        else
                        {
                            this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                        }
                    }
                    //terceiro caso 4112341234 -> 41 1234-1234
                    else if (SizeofCelular == 10)
                    {
                        {
                            this.Regiao = numero.Remove(2);
                            this.Numero = numero.Remove(0, 2);
                        }
                    }
                    //quarto caso 04112341234 -> +55 41 12341234
                    //            41912341234 -> 41 912341234
                    //            117777-7777
                    //            08005100116 -> 0800 5100116 (deveria ser +55 800 5100116 mas ninguém usa esse formato)
                    //            +5512341234 -> +55 12341234 (quem diabos digita isso? não sei, mas encontrei isso na minha agenda e o próprio app do android/wp transforma isso em (015 41 12341234 ) onde 015 é o csp do usuário e o 41 a região do usuário, mas como o app faz isso, não vamos nos esquentar com isso não.
                    else if (SizeofCelular == 11)
                    {
                        if (numero[0] == '0')
                        {
                            if ((numero[1] == '9' || numero[1] == '8' || numero[1] == '5' || numero[1] == '3') && numero[2] == '0' && numero[3] == '0')
                            {// caso 0800, neste caso um número de telefone tem 7 digitos
                                /*
                                 * A discagem de um código não geográfico deve ser precedida do prefixo nacional como apresentado a seguir
                                 *  0 + 3 digitos para o código + 7 digitos para o telefone
                                 */
                                this.Numero_Nao_Geografico = numero.Remove(4);
                                this.Numero = numero.Remove(0, 4);
                            }
                            else
                            {
                                //04112341234 -> +55 41 12341234
                                this.Pais = "+55";
                                this.Regiao = numero.Remove(3);
                                this.Regiao = this.Regiao.Remove(0, 1);
                                this.Numero = numero.Remove(0, 3);
                                this.Formatacao_PrefixoLongaDistanciaNacional = true;
                            }
                        }
                        else if(numero[0] == '+')
                        {
                            //+5512341234
                            this.Pais = numero.Remove(3);
                            this.Numero = numero.Remove(0, 3);
                        }
                        else
                        {
                            if (numero[3] >= '6' && numero[3] <= '9' && numero[2] == '9') // 41912341234
                            {
                                this.Regiao = numero.Remove(2);
                                this.Numero = numero.Remove(0, 2);
                            }
                            else if (this.Formatacao_TracoNumero == true) // 117777-7777
                            {
                                this.Regiao = numero.Remove(2);
                                this.Numero = numero.Remove(0, 2);
                            }
                            else
                            {
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;

                            }
                        }
                    }
                    // quinto caso 041912341234 -> +55 41 91234-1234
                    //909012341234 -> 9090 12341234
                    //041912341234
                    //144122221111 -> 14 41 2222-1111
                    //  !904112341234 -> 90 41 12341234 NâO É valido por não conter o csp
                    else if (SizeofCelular == 12)
                    {
                        if (numero[0] == '0')
                        {
                            if (numero[4] >= '6' && numero[4] <= '9' && numero[3] == '9')
                            {
                                this.Pais = "+55";
                                this.Regiao = numero.Remove(3);
                                this.Regiao = this.Regiao.Remove(0, 1);
                                this.Numero = numero.Remove(0, 3);
                                this.Formatacao_PrefixoLongaDistanciaNacional = true;
                            }
                            else
                            {
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                        else
                        {
                            this.CSP = numero.Remove(2);
                            if (Operadora.ValidaPrestadora(this.CSP) != 0)
                            {
                                // 144122221111 -> 14 41 2222-1111
                                this.Regiao = numero.Remove(4);
                                this.Regiao = this.Regiao.Remove(0,2);
                                this.Numero = numero.Remove(0, 4);
                            }
                            else if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarLocal)
                            {
                                // 909012341234
                                this.CSP = "";
                                this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;
                                this.Numero = numero.Remove(0, 4); //  tira o 9090
                                this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;
                            }
                            else if(ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarInterurbano)
                            {
                                //904112341234
                                //Modelo invalido então vamos corrigir para
                                //909012341234
                                //se a região for a do usuário ou 
                                //90 41 12341234
                                //caso contrario
#warning falta tratar os casos comentados acima
                                this.CSP = "";
                                this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarInterurbano;
                                this.Regiao = numero.Remove(0, 2);
                                this.Regiao = this.Regiao.Remove(2);
                                this.Numero = numero.Remove(0, 4); //  tira o 90 e a região
                            }                            
                            else
                            {
                                this.CSP = "";
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                    }
                    // sexto caso +554112341234 -> +55 41 1234-1234
                    //            0144112341234 -> +55 14 41 12341234 (onde 14 é o CSP)
                    //            9090912341234 -> 9090 912341234       
                    //            !!! 9004112341234 -> 90 041 12341234  (não é válido pois precisa do CSP para fazer chamadas a cobrar a distancia
#warning reavaliar ultimo caso invalido do comentario
                    else if (SizeofCelular == 13)
                    {
                        if (numero[0] == '+')
                        {
                            this.Pais = numero.Remove(3);
                            this.Regiao = numero.Remove(0, 3);
                            this.Regiao = this.Regiao.Remove(2);
                            this.Numero = numero.Remove(0, 5);
                        }
                        else
                        {
                            if(ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarLocal)
                            {
                                if(numero[5] >= '6' && numero[5] <= '9' && numero[4] == '9')
                                {
                                    //9090 912341234
                                    this.Numero = numero.Remove(0, 4);
                                    this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;

                                }
                                else // tem o 9090 mas o proximo digito não é o nono digito, então não sabemos o que é, ou tem o nono digito entretanto é um numero fixo.
                                {
                                    this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                }
                            }
                            else if(numero[0] == '0')
                            {// achamos um zero, então pode conter um CSP nos próximos dois digitos.
                                this.CSP = numero.Remove(3);
                                this.CSP = this.CSP.Remove(0, 1);
                                if(Operadora.ValidaPrestadora(this.CSP) != 0)
                                {
                                    // 0144112341234 -> +55 14 41 1234-1234
                                    this.Pais = "+55";
                                    this.Regiao = numero.Remove(0, 3);
                                    this.Regiao = this.Regiao.Remove(2);
                                    this.Numero = numero.Remove(0, 5);
                                    this.Formatacao_PrefixoLongaDistanciaNacional = true;
                                }
                                else
                                {
                                    this.CSP = "";
                                    this.Numero_Nao_Reconhecido = numero_original_nao_formatado;

                                }
                            }
                            else
                            {
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                    }
                    //setimo caso +5504112341234 -> +55 41 1234-1234
                    //            +5541912341234 -> +55 41 91234-1234
                    //            01441912341234 -> +55 14 41 91234-1234
                    //            90904112341234 -> 9090 41 1234-1234
                    //            90144112341234 -> 90 14 41 12341234 (14 = csp)
                    else if (SizeofCelular == 14)
                    {
                        if (numero[0] == '+')
                        {
                            if (numero[3] == '0') //+5504112341234
                            {
                                this.Pais = numero.Remove(3);
                                this.Regiao = numero.Remove(0, 4);
                                this.Regiao = this.Regiao.Remove(2);
                                this.Numero = numero.Remove(0, 6);
                                this.Formatacao_PrefixoLongaDistanciaNacional = true;
                            }
                            else //+5541912341234
                            {
                                if (numero[6] >= '6' && numero[6] <= '9' && numero[5] == '9')
                                {
                                    this.Pais = numero.Remove(3);
                                    this.Regiao = numero.Remove(0, 3);
                                    this.Regiao = this.Regiao.Remove(2);
                                    this.Numero = numero.Remove(0, 5);
                                }
                                else
                                {
                                    this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                }
                            }
                        }
                        else if(ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarInterurbano)
                        {
                            //nesse caso precisamos checar o csp
                            this.CSP = numero.Remove(0, 2);
                            this.CSP = this.CSP.Remove(2);
                            if(Operadora.ValidaPrestadora(CSP) !=0) 
                            {
                                // 90144112341234
                                this.Regiao = numero.Remove(0, 4);
                                this.Regiao = this.Regiao.Remove(2);
                                this.Numero = numero.Remove(0, 6);
                                this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarInterurbano;

                            }
                            else
                            {
                                //não é uma csp
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                this.CSP = "";
                            }

                        }
                        else if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarLocal)
                        {
                                // 90904112341234 -> +51 9090 41 12341234
                                this.Pais = "+55";
                                this.Regiao = numero.Remove(0, 4);
                                this.Regiao = this.Regiao.Remove(2);
                                this.Numero = numero.Remove(0, 6);
                                this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;
                        }
                        else if(numero[0] == '0') //01441912341234
                        {
                            // achamos um zero, então pode conter um CSP nos próximos dois digitos.
                            this.CSP = numero.Remove(3);
                            this.CSP = this.CSP.Remove(0, 1);
                            if (Operadora.ValidaPrestadora(this.CSP) != 0)
                            {
                                if (numero[6] >= '6' && numero[6] <= '9' && numero[5] == '9')
                                {
                                    //confirmando que tem o nono digito e é um celular
                                    // 01441912341234 -> +55 14 41 91234-1234
                                    this.Pais = "+55";
                                    this.Regiao = numero.Remove(0, 3);
                                    this.Regiao = this.Regiao.Remove(2);
                                    this.Numero = numero.Remove(0, 5);
                                    this.Formatacao_PrefixoLongaDistanciaNacional = true;
                                }
                                else
                                {
                                    this.CSP = "";
                                    this.Numero_Nao_Reconhecido = numero_original_nao_formatado;

                                }
                            }
                            else
                            {
                                this.CSP = "";
                                this.Numero_Nao_Reconhecido = numero_original_nao_formatado;

                            }
                        }
                        else 
                        {
                            this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                        }
                    }
                    // oitavo caso: 900144112341234 -> 90 014 41 12341234 (014 = csp)
                    //            esse caso existe? não achei informação sobre o mesmo

                    /*                    else if (SizeofCelular == 15)
                                        {
                                            if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarInterurbano) //interurbano e longa distancia tem o mesmo modelo de escrita
                                            {
                                                if(numero[2] == '0')
                                                {
                                                    this.CSP = numero.Remove(0, 2);
                                                    this.CSP = this.CSP.Remove(2);
                                                    if(Operadora.ValidaPrestadora(this.CSP) != 0)
                                                    {
                                                        this.Regiao = numero.Remove(0, 5);
                                                        this.Regiao = this.Regiao.Remove(2);
                                                        this.Numero = numero.Remove(0, 7);
                                                        this.Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarInterurbano;
                                                    }
                                                    else
                                                    {
                                                        this.CSP = "";
                                                        this.Numero_Nao_Reconhecido = numero;
                                                    }
                                                }
                                                else
                                                {
                                                    this.Numero_Nao_Reconhecido = numero;
                                                }

                                            }
                                        }
                     */
                    else
                    {
                        this.Numero_Nao_Reconhecido = numero_original_nao_formatado;
                    }

                }
            }
            else
            {
                Servico = numero;
            }
            #endregion

        }

/*        public void AlteraNumero(string Numero)
        {
            if (Numero.Count() == 8 || Numero.Count() == 9)
            {
                this.Numero = Numero;
            }
        }
        */

        public bool SetCSP(string CSP)
        {
            bool Saida = false;
            if (this.Numero_Nao_Reconhecido.Count() != 0)
            {
                if(Operadora.ValidaPrestadora(CSP) != 0)
                {
                    this.CSP = CSP;
                    Saida = true;
                }
            }
            return Saida;
        }

        /// <summary>
        /// Só chame esta função com a certeza de ter 4 ou mais números
        /// || referencia o número porem não altera os dados.
        /// </summary>
        /// <param name="numero"></param>
        /// <returns></returns>
        private ChamadaACobrar ENoventaNoventa(ref string numero)
        {
            ChamadaACobrar Saida = ChamadaACobrar.NaoCobrar;
            if(numero[0] == '9' && numero[1] == '0')
            {
                if(numero[2] == '9' && numero[3] == '0')
                {
                    Saida = ChamadaACobrar.CobrarLocal;
                }
                else
                {
                    Saida = ChamadaACobrar.CobrarInterurbano;
                }
            }
            return Saida;
        }

        private bool EhInternacional(ref string numero)
        {
            bool saida = false;
            if(numero.Count() > 2)
            {
                if(numero[0] == '+') //previamente já é filtrado 00XX para +XX 
                {
                    if(numero[1] == '5' && numero[2] == '5')
                    {
                        // nacional
                    }
                    else
                    {
                        saida = true;
                    }
                }
            }
            return saida;
        }


    }

}
