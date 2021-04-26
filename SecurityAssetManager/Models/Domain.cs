using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecurityAssetManager.Models
{
    public class Domain
    {
        [Key]
        public Guid DomainID { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }

        public virtual ICollection<UserDomain> UserDomains { get; set; }

    }
}