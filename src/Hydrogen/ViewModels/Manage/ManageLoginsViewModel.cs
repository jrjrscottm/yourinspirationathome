using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using AuthenticationDescription = Microsoft.AspNetCore.Http.Authentication.AuthenticationDescription;

namespace Hydrogen.ViewModels.Manage
{
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public List<AuthenticationDescription> OtherLogins { get; set; }
    }
}
