using System.Security.Principal;

namespace Nalco_Consumables.Security
{
    public class APIPrincipal : IPrincipal
    {
        public APIPrincipal(string userName)
        {
            UserName = userName;
            Identity = new GenericIdentity(userName);
        }

        public IIdentity Identity { get; set; }
        public string UserName { get; set; }

        public bool IsInRole(string role)
        {
            if (role.Equals("user"))
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