//FONTE para estruturação de números: http://www.teleco.com.br/pdfs/tutorialnum.pdf
//
// Parser NÚMERO DE TELEFONE, POR: Lucas Zimerman Fraulob
//

using System.Linq;
using _9Contatos.Telefones.operadora;
using System;

namespace _9Contatos.Telefones.telefone
{
    enum ChamadaACobrar
    {
        CobrarLocal,
        CobrarInterurbano,
        NaoCobrar
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

        public string Get_Numero_Formatado(bool ManterFormatacao, bool TracoNumero /* - */, bool EspacoEntreDados, bool AspasEntreRegiao, bool PrefixoLongaDistanciaNacional, ref string DDD_Nacional, bool OcultaDDD_Nacional, bool Oculta_Pais)
        {
            string Numero_Formatado = "";
            string Espaco = "";
            string tmp = "";
            if (EspacoEntreDados == true || (ManterFormatacao == true && Formatacao_EspacoEntreNumero == true))
            {
                //Marca se o usuário quer todos os os códigos de telefone juntos ou separado.
                Espaco = " ";
            }
            if (Servico.Count() > 0)
            {
                //190, 199,...
                //Serviços Publicos de Emergencia
                Numero_Formatado = Servico;
            }
            else if (Numero_Nao_Reconhecido.Count() > 0)
            {
                // ex: 1 , 9234u923084902849023848098409181908
                // Número internacional não cai aqui
                Numero_Formatado = Numero_Nao_Reconhecido;
            }
            else if (Numero_Nao_Geografico.Count() > 0) 
            {
                // 0800 ...
                Numero_Formatado = Get_Numero_Nao_Geografico(ManterFormatacao, TracoNumero);
            }
            else if (Numero_Internacional.Count() > 0)
            {
                Numero_Formatado = Numero_Internacional;
            }
            else if (Numero != "")
            {
                //Caso de um número de telefone padrão
                Numero_Formatado = Get_Numero(ManterFormatacao, TracoNumero);
                tmp = Get_Regiao(ManterFormatacao, AspasEntreRegiao, ref DDD_Nacional, OcultaDDD_Nacional, Oculta_Pais);
                if(tmp.Count() > 0)
                {
                    Numero_Formatado = tmp + Espaco + Numero_Formatado;
                }
                tmp = Get_CSP(Oculta_Pais);
                if (tmp.Count() > 0)
                {
                    Numero_Formatado = tmp + Espaco + Numero_Formatado;
                }
                tmp = Get_NumeroAcobrar(false);
                if (tmp.Count() > 0)
                {
                    Numero_Formatado = tmp + Espaco + Numero_Formatado;
                }
                tmp = Get_Pais(Oculta_Pais);
                if (tmp.Count() > 0)
                {
                    Numero_Formatado = tmp + Espaco + Numero_Formatado;
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
            int SizeofCelular = 0;
            int Cnt_Aspas = 0;
            int Cnt_Traco = 0;
            string numero_original_nao_formatado = numero;
            if (numero != null) //evita ter um número de telefone nulo e transforma ele me vazio
            {
                SizeofCelular = numero.Count();
            }
            if (SizeofCelular >= 8)
            {
                #region Filtros (removemos simbolos idevidos e também checamos a presença de outros
                for (int i = 0; i < numero.Count(); i++)
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
                if (SizeofCelular > 2)
                {
                    if (numero[0] == '0' && numero[1] == '0')
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
                    Numero_Nao_Reconhecido = numero_original_nao_formatado;
                }
                else if (EhInternacional(ref numero) == true)
                {
                    Numero_Internacional = numero_original_nao_formatado;
                }
                else
                {
                    //primerio caso 12341234 -> 1234-1234
                    if (SizeofCelular == 8)
                    {
                        Numero = numero;
                    }
                    //segundo caso 912341234 -> 91234-1234
                    else if (SizeofCelular == 9)
                    {
                        if (numero[0] == '9')
                        {
                            if (numero[1] >= '6' && numero[1] <= '9')
                            { //é um celular
                                Numero = numero;
                            }
                            else
                            { //ai temos um numero alienigena
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                        else
                        {
                            Numero_Nao_Reconhecido = numero_original_nao_formatado;
                        }
                    }
                    //terceiro caso 4112341234 -> 41 1234-1234
                    else if (SizeofCelular == 10)
                    {
                        {
                            Regiao = numero.Remove(2);
                            Numero = numero.Remove(0, 2);
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
                                Numero_Nao_Geografico = numero.Remove(4);
                                Numero = numero.Remove(0, 4);
                            }
                            else
                            {
                                //04112341234 -> +55 41 12341234
                                Pais = "+55";
                                Regiao = numero.Remove(3);
                                Regiao = Regiao.Remove(0, 1);
                                Numero = numero.Remove(0, 3);
                                Formatacao_PrefixoLongaDistanciaNacional = true;
                            }
                        }
                        else if (numero[0] == '+')
                        {
                            //+5512341234
                            Pais = numero.Remove(3);
                            Numero = numero.Remove(0, 3);
                        }
                        else
                        {
                            if (numero[3] >= '6' && numero[3] <= '9' && numero[2] == '9') // 41912341234
                            {
                                Regiao = numero.Remove(2);
                                Numero = numero.Remove(0, 2);
                            }
                            else if (Formatacao_TracoNumero == true) // 117777-7777
                            {
                                Regiao = numero.Remove(2);
                                Numero = numero.Remove(0, 2);
                            }
                            else
                            {
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;

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
                                Pais = "+55";
                                Regiao = numero.Remove(3);
                                Regiao = Regiao.Remove(0, 1);
                                Numero = numero.Remove(0, 3);
                                Formatacao_PrefixoLongaDistanciaNacional = true;
                            }
                            else
                            {
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                        else
                        {
                            CSP = numero.Remove(2);
                            if (Operadora.ValidaPrestadora(CSP) != 0)
                            {
                                // 144122221111 -> 14 41 2222-1111
                                Regiao = numero.Remove(4);
                                Regiao = Regiao.Remove(0, 2);
                                Numero = numero.Remove(0, 4);
                            }
                            else if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarLocal)
                            {
                                // 909012341234
                                CSP = "";
                                Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;
                                Numero = numero.Remove(0, 4); //  tira o 9090
                                Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;
                            }
                            else if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarInterurbano)
                            {
                                //904112341234
                                //Modelo invalido então vamos corrigir para
                                //909012341234
                                //se a região for a do usuário ou 
                                //90 41 12341234
                                //caso contrario
#warning falta tratar os casos comentados acima
                                CSP = "";
                                Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarInterurbano;
                                Regiao = numero.Remove(0, 2);
                                Regiao = Regiao.Remove(2);
                                Numero = numero.Remove(0, 4); //  tira o 90 e a região
                            }
                            else
                            {
                                CSP = "";
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;
                            }
                        }
                    }
                    // sexto caso +554112341234 -> +55 41 1234-1234
                    //            0144112341234 -> +55 14 41 12341234 (onde 14 é o CSP)
                    //            1441982341234 -> +55 14 41 982341234
                    //            9090912341234 -> 9090 912341234       
                    //            !!! 9004112341234 -> 90 041 12341234  (não é válido pois precisa do CSP para fazer chamadas a cobrar a distancia
#warning reavaliar ultimo caso invalido do comentario
                    else if (SizeofCelular == 13)
                    {
                        if (numero[0] == '+')
                        {
                            Pais = numero.Remove(3);
                            Regiao = numero.Remove(0, 3);
                            Regiao = Regiao.Remove(2);
                            Numero = numero.Remove(0, 5);
                        }
                        else
                        {
                            if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarLocal)
                            {
                                if (numero[5] >= '6' && numero[5] <= '9' && numero[4] == '9')
                                {
                                    //9090 912341234
                                    Numero = numero.Remove(0, 4);
                                    Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;

                                }
                                else // tem o 9090 mas o proximo digito não é o nono digito, então não sabemos o que é, ou tem o nono digito entretanto é um numero fixo.
                                {
                                    Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                }
                            }
                            else if (numero[0] == '0')
                            {// achamos um zero, então pode conter um CSP nos próximos dois digitos.
                                CSP = numero.Remove(3);
                                CSP = CSP.Remove(0, 1);
                                if (Operadora.ValidaPrestadora(CSP) != 0)
                                {
                                    // 0144112341234 -> +55 14 41 1234-1234
                                    Pais = "+55";
                                    Regiao = numero.Remove(0, 3);
                                    Regiao = Regiao.Remove(2);
                                    Numero = numero.Remove(0, 5);
                                    Formatacao_PrefixoLongaDistanciaNacional = true;
                                }
                                else
                                {
                                    CSP = "";
                                    Numero_Nao_Reconhecido = numero_original_nao_formatado;

                                }
                            }
                            else if (Operadora.ValidaPrestadora(numero.Remove(2)) != 0)
                            {
                                //é UM CSP
                                CSP = numero.Remove(2);
                                Regiao = numero.Remove(4);
                                Regiao = Regiao.Remove(0, 2);
                                if (regiao.Regiao.ValidaRegiao(Regiao) != 0)
                                {
                                    if (numero[5] >= '6' && numero[5] <= '9' && numero[4] == '9')
                                    {
                                        //14 41 98234-1234
                                        this.Numero = numero.Remove(0, 4);

                                    }
                                    else
                                    {
                                        Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                    }
                                }
                                else
                                {
                                    Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                }

                            }
                            else
                            {
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;
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
                                Pais = numero.Remove(3);
                                Regiao = numero.Remove(0, 4);
                                Regiao = Regiao.Remove(2);
                                Numero = numero.Remove(0, 6);
                                Formatacao_PrefixoLongaDistanciaNacional = true;
                            }
                            else //+5541912341234
                            {
                                if (numero[6] >= '6' && numero[6] <= '9' && numero[5] == '9')
                                {
                                    Pais = numero.Remove(3);
                                    Regiao = numero.Remove(0, 3);
                                    Regiao = Regiao.Remove(2);
                                    Numero = numero.Remove(0, 5);
                                }
                                else
                                {
                                    Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                }
                            }
                        }
                        else if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarInterurbano)
                        {
                            //nesse caso precisamos checar o csp
                            CSP = numero.Remove(0, 2);
                            CSP = CSP.Remove(2);
                            if (Operadora.ValidaPrestadora(CSP) != 0)
                            {
                                // 90144112341234
                                Regiao = numero.Remove(0, 4);
                                Regiao = Regiao.Remove(2);
                                Numero = numero.Remove(0, 6);
                                Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarInterurbano;

                            }
                            else
                            {
                                //não é uma csp
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;
                                CSP = "";
                            }

                        }
                        else if (ENoventaNoventa(ref numero) == ChamadaACobrar.CobrarLocal)
                        {
                            // 90904112341234 -> +51 9090 41 12341234
                            Pais = "+55";
                            Regiao = numero.Remove(0, 4);
                            Regiao = Regiao.Remove(2);
                            Numero = numero.Remove(0, 6);
                            Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarLocal;
                        }
                        else if (numero[0] == '0') //01441912341234
                        {
                            // achamos um zero, então pode conter um CSP nos próximos dois digitos.
                            CSP = numero.Remove(3);
                            CSP = CSP.Remove(0, 1);
                            if (Operadora.ValidaPrestadora(CSP) != 0)
                            {
                                if (numero[6] >= '6' && numero[6] <= '9' && numero[5] == '9')
                                {
                                    //confirmando que tem o nono digito e é um celular
                                    // 01441912341234 -> +55 14 41 91234-1234
                                    Pais = "+55";
                                    Regiao = numero.Remove(0, 3);
                                    Regiao = Regiao.Remove(2);
                                    Numero = numero.Remove(0, 5);
                                    Formatacao_PrefixoLongaDistanciaNacional = true;
                                }
                                else
                                {
                                    CSP = "";
                                    Numero_Nao_Reconhecido = numero_original_nao_formatado;

                                }
                            }
                            else
                            {
                                CSP = "";
                                Numero_Nao_Reconhecido = numero_original_nao_formatado;

                            }
                        }
                        else
                        {
                            Numero_Nao_Reconhecido = numero_original_nao_formatado;
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
                                                    CSP = numero.Remove(0, 2);
                                                    CSP = CSP.Remove(2);
                                                    if(Operadora.ValidaPrestadora(CSP) != 0)
                                                    {
                                                        Regiao = numero.Remove(0, 5);
                                                        Regiao = Regiao.Remove(2);
                                                        Numero = numero.Remove(0, 7);
                                                        Contem_Numero_A_Cobrar = ChamadaACobrar.CobrarInterurbano;
                                                    }
                                                    else
                                                    {
                                                        CSP = "";
                                                        Numero_Nao_Reconhecido = numero;
                                                    }
                                                }
                                                else
                                                {
                                                    Numero_Nao_Reconhecido = numero;
                                                }

                                            }
                                        }
                     */
                    else
                    {
                        Numero_Nao_Reconhecido = numero_original_nao_formatado;
                    }
                }
            }
            else if (numero == null)
            {
                // não carrega o serviço como um valor nulo
            }
            else
            {
                Servico = numero;
            }
            #endregion
        }

