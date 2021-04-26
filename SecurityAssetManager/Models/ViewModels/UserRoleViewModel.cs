using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}