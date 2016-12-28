using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using _9Contatos.Classe;

namespace _9Contatos.Codigo
{
    /// <summary>
    /// Objeto utilizado para interagir com o App Pessoas diretamente
    /// </summary>
    class PeopleAPI
    {
        private List<Contact> Contatos = new List<Contact>();

        /// <summary>
        ///  Função que altera um contato.
        /// </summary>
        /// <param name="Contato_a_Mudar"> O objeto contato que será alterado</param>
        /// <param name="NomeCompletos">O nome completo do contato a ser alterado (validação)</param>
        /// <param name="TelefonesNovos">Os números de telefone novos a serem alterados</param>
        /// <param name="TelefonesAntigos">Os números antigos de telefones (validação)</param>
        /// <returns>
        ///         0 = Feito
        ///         -1 = contagem de telefones antigos difere do número no contato_a_mudar
        ///         -2 o contagem de telefones novos difere do número no contato_a_mudar
        ///         -3 = Nome do contato se difere ao nomecompleto a ser validado
        /// </returns>
        public async Task<int> AlterarContato(Contact Contato_a_Mudar, string NomeCompleto, List<string> TelefonesNovos, List<string> TelefonesAntigos)
        {
            int Valido = 0;
            var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadWrite);

            var contactList = await contactStore.GetContactListAsync(Contato_a_Mudar.ContactListId);

            var contact = await contactList.GetContactAsync(Contato_a_Mudar.Id);
            if (contact.FullName != NomeCompleto)
            {
                Valido = -3;
            }
            else
            {
                if (contact.Phones.Count() != TelefonesAntigos.Count()) // não vamos alterar os números caso se o total de telefones for invalido
                {
                    Valido = -1;
                }
                else
                {
                    if (contact.Phones.Count() != TelefonesNovos.Count()) // não vamos alterar os números caso se o total de telefones for invalido
                    {
                        Valido = -2;
                    }
                    else
                    {
                        for (int i = 0, fim = TelefonesNovos.Count(); i < fim; i++)
                        {
                            contact.Phones[i].Number = TelefonesNovos[i];
                        }
                        await contactList.SaveContactAsync(contact);
                    }
                }
            }
            return Valido;
        }

        /// <summary>
        ///  Função que altera um contato do app, linkando ao contato que deveria ser editado
        /// </summary>
        /// <param name="Contato_a_Mudar"> O objeto contato que será alterado</param>
        /// <param name="NomeCompletos">O nome completo do contato a ser alterado (validação)</param>
        /// <param name="TelefonesNovos">Os números de telefone novos a serem alterados</param>
        /// <param name="TelefonesAntigos">Os números antigos de telefones (validação)</param>
        /// <returns>
        ///         0 = Feito
        ///         -1 = contagem de telefones antigos difere do número no contato_a_mudar
        ///         -2 o contagem de telefones novos difere do número no contato_a_mudar
        ///         -3 = Nome do contato se difere ao nomecompleto a ser validado
        /// </returns>
        public async Task<int> AlterarContato_Link(Windows.ApplicationModel.Contacts.Contact Contato_a_Mudar, string NomeCompleto, List<string> TelefonesNovos, List<string> TelefonesAntigos)
        {
            Contact contato_temp;
            int Valido = 0;
            var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            var listaListasApp = await contactStore.FindContactListsAsync();
            ContactList contactList;
            contactList = listaListasApp.First();
            for (int i = 0, fim = TelefonesNovos.Count(); i < fim; i++)
            {
                Contato_a_Mudar.Phones[i].Number = TelefonesNovos[i];
            }
            contato_temp = ClonaContato(ref Contato_a_Mudar);
            await contactList.SaveContactAsync(contato_temp);

            return Valido;
        }


