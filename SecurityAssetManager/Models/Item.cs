using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace SecurityAssetManager.Models
{
    public class Item : IValidatableObject
    {

        [Key]
        public Guid ItemID { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [Display(Name = "Item Name")]
        public String Name { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public String Description { get; set; }

        [Required]
        public Boolean Status { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9)]
        [Index("Ix_Barcode", Order = 1, IsUnique = true)]
        //[Remote("IsBarcodeAvailble", "ItemsController", ErrorMessage = "Barcode already exists in database. Please try again.")]
        public String Barcode { get; set; }


        public Boolean isActive { get; set; }

        [ForeignKey("Container")]
        [Required]
        public Guid ContainerID { get; set; }
        public virtual Container Container { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var validateBarcode = db.Items.FirstOrDefault
            (x => x.Barcode == Barcode && x.ItemID != ItemID);
            if (validateBarcode != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Barcode already exists in database. Please try again.", new[] { "Barcode" });
                yield return errorMessage;
            }
            else
            {
                yield return ValidationResult.Success;
            }
        }


    }
}