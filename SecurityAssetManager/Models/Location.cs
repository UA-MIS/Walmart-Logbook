using System;
using System.ComponentModel.DataAnnotations;

namespace SecurityAssetManager.Models
{
    public class Location
    {
        [Key]
        public Guid LocationID { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [Display(Name = "Location Name")]
        public String Name { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public String Description { get; set; }

        public Boolean isActive { get; set; }

    }
}