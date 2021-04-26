using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SecurityAssetManager.Models;
using SecurityAssetManager.Models.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace SecurityAssetManager.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        // GET: Items
        /*Purpose: Return a list of items to the Items Index view 
 * Creates list of items from database that only the admin account can click the item
 * but if logged into a different account it will be a read only view of the item index .*/

        public ActionResult Index()
        {
            try
            {
                var items = new List<Item>();
                if (User.IsInRole("Admin") || User.IsInRole("Auditor")) items = db.Items.Include(i => i.Container).Where(i => i.isActive).ToList();
                else items = db.DomainFilteredItems.Include(i => i.Container).ToList();

                return View("Index", items);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Index"));
            }
        }
        //Keyholder Index

        /* Creates list of items from database that only contains the items in the keyholders containers 
         * where hey have clickable action items for CRUD functionality */
        public ActionResult KeyholderIndex()
        {
            ViewBag.FilteredItems = true;
            var items = db.Items.Include(i => i.Container).ToList().Where(i => i.Container.UserID == User.Identity.GetUserId() && i.isActive == true);
            return View("Index", items);
        }

        public ActionResult IndexFiltered(Guid? id)
        {
            var items = db.Items.Include(i => i.Container).ToList().Where(i => i.isActive == true && i.ContainerID == id);

            return View("Index", items);
        }

        // GET: Items/Details/id

        /*Purpose: Display Container data  
         * Need to check if item exists to make sure its data can be displayed 
         If ID is NOT null: 
         * Searches for item with that ID
         If container is NOT null: 
         * Creates list of items that exist under container but only for instances
         where items are present in database and container has an id         
         * Once this is done, an instance of a viewmodel can be created and returned to Detail view
         in order to allow the containers and its items to be displayed.*/
        public ActionResult Details(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find item by ID
                Item item = db.Items.Find(id);
                //If no Item is found return error
                if (item == null)
                {
                    return HttpNotFound();

                }
                //Gets events for the details page
                var events = new List<Event>();
                //If admin or auditor gets events for all domains
                if (User.IsInRole("Auditor") || User.IsInRole("Admin")) events = db.Events.Where(e => e.ItemID == id).ToList();
                //Gets event specific to a users domain
                else events = db.FilteredEvents.Where(e => e.ItemID == id).ToList();

                ItemDetailViewModel itemDetailView = new ItemDetailViewModel()
                {
                    item = item,
                    events = events
                };

                return View(itemDetailView);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Details"));
            }
        }

        // GET: Items/Create

        //Purpose: Retrieve information from view 


        /*If this data is valid and the user is an admin or keyholder role they will be able to create an item:
         * An item can then be created and added to the database. 
         * A new event to log the item creation can also then be created and added to the database.
         * After this, the user will be rediected to the Index view. 
         If data is not valid: 
         * Viewbags will be recreated and the view will reload. 
         */

        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public ActionResult Create()
        {
            try
            {
                if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive == true), "ContainerID", "Name");
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive == true), "ContainerID", "Name");
                }

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Index"));
            }
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public ActionResult Create([Bind(Include = "ItemID,Name,Description,Status,Barcode,needWitness,ContainerID")] Item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Create new item and assign GUID
                    item.ItemID = Guid.NewGuid();
                    item.isActive = true;
                    item.Status = true;
                    //add new item to DB
                    db.Items.Add(item);

                    //Calls create event from the Event class and returns the event
                    var container = db.Containers.Find(item.ContainerID);
                    var logEvent = Event.CreateEvent(Event.AddItem, item.ItemID, item.Name, User.Identity.Name, container.DomainID, "null");
                    db.Events.Add(logEvent);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive == true), "ContainerID", "Name", item.ContainerID);
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive == true), "ContainerID", "Name", item.ContainerID);
                }
                return View(item);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Create"));
                //return View();
            }
        }
        //Purpose: Retrieve information from view 

        //If the user is  Admin or Keyholder, they wil have the option to duplicate an item in the index view*


        // GET: Items/Details/Duplicate/id
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public ActionResult Duplicate(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find Item by ID
                Item item = db.Items.Find(id);
                //If no item found return error
                if (item == null)
                {
                    return HttpNotFound();
                }
                //Creates a copy of the item being duplicated
                var temp = item;

                var viewModel = new DupItemViewModel
                {
                    ItemId = temp.ItemID,
                    Name = temp.Name,
                    Description = temp.Description,
                    Barcode = temp.Barcode,
                    Container = temp.Container,
                    Password = ""
                    //KeyHolder = null
                };

                ViewBag.KeyHolder = new SelectList(string.Empty, "Value", "Text");

                
                ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive == true), "ContainerID", "Name");
                

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Index"));
            }
        }

        // POST: Items/Details/Duplicate/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public ActionResult Duplicate([Bind(Include = "ItemId,Name,Description,Barcode,NewBarcode,ContainerId,KeyHolder,Password")] DupItemViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Finds the original item 
                    Item temp = db.Items.Find(vm.ItemId);
                    //Validates unique barcode (another validation is done on the new item creation).
                    var validateBarcode = db.Items.FirstOrDefault(x => x.Barcode == vm.NewBarcode);
                    if (validateBarcode != null)
                    {
                        ModelState.AddModelError("NewBarcode", "Barcode already exists in database. Please try again.");
                    }
                    else
                    {
                        //ApplicationUser witness = UserManager.FindByEmail(vm.KeyHolder);
                        ApplicationUser witness = db.Containers.Find(vm.ContainerId).User;
                        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name))
                        {
                            var result = (UserManager.CheckPassword(witness, vm.Password) || (pc.ValidateCredentials(witness.Email, vm.Password)));
                            if (result)
                            {
                                //Creates a new item based on the duplicated item 
                                Item item = new Item
                                {
                                    ItemID = Guid.NewGuid(),
                                    Name = temp.Name,
                                    Description = temp.Description,
                                    isActive = true,
                                    Status = true,
                                    Barcode = vm.NewBarcode,
                                    ContainerID = vm.ContainerId
                                };

                                db.Items.Add(item);

                                //Calls create event from the Event class and returns the event
                                var container = db.Containers.Find(item.ContainerID);
                                StringBuilder sbDupe = new StringBuilder("Item Duplicated to container \"");
                                sbDupe.Append(container.Name);
                                sbDupe.Append("\" owned by ");
                                sbDupe.Append(container.User.Email);
                                var logEvent = Event.CreateVerboseEvent(Event.DuplicateItem, item.ItemID, item.Name, User.Identity.Name, container.DomainID, "null", sbDupe.ToString());
                                db.Events.Add(logEvent);

                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }

                            else
                            {
                                ModelState.AddModelError("", "Invalid password");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid model state");
                    if (vm.NewBarcode != null)
                    {
                        var validateBarcode = db.Items.FirstOrDefault(x => x.Barcode == vm.NewBarcode);
                        if (validateBarcode != null)
                        {
                            ModelState.AddModelError("NewBarcode", "Barcode already exists in database. Please try again.");
                        }
                    }
                    if (db.Containers.Find(vm.ContainerId) != null && vm.Password != null)
                    {
                        ApplicationUser witness = db.Containers.Find(vm.ContainerId).User;
                        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name))
                        {
                            var result = (UserManager.CheckPassword(witness, vm.Password) || (pc.ValidateCredentials(witness.Email, vm.Password)));
                            if (!result)
                            {
                                ModelState.AddModelError("Password", "Incorrect password");
                            }
                        }
                    }
                    if(db.Containers.Find(vm.ContainerId) == null)
                    {
                        ModelState.AddModelError("ContainerId", "Please select a container");
                    }
                }

                Item curr = db.Items.Find(vm.ItemId);
                var copy = curr;

                var viewModel = new DupItemViewModel
                {
                    ItemId = copy.ItemID,
                    Name = copy.Name,
                    Description = copy.Description,
                    Barcode = copy.Barcode,
                    Container = copy.Container,
                    Password = "",
                    KeyHolder = vm.KeyHolder
                };

                Container list = db.Containers.FirstOrDefault(c => c.ContainerID == vm.ContainerId);
                var users = db.Users.ToList<ApplicationUser>();

                List<SelectListItem> keyHolders = new List<SelectListItem>();

                keyHolders.Add(new SelectListItem
                {
                    Value = vm.KeyHolder,
                    Text = vm.KeyHolder
                });

                ViewBag.KeyHolder = keyHolders;
                if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive == true), "ContainerID", "Name");
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive == true), "ContainerID", "Name");
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Duplicate"));
            }
        }
        /*Purpose: Retrieve information from view 
         
          
         If this data is valid:
         *First ensures that item data can be updated.
         * A new event to log that the item has been edited can also then be 
         created and added to the database.
         *Confirms item is still present and saves event to database.
         * After this, the user will be rediected to the Index view to see these changes made to item. 
         If data is not valid: 
         * Viewbags will be recreated and the view will reload. 
         */
        //JSON Function that auto fills the keyholder drop down in the duplicate view depending on the container selected
        public JsonResult GetKeyHolder(Guid id)
        {
            Container list = db.Containers.FirstOrDefault(c => c.ContainerID == id);
            var users = db.Users.ToList<ApplicationUser>();
            List<SelectListItem> keyHolders = new List<SelectListItem>();


            keyHolders.Add(new SelectListItem
            {
                Value = list.User.Email,
                Text = list.User.Email
            });

            return Json(new SelectList(keyHolders, "Value", "Text"));
        }

        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public async Task<ActionResult> CheckInOut(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find Item by ID
                Item item = db.Items.Find(id);
                //If no item found return error
                if (item == null)
                {
                    return HttpNotFound();
                }

                List<SelectListItem> witnesses = new List<SelectListItem>();
                //Creates a list of users in the Witness role
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Witness))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            witnesses.Add(new SelectListItem
                            {
                                Value = u.Email,
                                Text = u.Email
                            });
                        }
                    }
                }

                ViewBag.List = witnesses;

                CheckInOutViewModel cio = new CheckInOutViewModel()
                {
                    ItemID = item.ItemID,
                    ItemName = item.Name,
                    ContainerName = item.Container.Name,
                    Container = item.Container,
                    LocationName = item.Container.Location.Name,
                    Status = item.Status,
                    Barcode = item.Barcode,
                    Justification = ""
                };

                return View(cio);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "CheckInOut"));
            }
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public async Task<ActionResult> CheckInOut([Bind(Include = "ItemID,ItemName,ContainerName,LocationName,Barcode,NewBarcode,Status,Witness,Witnesses,Password,Justification")] CheckInOutViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == vm.Witness)
                {
                    ModelState.AddModelError("Witness", "You may not act as a witness for your own action.");
                }
                else
                {
                    Item checkItem = db.Items.Find(vm.ItemID);
                    ApplicationUser witness = UserManager.FindByEmail(vm.Witness);
                    if (witness == null)
                    {
                        ModelState.AddModelError("Witness", "Please enter a valid username.");
                    }
                    else
                    {
                        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name))
                        {
                            var result = (UserManager.CheckPassword(witness, vm.Password) || pc.ValidateCredentials(vm.Witness, vm.Password));
                            if (result)
                            {
                                //Item is checked in
                                if (checkItem.Status == false)
                                {
                                    checkItem.Status = true;
                                    //Validates unique barcode (another validation is done on the new item creation).
                                    var validateBarcode = db.Items.FirstOrDefault(x => x.Barcode == vm.NewBarcode && x.ItemID != vm.ItemID);
                                    if (validateBarcode != null)
                                    {
                                        ModelState.AddModelError("NewBarcode", "Barcode already exists in database. Please try again.");
                                    }
                                    else
                                    {
                                        checkItem.Barcode = vm.NewBarcode;
                                        //Calls create event from the Event class and returns the event
                                        var container = db.Containers.Find(checkItem.ContainerID);
                                        var logEvent = Event.CreateEvent(Event.CheckedIn, checkItem.ItemID, checkItem.Name, User.Identity.Name, container.DomainID, vm.Justification);
                                        db.Events.Add(logEvent);
                                        db.SaveChanges();
                                        return RedirectToAction("Index");
                                    }
                                }

                                //Item is checked out
                                else
                                {
                                    if (vm.NewBarcode != checkItem.Barcode)
                                    {
                                        ModelState.AddModelError("NewBarcode", "Barcode does not match. Please ensure you are scanning the correct item.");
                                    }
                                    else
                                    {
                                        checkItem.Status = false;
                                        //Calls create event from the Event class and returns the event
                                        var container = db.Containers.Find(checkItem.ContainerID);
                                        var logEvent = Event.CreateEvent(Event.CheckedOut, checkItem.ItemID, checkItem.Name, User.Identity.Name, container.DomainID, vm.Justification);
                                        db.Events.Add(logEvent);
                                        db.SaveChanges();
                                        return RedirectToAction("Index");
                                    }
                                }

                                //db.SaveChanges();
                                //return RedirectToAction("Index");

                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid witness password");
                            }
                        }
                    }
                }
            }

            List<SelectListItem> witnesses = new List<SelectListItem>();

            //Creates a list of users in the Witness role
            foreach (var u in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(u.Id, RoleName.Witness))
                {
                    if (u.Email != User.Identity.Name)
                    {
                        witnesses.Add(new SelectListItem
                        {
                            Value = u.Email,
                            Text = u.Email
                        });
                    }
                }
            }

            Item item = db.Items.Find(vm.ItemID);

            CheckInOutViewModel cio = new CheckInOutViewModel()
            {
                ItemID = item.ItemID,
                ItemName = item.Name,
                ContainerName = item.Container.Name,
                Container = item.Container,
                LocationName = item.Container.Location.Name,
                Status = item.Status,
                Barcode = item.Barcode,
            };

            ViewBag.List = witnesses;
            return View(cio);
        }

        // GET: Items/Edit/5
        /*Purpose: Retrieve information from view       
          
         If this data is valid:
         *First ensures that item data can be updated by admin or keyholder.
         * A new event to log that the item has been edited can also then be 
         created and added to the database.
         *Confirms item is still present and saves event to database.
         * After this, the user will be rediected to the Index view to see these changes made to container. 
         If data is not valid: 
         * Viewbags will be recreated and the view will reload. 
         */
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public async Task<ActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Item item = db.Items.Find(id);
                if (item == null)
                {
                    return HttpNotFound();
                }

                List<SelectListItem> witnesses = new List<SelectListItem>();
                //Creates a list of users in the Witness role
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Witness))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            witnesses.Add(new SelectListItem
                            {
                                Value = u.Email,
                                Text = u.Email
                            });
                        }
                    }
                }

                ViewBag.List = witnesses;

                ItemEditViewModel IE = new ItemEditViewModel()
                {
                    ItemID = item.ItemID,
                    Name = item.Name,
                    Description = item.Description,
                    Barcode = item.Barcode,
                    ContainerID = item.ContainerID,
                    Container = db.Containers.Find(item.ContainerID),
                    Status = item.Status
                };

                if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive == true), "ContainerID", "Name");
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive == true), "ContainerID", "Name");
                }

                return View(IE);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Index"));
            }
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public async Task<ActionResult> Edit([Bind(Include = "ItemID,Name,Description,Status,Barcode,ContainerID,Witness,Witnesses,Password")] ItemEditViewModel vm, string justification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (User.Identity.Name == vm.Witness)
                    {
                        ModelState.AddModelError("Witness", "You may not act as a witness for your own action.");
                    }
                    else
                    {
                        Item checkItem = db.Items.Find(vm.ItemID); // reading in the Item being edited
                        db.Entry(checkItem).State = EntityState.Modified; // Modifying the items properties
                        ApplicationUser witness = UserManager.FindByEmail(vm.Witness); // Reading in the witness for verification
                        if (witness == null)
                        {
                            ModelState.AddModelError("Witness", "Please enter a valid username.");
                        }
                        else
                        {
                            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name))
                            {
                                var result = (UserManager.CheckPassword(witness, vm.Password) || (pc.ValidateCredentials(vm.Witness, vm.Password))); // Checking the witnesses credentials for verification
                                if (result) // if witness verification is successful..
                                {
                                    // setting descriptive logging
                                    StringBuilder sb = new StringBuilder("Item Edited:");
                                    if (!(checkItem.Name.Equals(vm.Name)))
                                    {
                                        sb.Append(" Name \"");
                                        sb.Append(checkItem.Name);
                                        sb.Append("\" changed to \"");
                                        sb.Append(vm.Name);
                                        sb.Append("\";");
                                    }
                                    if (!(checkItem.Description.Equals(vm.Description)))
                                    {
                                        sb.Append(" Description \"");
                                        sb.Append(checkItem.Description);
                                        sb.Append("\" changed to \"");
                                        sb.Append(vm.Description);
                                        sb.Append("\";");
                                    }
                                    if (checkItem.ContainerID != vm.ContainerID)
                                    {
                                        sb.Append(" Container \"");
                                        sb.Append(db.Containers.Find(checkItem.ContainerID).Name);
                                        sb.Append("\" changed to \"");
                                        sb.Append(db.Containers.Find(vm.ContainerID).Name);
                                        sb.Append("\";");
                                    }
                                    if (!(checkItem.Barcode.Equals(vm.Barcode)))
                                    {
                                        sb.Append(" Barcode \"");
                                        sb.Append(checkItem.Barcode);
                                        sb.Append("\" changed to \"");
                                        sb.Append(vm.Barcode);
                                        sb.Append("\";");
                                    }

                                    // updating the item name, description, containerID, and barcode (if changed)
                                    checkItem.Name = vm.Name;
                                    checkItem.Description = vm.Description;
                                    checkItem.ContainerID = vm.ContainerID;
                                    checkItem.Barcode = vm.Barcode;

                                    //Calls create event from the Event class and returns the event
                                    var container = db.Containers.Find(checkItem.ContainerID);
                                    var logEvent = Event.CreateVerboseEvent(Event.EditItem, checkItem.ItemID, checkItem.Name, User.Identity.Name, container.DomainID, justification, sb.ToString());
                                    db.Events.Add(logEvent);

                                    checkItem.isActive = true;
                                    db.SaveChanges();
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid witness password.");
                                }
                            }
                        }
                    }
                }

                List<SelectListItem> witnesses = new List<SelectListItem>();

                //Creates a list of users in the Witness role
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Witness))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            witnesses.Add(new SelectListItem
                            {
                                Value = u.Email,
                                Text = u.Email
                            });
                        }
                    }
                }

                Item item = db.Items.Find(vm.ItemID);

                ItemEditViewModel IE = new ItemEditViewModel()
                {
                    ItemID = item.ItemID,
                    Name = item.Name,
                    Description = item.Description,
                    Barcode = item.Barcode,
                    ContainerID = item.ContainerID,
                    Container = db.Containers.Find(item.ContainerID),
                    Status = item.Status
                };

                ViewBag.List = witnesses;

                if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive == true), "ContainerID", "Name", item.ContainerID);
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive == true), "ContainerID", "Name", item.ContainerID);
                }

                return View(IE);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Edit"));
            }
        }
        /* Purpose: Send information to archive view 
         * Authorizes admin and keyholder user to perform this action
         * Need to check if item exists in order to confirm that it can be archived 
         * If ID is not null: 
         * Searches for item in that ID
         * If item is not null: 
         * Will then return view and user will be brought to Archive view.
         */
        // GET: Items/Archive/5
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public async Task<ActionResult> Archive(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find Item by ID
                Item item = db.Items.Find(id);
                //If no item found return error
                if (item == null)
                {
                    return HttpNotFound();
                }

                List<SelectListItem> witnesses = new List<SelectListItem>();
                //Creates a list of users in the Witness role
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Witness))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            witnesses.Add(new SelectListItem
                            {
                                Value = u.Email,
                                Text = u.Email
                            });
                        }
                    }
                }

                ViewBag.List = witnesses;

                ItemArchiveViewModel ia = new ItemArchiveViewModel()
                {
                    ItemID = item.ItemID,
                    ContainerID = item.Container.ContainerID,
                    Container = item.Container,
                    ContainerName = item.Container.Name,
                    Name = item.Name,
                    Description = item.Description,
                    Status = item.Status,
                    isActive = item.isActive
                };

                return View(ia);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Index"));
            }
        }


        // POST: Items/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]

        public async Task<ActionResult> ArchiveConfirmed([Bind(Include = "ItemID,Name,Description,Container,ContainerID,ContainerName,isActive,Status,Witness,Witnesses,Password,Justification")] ItemArchiveViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (User.Identity.Name == vm.Witness)
                    {
                        ModelState.AddModelError("Witness", "You may not act as a witness for your own action.");
                    }
                    else
                    {
                        Item checkItem = db.Items.Find(vm.ItemID);
                        ApplicationUser witness = UserManager.FindByEmail(vm.Witness);
                        if (witness == null)
                        {
                            ModelState.AddModelError("Witness", "Please enter a valid username.");
                        }
                        else
                        {
                            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name))
                            {
                                var result = (UserManager.CheckPassword(witness, vm.Password) || (pc.ValidateCredentials(vm.Witness, vm.Password))); // Checking the witnesses credentials for verification

                                if (result)
                                {

                                    //Calls create event from the Event class and returns the event
                                    var container = db.Containers.Find(checkItem.ContainerID);
                                    //Adds event to DB
                                    var logEvent = Event.CreateEvent(Event.ArchiveItem, checkItem.ItemID, checkItem.Name, User.Identity.Name, container.DomainID, vm.Justification);

                                    db.Events.Add(logEvent);

                                    checkItem.isActive = false;
                                    db.SaveChanges();
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid witness password.");
                                }
                            }
                        }
                    }
                }

                List<SelectListItem> witnesses = new List<SelectListItem>();

                //Creates a list of users in the Witness role
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Witness))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            witnesses.Add(new SelectListItem
                            {
                                Value = u.Email,
                                Text = u.Email
                            });
                        }
                    }
                }

                Item item = db.Items.Find(vm.ItemID);

                ItemArchiveViewModel ia = new ItemArchiveViewModel()
                {
                    ItemID = item.ItemID,
                    ContainerID = item.Container.ContainerID,
                    Container = item.Container,
                    ContainerName = item.Container.Name,
                    Name = item.Name,
                    Description = item.Description,
                    Status = item.Status,
                    isActive = item.isActive,

                };

                ViewBag.List = witnesses;

                return View(ia);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Items", "Archive"));
            }
        }

        // CSV METHOD:

        public void exportCSV()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Item Name\",\"Container Name\",\"Description\",\"Domain\",\"Barcode\""); // Declaring header for each item in the item log

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Details.csv"); // Giving the file a name
            Response.ContentType = "text/csv"; // Declaring the file type of CSV

            var item = new List<Item>(); // creating the list of items
            item = db.Items.ToList(); // retrieving the list of items from the database

            foreach (var product in item) // displaying information for each item in the item log
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                    product.Name,
                    product.Container.Name,
                    product.Description,
                    product.Container.Domain.DomainName,
                    product.Barcode));
            }
            Response.Write(sw.ToString());
            Response.End();
        }

        /*
        public ActionResult IsBarcodeAvailble(string Barcode)
        {
            try
            {
                var barcode = db.Items.Single(m => m.Barcode == Barcode);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        */


        /* releases unmanaged resources*/
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}



