using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;

using Client = Econocom.Model.Models.Benchmark.Client;

namespace Econocom.Model.ViewModel
{
    public class RegistrationClientModel
    {
        public int Id { get; set; }
        [Required]
        public string Nom { get; set; }
        
        [Required]
        public string IdentificationNational{ get; set; }

        [Required]
        public string Group { get; set; }
        
        [Required]
        public string TVAIntraComm { get; set; }

        [Required]
        public string CODENAF { get; set; }
        public string Type { get; set; }

        [Required]
        public string RaisonSociale { get; set; }

        public System.DateTime DateDebut { get; set; }
        public Nullable<System.DateTime> DateFin { get; set; }

      
        public Nullable<int> AdresseId { get; set; }
        public Nullable<int> SecteurActiviteId { get; set; }

        [UIHint("Adresse")]
        public virtual AdresseViewModel Adresse { get; set; }
       
        public virtual List<ContactViewModel> Contacts { get; set; }

        public RegistrationClientModel()
        {
            this.Adresse = new AdresseViewModel();          
        }

        public Client ToClient()
        {
            try
            {
                var client = new Client
                {   
                    Id =  this.Id,
                    Nom = this.Nom,
                    IdentificationNational = this.IdentificationNational,
                    Groupe = this.Group,
                    TVAIntraComm = this.TVAIntraComm,
                    CodeNAF = this.CODENAF,
                    RaisonSociale = this.RaisonSociale,
                    DateDebut = DateTime.Now,
                    DateFin = DateTime.Now,
                    AdresseId = this.AdresseId,
                    SecteurActiviteId = this.SecteurActiviteId
                };
                //A initialiser par moderateur ou par client ou par code ??

                var adresse = new Adresse();
                if (this.Adresse != null)
                {
                    adresse.Id = this.Adresse.Id;
                    adresse.Adresse1 = this.Adresse.Adresse1;
                    adresse.Adresse2 = this.Adresse.Adresse2;
                    adresse.Adresse3 = this.Adresse.Adresse3;
                    adresse.CodePostal = this.Adresse.CodePostal;
                    adresse.Region = this.Adresse.Region;
                    adresse.Ville = this.Adresse.Ville;
                    adresse.PaysId = this.Adresse.PaysId;
                    client.Adresse = adresse;
                }
                else
                {
                    return null;
                }
                
                if (this.Contacts != null && this.Contacts.Any())
                {
                  foreach (ContactViewModel contactView in Contacts)
                  {
                      var contact = new Contact();
                      contact.Id = contactView.Id.Value;
                      contact.MotPasse = contactView.MotPasse;
                      contact.NomContact = contactView.NomContact;
                      contact.NumeroGSM = contactView.NumeroGSM;
                      contact.NumeroPhone = contactView.NumeroPhone;
                      contact.PrenomContact = contactView.PrenomContact;
                      contact.Fonction = contactView.Fonction;
                      contact.Authorisations = contactView.Authorisations;
                      contact.Email = contactView.Email;
                      contact.ClientId = contactView.ClientId;
                      //contact. = contactView.ReponseId;
                      contact.TypeContactId = contactView.TypeContactId;
                      client.Contacts.Add(contact);
                  }
                   
                }
                else
                {
                    return null;
                }


                return client;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

   
}
