using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecurityAssetManager.CustomValidation;

namespace SecurityAssetManager.Models.ViewModels
{
    public class CheckInOutViewModel
    {
        public Guid ItemID { get; set; }
        public string ItemName { get; set; }
        public string ContainerName { get; set; }
        public Container Container { get; set; }
        public string LocationName { get; set; }
        public Boolean Status { get; set; }
        public String Barcode { get; set; }
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public String NewBarcode { get; set; }
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