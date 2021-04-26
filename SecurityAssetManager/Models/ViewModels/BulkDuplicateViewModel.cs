using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SecurityAssetManager.CustomValidation;

namespace SecurityAssetManager.Models.ViewModels
{
    public class BulkDuplicateViewModel
    {
        public string CurrentContainerName { get; set; }
        public Guid CurrentContainerID { get; set; }
        public Guid NewContainerID { get; set; }
        public string Password { get; set; }
        public string KeyHolder { get; set; }
    }
}