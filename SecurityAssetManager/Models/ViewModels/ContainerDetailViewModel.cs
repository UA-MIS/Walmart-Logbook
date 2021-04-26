using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SecurityAssetManager.Models.ViewModels
{ 
    public class ContainerDetailViewModel
    {
        //Used to display container details and list of items associated with it in Container Detail view
        public Container container { get; set; }
        public IEnumerable<Item> items { get; set; }
    }
}