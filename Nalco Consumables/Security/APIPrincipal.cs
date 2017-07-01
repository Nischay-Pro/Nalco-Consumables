using System.Security.Principal;

namespace Nalco_Consumables.Security
{
    public class APIPrincipalAdmin : IPrincipal
    {
        public APIPrincipalAdmin(string userName)
        {
            UserName = userName;
            Identity = new GenericIdentity(userName);
        }

        public IIdentity Identity { get; set; }

        public string UserName { get; set; }

        public bool IsInRole(string role)
        {
            if (role.Equals("Administrator"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class APIPrincipalUser : IPrincipal
    {
        public APIPrincipalUser(string userName)
        {
            UserName = userName;
            Identity = new GenericIdentity(userName);
        }

        public IIdentity Identity { get; set; }

        public string UserName { get; set; }

        public bool IsInRole(string role)
        {
            if (role.Equals("User"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}