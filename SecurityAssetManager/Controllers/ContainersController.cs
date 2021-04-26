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

namespace SecurityAssetManager.Controllers
{
    [Authorize]
    public class ContainersController : Controller
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

        // GET: Containers
        //Purpose: Return a list of containers to the Container Index view 
        public ActionResult Index()
        {
            try
            {

                //Creates list of container
                var containers = new List<Container>();
                //For user in admin or auditor role 
                if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
                {
                    //Retrieves full list of containers within a location
                    containers = db.Containers.Include(c => c.Location).Where(i => i.isActive == true).ToList();
                }
                //For users not in admin or auditor role
                else
                {
                    //Retrieves containers dependent on user's domain
                    containers = db.FilteredContainers.Include(c => c.Location).ToList();
                }

                //Returns containers to view 
                return View(containers);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Index"));
            }
        }



        // GET: Containers/Details/5
        //Purpose: Displays container details and items associated with it in Container Detail view 
        public ActionResult Details(Guid? id)
        {

            try
            {
                //Checks if ID exists 
                if (id == null)
                {

                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //Finds container by ID 
                Container container = db.Containers.Find(id);

                //Returns error if container not found
                if (container == null)
                {
                    return HttpNotFound();
                }

                //Retrieves list of items within container 
                var items = db.Items.Include(i => i.Container).ToList().Where(i => i.isActive == true && i.ContainerID == id);

                //Creates instance of a view model 
                ContainerDetailViewModel containerDetailView = new ContainerDetailViewModel()
                {
                    //Passes object data
                    container = container,
                    items = items
                };

                //returns View Model to view
                return View(containerDetailView);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Details"));
            }
        }

        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public ActionResult BulkDuplicate(Guid? id)
        {

            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Container container = db.Containers.Find(id);
                if (container == null)
                {
                    return HttpNotFound();
                }

                if (User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive).ToList(), "ContainerID", "Name");
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive).ToList(), "ContainerID", "Name");
                }

                ViewBag.ItemList = new SelectList(db.Items.Include(i => i.Container).Where(i => i.isActive && i.ContainerID == id).ToList(), "ItemID", "Name");
                ViewBag.KeyHolder = new SelectList(string.Empty, "Value", "Text");

                BulkDuplicateViewModel vm = new BulkDuplicateViewModel
                {
                    CurrentContainerName = container.Name,
                    CurrentContainerID = container.ContainerID,
                    Password = "",
                    KeyHolder = null
                };

                return View(vm);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Details"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Keyholder)]
        public ActionResult BulkDuplicate([Bind(Include = "CurrentContainerName,CurrentContainerID,NewContainerID,KeyHolder,Password")] BulkDuplicateViewModel vm, params Guid[] selectedItems)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser witness = UserManager.FindByEmail(vm.KeyHolder);
                    var result = UserManager.CheckPassword(witness, vm.Password);
                    Container container = db.Containers.Find(vm.NewContainerID);

                    if (result)
                    {
                        foreach (var selected in selectedItems)
                        {
                            Item temp = db.Items.Find(selected);

                            Item item = new Item
                            {
                                ItemID = Guid.NewGuid(),
                                Name = temp.Name,
                                Description = temp.Description,
                                isActive = true,
                                Status = true,
                                ContainerID = container.ContainerID
                            };

                            db.Items.Add(item);

                            //Calls create event from the Event class and returns the event
                            var logEvent = Event.CreateEvent(Event.DuplicateItem, item.ItemID, item.Name, User.Identity.Name, container.DomainID, "null");
                            db.Events.Add(logEvent);
                            db.SaveChanges();
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid password");
                    }
                }

                if (User.IsInRole("Admin"))
                {
                    ViewBag.ContainerID = new SelectList(db.Containers.Where(i => i.isActive).ToList(), "ContainerID", "Name");
                }
                else
                {
                    ViewBag.ContainerID = new SelectList(db.FilteredContainers.Where(i => i.isActive).ToList(), "ContainerID", "Name");
                }

                ViewBag.ItemList = new SelectList(db.Items.Include(i => i.Container).Where(i => i.isActive && i.ContainerID == vm.CurrentContainerID).ToList(), "ItemID", "Name");

                BulkDuplicateViewModel viewModel = new BulkDuplicateViewModel
                {
                    CurrentContainerName = vm.CurrentContainerName,
                    CurrentContainerID = vm.CurrentContainerID,
                    Password = "",
                    KeyHolder = vm.KeyHolder
                };


                Container list = db.Containers.FirstOrDefault(c => c.ContainerID == vm.CurrentContainerID);
                var users = db.Users.ToList<ApplicationUser>();
                List<SelectListItem> keyHolders = new List<SelectListItem>();

                keyHolders.Add(new SelectListItem
                {
                    Value = vm.KeyHolder,
                    Text = vm.KeyHolder
                });

                ViewBag.KeyHolder = keyHolders;

                return View(viewModel);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "BulkDuplicate"));
            }
        }

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
        /*Note: CRUD actions are only accessible to users in an Admin role.*/


        // GET: Containers/Create
        //Purpose: Sends information to the Create view

        [Authorize(Roles = RoleName.Admin)]
        public async Task<ActionResult> Create()
        {

            try
            {
                //Retrieves list of users and creates new list of emails 
                var users = db.Users.ToList<ApplicationUser>();
                var emails = new List<string>();

                //For each user retrieve email and add to email list 
                foreach (var i in users)
                {
                    emails.Add(i.Email);
                }

                //Creates dropdown list of keyholders 
                List<SelectListItem> keyHolders = new List<SelectListItem>();

                //Find possible keyholders and add to new list for dropdown 
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Keyholder))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            keyHolders.Add(new SelectListItem
                            {
                                Value = u.Id,
                                Text = u.Email
                            });
                        }
                    }
                }

                //Creates viewbag for keyholders, location, and domain; used to populate dropdown lists in view
                ViewBag.KeyHolderID = keyHolders; //new SelectList(users, "Id", "Email");
                ViewBag.LocationID = new SelectList(db.Locations.Where(i => i.isActive == true), "LocationID", "Name");
                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName");

                return View();
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Index"));
            }
        }

        // POST: Containers/Create
        //Purpose: Retrieves information from Create view

        /*HttpPost is called after information is filled out and submitted in view 
        and checks to make sure everything selected/typed in view was done properly.*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]

        // Data is bound to the model to specify exact properties needed
        public async Task<ActionResult> Create([Bind(Include = "ContainerID,Name,LocationID,UserID,DomainID")] Container container)
        {

            try
            {

                //Checks to see if data was bound to model correctly
                if (ModelState.IsValid)
                {
                    //Creates new container and assigns new Guid

                    container.ContainerID = Guid.NewGuid();

                    //Sets container to active
                    container.isActive = true;

                    //Adds container to database
                    db.Containers.Add(container);

                    //Calls create event from the Event class and returns the event
                    var logEvent = Event.CreateEvent(Event.AddContainer, container.ContainerID, container.Name, User.Identity.Name, container.DomainID, "null");

                    //Adds and saves event to database
                    db.Events.Add(logEvent);
                    db.SaveChanges();

                    //Redirects user to Index view 
                    return RedirectToAction("Index");
                }

                //Creates dropdown list of keyholders 
                List<SelectListItem> keyHolders = new List<SelectListItem>();

                //Find possible keyholders and adds to new list for dropdown
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Keyholder))
                    {
                        if (u.Email != User.Identity.Name)
                        {
                            keyHolders.Add(new SelectListItem
                            {
                                Value = u.Id,
                                Text = u.Email
                            });
                        }
                    }
                }

                //Creates viewbag for keyholders, location, and domain; used to populate dropdown lists in view
                ViewBag.KeyHolderID = keyHolders; // new SelectList(users, "Id", "Email", container.UserID);
                ViewBag.LocationID = new SelectList(db.Locations.Where(i => i.isActive == true), "LocationID", "Name", container.LocationID);
                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName", container.DomainID);

                //returns container to view
                return View(container);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Create"));
            }

        }

        // GET: Containers/Edit/5
        //Purpose: Sends information to Edit view

        [Authorize(Roles = RoleName.Admin)]
        public async Task<ActionResult> Edit(Guid? id)
        {

            try
            {

                //Checks if ID exists 
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //Find container by ID
                Container container = db.Containers.Find(id);

                //Returns error if container is not found 
                if (container == null)
                {
                    return HttpNotFound();
                }

                //Creates new keyholder list and populates dropdown 
                List<SelectListItem> keyHolders = new List<SelectListItem>();
                keyHolders.Add(new SelectListItem
                {
                    Value = container.UserID,
                    Text = container.User.Email
                });

                //Finds possible keyholders and adds to new list for dropdown
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Keyholder))
                    {
                        if (u.Email != container.User.Email)
                        {
                            keyHolders.Add(new SelectListItem
                            {
                                Value = u.Id,
                                Text = u.Email
                            });
                        }
                    }
                }

                //Creates viewbag for keyholders, location, and domain; used to populate dropdown lists in view
                ViewBag.KeyHolderID = keyHolders;
                ViewBag.LocationID = new SelectList(db.Locations.Where(i => i.isActive == true), "LocationID", "Name", container.LocationID);
                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName", container.DomainID);

                //return container to view 
                return View(container);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Index"));
            }
        }

        // POST: Containers/Edit/5
        //Purpose: Retrieves information from Edit view

        /*HttpPost is called after information is filled out and submitted in view 
        and checks to make sure everything selected/typed in view was done properly.*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]

        // Data is bound to the model to specify exact properties needed
        public async Task<ActionResult> Edit([Bind(Include = "ContainerID,Name,LocationID,DomainID,UserID")] Container container, string justification)
        {

            try
            {
                //Checks to see if data was bound to model correctly
                if (ModelState.IsValid)
                {
                    //Assigns user to container 
                    container.User = db.Users.FirstOrDefault(c => c.Id == container.UserID);
                    db.Entry(container).State = EntityState.Modified;


                    //Calls create event from the Event class and returns the event
                    var logEvent = Event.CreateEvent(Event.EditContainer, container.ContainerID, container.Name, User.Identity.Name, container.DomainID, justification);
                    //Adds event to database 
                    db.Events.Add(logEvent);

                    //Sets container to active 
                    container.isActive = true;

                    //Saves event to database 
                    db.SaveChanges();
                    //Redirects user to Index view 
                    return RedirectToAction("Index");
                }

                //Creates new keyholder list and populates dropdown 
                List<SelectListItem> keyHolders = new List<SelectListItem>();
                keyHolders.Add(new SelectListItem
                {
                    Value = container.UserID,
                    Text = container.User.Email
                });

                //Find possible keyholders and adds to new list for dropdown
                foreach (var u in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(u.Id, RoleName.Keyholder))
                    {
                        if (u.Email != container.User.Email)
                        {
                            keyHolders.Add(new SelectListItem
                            {
                                Value = u.Id,
                                Text = u.Email
                            });
                        }
                    }
                }

                //Creates viewbag for keyholders, location, and domain; used to populate dropdown lists in view
                ViewBag.KeyHolderID = keyHolders;
                ViewBag.LocationID = new SelectList(db.Locations.Where(i => i.isActive == true), "LocationID", "Name", container.LocationID);
                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName", container.DomainID);

                //returns container to view 
                return View(container);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Edit"));
            }
        }

        // GET: Containers/Archive/5
        //Purpose: Sends information to Archive view

        [Authorize(Roles = RoleName.Admin)]
        public ActionResult Archive(Guid? id)
        {

            try
            {

                //Checks if ID exists
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                //Finds container by ID
                Container container = db.Containers.Find(id);

                //Returns error if container not found 
                if (container == null)
                {
                    return HttpNotFound();
                }

                //Returns container to view
                return View(container);
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Index"));
            }
        }

        // POST: Containers/Archive/5
        //Purpose: Retrieves information from Archive view

        /*HttpPost is called after information is filled out and submitted in view 
        and checks to make sure everything selected/typed in view was done properly.*/
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult ArchiveConfirmed(Guid id, string justification)
        {

            try
            {
                //Finds container by ID 
                Container container = db.Containers.Find(id);

                //Calls create event from the Event class and returns the event
                var logEvent = Event.CreateEvent(Event.ArchiveContainer, container.ContainerID, container.Name, User.Identity.Name, container.DomainID, justification);
                //Adds event to database 
                db.Events.Add(logEvent);
                //Sets container to inactive 
                container.isActive = false;
                //Saves event to database 
                db.SaveChanges();
                //Redirects user to Index view 
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Containers", "Archive"));
            }
        }

        // CSV METHOD:

        public void exportCSV()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Container Name\",\"Key Holder\",\"Location Name\",\"Domain\""); // Declaring header for each action in the container log

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Details.csv"); // Giving the file a name
            Response.ContentType = "text/csv"; // Declaring the file type of CSV

            var containers = new List<Container>(); // creating the list of containers
            containers = db.Containers.ToList(); // retrieving the list of containers from the database

            foreach (var container1 in containers) // displaying information for each container in the container log
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                    container1.Name,
                    container1.User.UserName,
                    container1.Location.Name,
                    container1.Domain.DomainName));
            }

            Response.Write(sw.ToString());
            Response.End();
        }

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
