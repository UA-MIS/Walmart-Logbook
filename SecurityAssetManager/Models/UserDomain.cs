using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityAssetManager.Models
{
    public class UserDomain
    {
        [Key]
        [Column(Order = 1)]
        public Guid UserID { get; set; }
        [Key]
        [Column(Order = 2)]
        public Guid DomainID { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Domain Domain { get; set; }

        public bool Selected { get; set; }
    }
}