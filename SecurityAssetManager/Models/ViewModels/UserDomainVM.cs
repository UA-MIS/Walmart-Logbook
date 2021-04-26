using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class UserDomainVM
    {
        public Guid? UserID { get; set; }
        public Guid DomainID { get; set; }
    }
}