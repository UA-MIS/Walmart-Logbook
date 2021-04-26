using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecurityAssetManager.Models
{
    //Holds all attributes associated with container
    public class Container
    {
        [Key]
        public Guid ContainerID { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [Display(Name = "Container Name")]
        public String Name { get; set; }

        [ForeignKey("Location")]
        public Guid LocationID { get; set; }
        public virtual Location Location { get; set; }


        //User ID
        [Display(Name = "Key Holder")]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public Guid DomainID { get; set; }
        [ForeignKey("DomainID")]
        public virtual Domain Domain { get; set; }

        public Boolean isActive { get; set; }
    }
}