        #region Funções para Formatação de número

        private string Get_Numero_Nao_Geografico(bool ManterFormatacao, bool TracoNumero)
        {
            string N = Servico;
            if (TracoNumero == true || ManterFormatacao == true && Formatacao_TracoNumero == true)
            {
               N = N.Insert(N.Count() - 3, "-");
            }
            return N;
        }

        private string Get_Numero(bool ManterFormatacao, bool TracoNumero)
        {
            string N = Numero;
            if(TracoNumero == true      ||      ManterFormatacao == true && Formatacao_TracoNumero == true)
            {
                N = N.Insert(N.Count() - 4, "-");
            }
            return N;
        }

        private string Get_Regiao(bool ManterFormatacao /*Não está em uso, aplicar posteriormente*/, bool AspasEntreRegiao, ref string Meu_DDD, bool Oculta_MeuDDD, bool Oculta_Pais)
        {
            string R = Regiao;
            if(R == "" && Oculta_MeuDDD == false)
            {
                R = Meu_DDD;
            }
            if(Oculta_MeuDDD == true && (R == Meu_DDD || R == "") )
            {
                //sendo o DDD vazio é dito que o R é igual ao Meu_DDD
                R = "";
            }
            else if(Oculta_Pais == true && Pais == "")
            {

                if (CSP == "")
                {
                    //precisa colocar o '0' na região
                    R = '0' + R;
                }
                if (AspasEntreRegiao == true)
                {
                    R = '(' + R + ')';
                }

            }
            else if(R != "")
            {
                //não precisa colocar o '0' na região pois temos no número o +55
                if(AspasEntreRegiao == true)
                {
                    R = '(' + R + ')';
                }
            }
            return R;
        }

