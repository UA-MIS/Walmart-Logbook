using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class LocationDetailViewModel
    {
        public Location location { get; set; }
        public IEnumerable<Container> containers { get; set; }
    }
}