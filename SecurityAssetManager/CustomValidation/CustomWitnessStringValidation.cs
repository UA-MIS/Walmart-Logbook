using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.CustomValidation
{
    public class CustomWitnessStringValidation:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value.Equals("Witness123");
        }
    }
}