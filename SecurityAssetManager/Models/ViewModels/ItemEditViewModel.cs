using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class ItemEditViewModel
    {
        public Guid ItemID { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Name { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public String Description { get; set; }
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public String Barcode { get; set; }
        [Required]
        public Guid ContainerID { get; set; }
        public string ContainerName { get; set; }
        public Container Container { get; set; }
        public Boolean Status { get; set; }
        [Required]
        public string Witness { get; set; }
        public IEnumerable<string> Witnesses { get; set; }
        [Required]
        public string Password { get; set; }
    }
}