        /// <summary>
        ///     Carrega todos os contatos do Pessoas
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadBuffer(QualAPI api)
        {
            ContactStore allAccessStore;
            if (api == QualAPI.PeopleAPI_COM_Alteracao)
            {
                try
                {
                    allAccessStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadWrite);
                }
                catch(System.UnauthorizedAccessException)
                {
                    return false;
                }
            }
            else
            {
                allAccessStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
                if ((await allAccessStore.FindContactListsAsync()).Count == 0)
                {
                    await allAccessStore.CreateContactListAsync("Arruma Contatos");
                }
                else
                {
                    Limpa_Contatos_Temporarios();
                    ContactStore allAccessStore2;
                    //recria o link limpo
                    allAccessStore2 = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
                    await allAccessStore2.CreateContactListAsync("Arruma Contatos");
                    allAccessStore = allAccessStore2;
                }
            }
            Contatos.Clear();
            var contacts = await allAccessStore.FindContactsAsync();
            foreach (var contact in contacts)
            {
                //process aggregated contacts
                if (contact.IsAggregate)
                {
                    //here contact.ContactListId is "" (null....)                  
                    //in this case if you need the the ContactListId then you need to iterate through the raw contacts
                    if (api == QualAPI.PeopleAPI_COM_Alteracao)
                    {
                        var rawContacts = await allAccessStore.AggregateContactManager.FindRawContactsAsync(contact);
                        foreach (var rawContact in rawContacts)
                        {
                            Contatos.Add(rawContact);
                        }
                    }
                    else
                    {
                        //não precisamos do rawContacts
                        Contatos.Add(contact);
                    }
                }
                else //not aggregated contacts should work
                {
                    //                    Debug.WriteLine($"not aggregated, name: {contact.DisplayName }, ContactListId: {contact.ContactListId}");
                }
            }
            return true;
        }

        public Contato CarregaContato(int Indice)
        {
            Contato NovoContato = new Contato();
            if (Indice < this.Contatos.Count())
            {
                NovoContato.NomeCompleto = Contatos[Indice].FullName;
                for (int i = 0, fim = Contatos[Indice].Phones.Count(); i < fim; i++)
                {
                    NovoContato.Telefones_Antigos.Add(Contatos[Indice].Phones[i].Number);
                }
                NovoContato.ID = Contatos[Indice];
            }
            return NovoContato;
        }

        private Contact ClonaContato(ref Contact contato)
        {
            //Cortesia do código do Jaedson
            Contact cont = new Contact();
            foreach (var end in contato.Addresses)
            {
                cont.Addresses.Add(end);
            }
            cont.DisplayNameOverride = contato.DisplayNameOverride;
            cont.DisplayPictureUserUpdateTime = contato.DisplayPictureUserUpdateTime;
            foreach (var email in contato.Emails)
            {
                cont.Emails.Add(email);
            }
            foreach (var f in contato.Fields)
            {
                cont.Fields.Add(f);
            }
            cont.FirstName = contato.FirstName;
            cont.HonorificNamePrefix = contato.HonorificNamePrefix;
            cont.HonorificNameSuffix = contato.HonorificNameSuffix;
            foreach (var data in contato.ImportantDates)
            {
                cont.ImportantDates.Add(data);
            }
            cont.LastName = contato.LastName;
            cont.MiddleName = contato.MiddleName;
            cont.Name = contato.Name;
            cont.Nickname = contato.Nickname;
            cont.Notes = contato.Notes;

            foreach (var tel in contato.Phones)
            {   //número já foi alterado dentro do cont antes desta chamada
                cont.Phones.Add(tel);
            }
            foreach (var prop in contato.ProviderProperties)
            {
                cont.ProviderProperties.Add(prop);
            }
            cont.RingToneToken = contato.RingToneToken;
            foreach (var sig in contato.SignificantOthers)
            {
                cont.SignificantOthers.Add(sig);
            }
            cont.SourceDisplayPicture = contato.SourceDisplayPicture;
            cont.TextToneToken = contato.TextToneToken;
            cont.Thumbnail = contato.Thumbnail;
            foreach (var net in contato.Websites)
            {
                cont.Websites.Add(net);
            }
            cont.YomiFamilyName = contato.YomiFamilyName;
            cont.YomiGivenName = contato.YomiGivenName;

            return cont;
        }

        public int TotalContatos() => Contatos.Count();

        public async void Limpa_Contatos_Temporarios()
        {
            var allAccessStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            if ((await allAccessStore.FindContactListsAsync()).Count != 0)
            {
                //Limpa contatos antigos
                var listaListasApp = await allAccessStore.FindContactListsAsync();
                var lista = listaListasApp.First();
                await lista.DeleteAsync();
                //recria o link limpo
            }
        }
    }
}
