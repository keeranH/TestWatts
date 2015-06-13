using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Enum;

namespace Econocom.Helper.Authorisation
{
    public class AuthorisationHelper
    {
        public bool EstAdministrateur(Contact contact)
        {
            return (contact.Authorisations == (int) TypeContactEnum.Administrateur);
        }

        public bool EstSouscripteur(Contact contact)
        {
            return (contact.Authorisations == (int)TypeContactEnum.Souscripteur);
        }

        public bool EstUtilisateur(Contact contact)
        {
            return (contact.Authorisations == (int)TypeContactEnum.Utilisateur);
        }

        public bool EstWebmaster(Contact contact)
        {
            return (contact.Authorisations == (int)TypeContactEnum.WebMaster);
        }

        public bool EstCommercial(Contact contact)
        {
            return (contact.Authorisations == (int)TypeContactEnum.Commercial);
        }

        public bool EstSouscripteurAdmin(Contact contact)
        {
            return (contact.Authorisations == (int)TypeContactEnum.SouscripteurAdmin);
        }

    }


}
