using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace _9Contatos.API.Outlook
{

    // helper class
    public class Dummy
    {
        public String Field { get; set; }
    }

    /**
     * Estrutura de dados em Json, só que tem dados não reconhecidos, creio que sejam dinamicos...
     */
    class Contato_Outlook_Struct
    {
        public string Id;
        public string displayName;
        public List<string> homePhones;//mesmo sendo uma lista, eu só vi ele funcionar com um único elemento
        public string mobilePhone; //só pega um, não consegue pegar mais que um pelo visto
        public List<string> businessPhones;//mesmo sendo uma lista, eu só vi ele funcionar com um único elemento
    }

   
    /// <summary>
    /// Não se aplica a todos os casos e sim a um conjunto específico de dados
    /// </summary>
    class Contatos_Outlook
    {
        public List<Contato_Outlook_Struct> Contatos = new List<Contato_Outlook_Struct>();

        public void Carrega(string Json)
        {
            string []remove_value = new string[] { "@odata.etag" };
            string[] Contatos_Nao_Formatados = Json.Split(remove_value, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < Contatos_Nao_Formatados.Count(); i++)
            {
                Contatos.Add(new Contato_Outlook_Struct());
                Contatos[i-1].Id = Carrega_Id(ref Contatos_Nao_Formatados[i]);
                Contatos[i-1].displayName = Carrega_displayName(ref Contatos_Nao_Formatados[i]);
                Contatos[i-1].homePhones = Carrega_homePhones(ref Contatos_Nao_Formatados[i]); // a ser feito
                Contatos[i-1].mobilePhone = Carrega_MobilePhone(ref Contatos_Nao_Formatados[i]);
                Contatos[i-1].businessPhones = Carrega_businessPhones(ref Contatos_Nao_Formatados[i]);
            }
            //quebramos o json em duas partes, o cabeçalho e os dados dos contatos.

        }
        private List<string> Carrega_homePhones(ref string Local)
        {
            List<string> HPs = new List<string>();
            //1 parte, achar onde inicia o homePhones
            int Indice = Local.IndexOf("homePhones") + 13;
            //2 parte, achar onde termina o homePhones.
            int Indice2 = Local.IndexOf("]",Indice);
            if(Indice  != Indice2)
            {
                //não vazio
                string tmp = Local.Substring(Indice, Indice2 - Indice);
                string[] QuebrarSimbolos = new string[] { "," };
                string [] Numeros = tmp.Split( QuebrarSimbolos, StringSplitOptions.RemoveEmptyEntries);
                for(int i=0;i< Numeros.Count();i++)
                {
                    //Vamos remover algum simbolo que tenha sobrado, mesmo o parser do Telefone tendo capacidade de filtrar ele...
                    for(int j=0;j<Numeros[i].Count(); j++)
                    {
                        if(Numeros[i][j] == '\\' || Numeros[i][j] == '"')
                        {
                            Numeros[i] = Numeros[i].Remove(j, 1);
                        }
                    }
                    HPs.Add(Numeros[i]);
                }
            }
            return HPs;
        }

        private List<string> Carrega_businessPhones(ref string Local)
        {
            List<string> BPs = new List<string>();
            //1 parte, achar onde inicia o businessPhones
            int Indice = Local.IndexOf("businessPhones") + 17;
            //2 parte, achar onde termina o businessPhones.
            int Indice2 = Local.IndexOf("]}");
            if (Indice != Indice2)
            {
                //não vazio
                string tmp = Local.Substring(Indice, Indice2 - Indice);
                string[] QuebrarSimbolos = new string[] { "," };
                string[] Numeros = tmp.Split(QuebrarSimbolos, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < Numeros.Count(); i++)
                {
                    //Vamos remover algum simbolo que tenha sobrado, mesmo o parser do Telefone tendo capacidade de filtrar ele...
                    for (int j = 0; j < Numeros[i].Count(); j++)
                    {
                        if (Numeros[i][j] == '\\' || Numeros[i][j] == '"')
                        {
                            Numeros[i] = Numeros[i].Remove(j, 1);
                        }
                    }
                    BPs.Add(Numeros[i]);
                }
            }
            return BPs;

        }

        private string Carrega_Id(ref string Local)
        {
            //Id é uma primary key então sempre vai ter uma.
            //1a parte, achar onde está o Id
            int Indice = Local.IndexOf("id\":") +5;
            //2a parte, encontrar onde termina a ID.
            int Indice2 = Local.IndexOf("\"", Indice);
            //3a parte, copiar a Id.
            return Local.Substring(Indice, Indice2 - Indice);

        }

        private string Carrega_displayName(ref string Local)
        {
            //1a parte, achar onde está o displayName
            int Indice = Local.IndexOf("displayName") + 14;
            //2a parte, achar onde termina o displayName.
            int Indice2 = Local.IndexOf("\"", Indice);
            //3a parte, copiar o displayName
            if (Indice == Indice2)
            {
                //sem nome
                return "";
            }
            string str =  Local.Substring(Indice, Indice2 - Indice);

            if (str != "")
            {
                str = str.Replace(@"\\", @"\");
                str =  Regex.Replace(str, @"\\u([\dA-Fa-f]{4})", v => ((char)Convert.ToInt32(v.Groups[1].Value, 16)).ToString());
            }
            return str;
        }
        private int Achar_Indice(ref string Str, string Cmp, int inicio)
        {
            int outp= -1;
            int i, j;
            if (Cmp != null && Str != null)
            {
                for (i = 0; i < Str.Count() - Cmp.Count(); i++)
                {
                    for (j=0; j < Cmp.Count() && j+i < Str.Count() ;j++)
                    {
                        if(Str[i+j] != Cmp[j])
                        {
                            j = Cmp.Count() + 10;
                        }
                    }
                    if(j == Cmp.Count())
                    {
                        outp = i;
                        i = Str.Count();
                    }
                }
            }
            return outp;
        }

        private string Carrega_MobilePhone(ref string Local)
        {
            //1a parte, achar onde inicia o mobilePhone
            int Indice = Local.IndexOf("mobilePhone") + 13;
            //2a parte, achar onde termina o mobilePhone.
            int Indice2 = Local.IndexOf(",\"", Indice);
            //3a parte, copiaro  mobilePhone.
            if (Indice == Indice2)
            {
                return  "";
            }
            else if(Local[Indice2 -1] == '"')
            {
                Indice2 -= 1;
                Indice += 1;
            }
            string ret = Local.Substring(Indice, Indice2 - Indice);
                if (ret == "null")
                {
                    ret = "";
                }
            return ret;
        }

    }
}
