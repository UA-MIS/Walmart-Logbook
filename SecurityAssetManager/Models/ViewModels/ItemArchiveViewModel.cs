using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class ItemArchiveViewModel
    {
        public Guid ItemID { get; set; }
        public virtual Container Container { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ContainerID { get; set; }
        public string ContainerName { get; set; }
        public Boolean isActive { get; set; }
        public Boolean Status { get; set; }
        [Required]
        public string Witness { get; set; }
        public IEnumerable<string> Witnesses { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Justification { get; set; }
    }
}