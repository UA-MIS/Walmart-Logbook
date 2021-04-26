using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SecurityAssetManager.CustomValidation;

namespace SecurityAssetManager.Models.ViewModels
{
    public class DupItemViewModel
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public String Barcode { get; set; }
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public String NewBarcode { get; set; }
        [Required]
        public Guid ContainerId { get; set; }
        public Container Container { get; set; }
        public string KeyHolder { get; set; }
        [Required]
        public string Password { get; set; }
    }
}