using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class DomainDetailViewModel
    {
        public Domain domain { get; set; }
        public List<ApplicationUser> users { get; set; }
    }
}