        private string Get_CSP(bool Oculta_Pais)
        {
            string C = CSP;
            // única alteração que precisa ser feita é se não tem o dado do +55 e se é pedido para ocultar o dado do pais.
            // O digito 0 antes do CSP não precisa ser checado na região pois o mesmo é testado na região, então não precisamos testar duas vezes a mesma coisa.
            if(C.Count() > 0)
            {
                //só checamos o pais se tiver o CSP
                if(Pais.Count() == 0 && Oculta_Pais == true)
                {

                    C = '0' + C;
                }
            }
            return C;
        }

        private string Get_NumeroAcobrar(bool Remover_NumeroACobrar)
        {
            string NovNov = "";
            if(!Remover_NumeroACobrar)
            {
                if(Contem_Numero_A_Cobrar == ChamadaACobrar.CobrarInterurbano)
                {
                    NovNov = "90";
                }
                else if(Contem_Numero_A_Cobrar == ChamadaACobrar.CobrarLocal)
                {
                    NovNov = "9090";
                }
            }
            return NovNov;
        }

        private string Get_Pais(bool Oculta_Pais)
        {
            string P = "";
            if(Oculta_Pais == true && Pais.Count() > 0)
            {

            }
            else if(Oculta_Pais == false && Pais.Count() == 0)
            {
                P = "+55";
            }
            else
            {
                P = Pais;
            }
            return P;
        }

        #endregion



        public bool SetCSP(string CSP)
        {
            bool Saida = false;
            if (Numero_Nao_Reconhecido.Count() != 0)
            {
                if(Operadora.ValidaPrestadora(CSP) != 0)
                {
                    CSP = CSP;
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
