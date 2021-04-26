using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{
    public class ItemDetailViewModel
    {
        public Item item { get; set; }
        public IEnumerable<Event> events { get; set; }
    }
}