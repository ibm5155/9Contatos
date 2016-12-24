using System;
using System.Collections.Generic;

namespace _9Contatos.JSON
{
    class Contato_Oulook_Json
    {
        public string assistantName { get; set; }
        public string birthday { get; set; }
        public string businessAddress { get; set; }
        public string businessHomePage { get; set; }
        public List<string> businessPhones { get; set; }
        public List<string> categories { get; set; }
        public string changeKey { get; set; }
        public List<string> children { get; set; }
        public string companyName { get; set; }
        public string createdDateTime { get; set; }
        public string department { get; set; }
        public string displayName { get; set; }
        public List<string> emailAddresses { get; set; }
        public string fileAs { get; set; }
        public string generation { get; set; }
        public string givenName { get; set; }
        public string homeAddress { get; set; }
        public List<string> homePhones { get; set; }
        public string id { get; set; }
        public List<string> imAddresses { get; set; }
        public string initials { get; set; }
        public string jobTitle { get; set; }
        public string lastModifiedDateTime { get; set; }
        public string manager { get; set; }
        public string middleName { get; set; }
        public string mobilePhone { get; set; }
        public string nickName { get; set; }
        public string officeLocation { get; set; }
        public List<string> otherAddress { get; set; }
        public string parentFolderId { get; set; }
        public string personalNotes { get; set; }
        public string profession { get; set; }
        public string spouseName { get; set; }
        public string surname { get; set; }
        public string title { get; set; }
        public string yomiCompanyName { get; set; }
        public string yomiGivenName { get; set; }
        public string yomiSurname { get; set; }

        void Carrega(string Json)
        {
            string []remove_value = new string[] { "value" };
            string[] Contato_Filtrado = Json.Split(remove_value, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
