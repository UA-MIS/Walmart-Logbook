using Microsoft.AspNet.Identity.Owin;
using SecurityAssetManager.Models;
using SecurityAssetManager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SecurityAssetManager.Controllers
{
    [Authorize]
    public class UsersAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Auditor)]
        public async Task<ActionResult> Index()
        {
            try
            {
                //Creates list of users
                var users = await UserManager.Users.ToListAsync();

                //Creates list of view model
                List<UserRoleViewModel> urvms = new List<UserRoleViewModel>();

                //Repeats following process below for each user 
                foreach (var user in users)
                {
                    //Creates list of roles
                    var roles = await UserManager.GetRolesAsync(user.Id);

                    //Creates instance of view model
                    UserRoleViewModel urvm = new UserRoleViewModel()
                    {
                        User = user,
                        Roles = roles
                    };
                    //Adds view model to view model list
                    urvms.Add(urvm);
                }
                //Returns view model to view
                return View(urvms);
                /*
                 * CODE FOR SORTING USER INDEX BY DOMAIN
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                var userDomains = db.UserDomains.Include(i => i.Domain).Where(i => i.Selected == true && i.UserID.ToString() == user.Id).ToList();
                List<UserDomain> domains = new List<UserDomain>();

                foreach (var domain in userDomains)
                {
                    foreach (var d in db.UserDomains.ToList())
                    {
                        if (domain.DomainID == d.DomainID)
                        {
                            domains.Add(d);
                        }
                    }
                }

                IQueryable<ApplicationUser> filteredUsers = Enumerable.Empty<ApplicationUser>().AsQueryable();
                List<ApplicationUser> userList = filteredUsers.ToList();

                var users = await UserManager.Users.ToListAsync();

                foreach (var domain in domains)
                {
                    foreach (var u in users)
                    {
                        if (u.Id == domain.UserID.ToString())
                        {
                            userList.Add(u);
                        }
                    }
                }

                //Converts list to IQueryable so it can be returned
                filteredUsers = userList.AsQueryable();
                return View(filteredUsers);
                */
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Index"));
            }
        }
        //
        // GET: /Users/Details/5
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Auditor)]
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                //Checks if ID exists 
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find user by ID
                var user = await UserManager.FindByIdAsync(id);

                //Get all roles for user
                ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
                Guid copyID = Guid.Parse(id);

                //Get all domains assigned to the user
                var userDomains = db.UserDomains.Where(i => i.UserID == copyID).ToList();

                //Creates dropdown list of domains 
                List<SelectListItem> domains = new List<SelectListItem>();

                //Adding all domains assigned to user to select list
                foreach (var item in userDomains)
                {
                    var d = db.Domains.Find(item.DomainID);
                    domains.Add(new SelectListItem
                    {
                        Value = "1",
                        Text = d.DomainName
                    });
                }

                //Return viewbag for all assigned domains
                ViewBag.UserDomains = new SelectList(domains, "Value", "Text");

                return View(user);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Details"));
            }
        }

        //
        // GET: /Users/Create
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            try
            {
                //Get the list of Roles
                ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

                //Get the list of Domains
                ViewBag.Domains = new SelectList(db.Domains, "DomainID", "DomainName", "Description");

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Index"));
            }
        }
        //
        // POST: /Users/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Creates new list of user
                    var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                    //Creates list of admin result to ensure user and password match
                    var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                    //Add User to the selected Roles 
                    if (adminresult.Succeeded)
                    {
                        //Checks if selected roles is empty 
                        if (selectedRoles != null)
                        {
                            //Creates list of result to add user roles 
                            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);

                            //Calls create event from the Event class and returns the event
                            var logEvent = Event.CreateEvent(Event.AddUser, Guid.Parse(user.Id), user.UserName, User.Identity.Name, Guid.Empty, "null");
                            //Adds and saves event to database
                            db.Events.Add(logEvent);
                            db.SaveChanges();

                            //Checks if result succeeded 
                            if (!result.Succeeded)
                            {
                                //Adds result error to model if unsuccessful
                                ModelState.AddModelError("", result.Errors.First());
                                //Creates viewbag for role; used to populate dropdown list in view
                                ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                                return View();
                            }
                        }
                    }
                    else
                    {
                        //Adds admin result error to model if unsuccessful 
                        ModelState.AddModelError("", adminresult.Errors.First());
                        //Creates viewbag for role; used to populate dropdown list in view
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                        return View();

                    }

                    //Redirects user to Index view 
                    return RedirectToAction("Index");
                }
                //Creates viewbag for role; used to populate dropdown list in view
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Create"));
            }
        }

        //
        // GET: /Users/AdCreate
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdCreate()
        {
            try
            {
                //Get the list of Roles
                ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

                //Get the list of Domains
                ViewBag.Domains = new SelectList(db.Domains, "DomainID", "DomainName", "Description");

                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Index"));
            }
        }
        //
        // POST: /Users/AdCreate
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdCreate(AdRegisterViewModel userViewModel, params string[] selectedRoles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name)) {
                        using (var foundUser = UserPrincipal.FindByIdentity(pc, userViewModel.Username))
                        {
                            if(foundUser == null)
                            {
                                //Adds admin result error to model if unsuccessful 
                                ModelState.AddModelError("Username", "User does not exist in AD");
                                //Creates viewbag for role; used to populate dropdown list in view
                                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                                return View("Create");
                            }
                        }
                    }

                    string specials = "!@#$%&_-";
                    string numbers = "1234567890";
                    string caps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string lowers = "abcdefghijklmnopqrstuvwxyz";
                    List<char> specialsList = new List<char>();
                    List<char> numbersList = new List<char>();
                    List<char> capsList = new List<char>();
                    List<char> lowersList = new List<char>();
                    List<char> charsList = new List<char>();
                    specialsList.AddRange(specials);
                    numbersList.AddRange(numbers);
                    capsList.AddRange(caps);
                    lowersList.AddRange(lowers);
                    charsList.AddRange(specials);
                    charsList.AddRange(numbers);
                    charsList.AddRange(caps);
                    charsList.AddRange(lowers);

                    StringBuilder sbPass = new StringBuilder();
                    Random rnd = new Random();
                    sbPass.Append(specialsList.ElementAt(rnd.Next(specialsList.Count)));
                    sbPass.Append(numbersList.ElementAt(rnd.Next(numbersList.Count)));
                    sbPass.Append(capsList.ElementAt(rnd.Next(capsList.Count)));
                    sbPass.Append(lowersList.ElementAt(rnd.Next(lowersList.Count)));
                    for(int i=0; i<16; i++)
                    {
                        sbPass.Append(charsList.ElementAt(rnd.Next(charsList.Count)));
                    }

                    //Creates new list of user
                    var user = new ApplicationUser { UserName = userViewModel.Username, Email = userViewModel.Username };
                    //Creates list of admin result to ensure user and password match
                    var adminresult = await UserManager.CreateAsync(user, sbPass.ToString());

                    //Add User to the selected Roles 
                    if (adminresult.Succeeded)
                    {
                        //Checks if selected roles is empty 
                        if (selectedRoles != null)
                        {
                            //Creates list of result to add user roles 
                            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);

                            //Calls create event from the Event class and returns the event
                            var logEvent = Event.CreateEvent(Event.AddUser, Guid.Parse(user.Id), user.UserName, User.Identity.Name, Guid.Empty, "null");
                            //Adds and saves event to database
                            db.Events.Add(logEvent);
                            db.SaveChanges();

                            //Checks if result succeeded 
                            if (!result.Succeeded)
                            {
                                //Adds result error to model if unsuccessful
                                ModelState.AddModelError("", result.Errors.First());
                                //Creates viewbag for role; used to populate dropdown list in view
                                ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                                return View("Create");
                            }
                        }
                    }
                    else
                    {
                        //Adds admin result error to model if unsuccessful 
                        ModelState.AddModelError("", adminresult.Errors.First());
                        //Creates viewbag for role; used to populate dropdown list in view
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                        return View("Create");

                    }

                    //Redirects user to Index view 
                    return RedirectToAction("Index");
                }
                //Creates viewbag for role; used to populate dropdown list in view
                ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Create"));
            }
        }

        //
        // GET: /Users/Edit/1
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                //Checks if ID exists
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Finds user by ID
                var user = await UserManager.FindByIdAsync(id);

                //Returns error if user not found 
                if (user == null)
                {
                    return HttpNotFound();
                }

                //Gets user role by user id
                var userRoles = await UserManager.GetRolesAsync(user.Id);

                //Returns view model and its contents to view 
                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Index"));
            }
        }
        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, string justification, params string[] selectedRole)
        {
            try
            {
                //Checks if data bound correctly 
                if (ModelState.IsValid)
                {
                    //Finds user by id
                    var user = await UserManager.FindByIdAsync(editUser.Id);
                    //Checks if user exists
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    //Allows user name and email to be edited
                    user.UserName = editUser.Email;
                    user.Email = editUser.Email;

                    //Gets user role by user id 
                    var userRoles = await UserManager.GetRolesAsync(user.Id);

                    //Creates new selected role string if selected role isn't null 
                    selectedRole = selectedRole ?? new string[] { };

                    //Creates result list to add to roles
                    var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                    //Checks if result succeeded 
                    if (!result.Succeeded)
                    {
                        //Adds result error to model 
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }
                    //Removes result from roles 
                    result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                    //Checks if result succeeeded 
                    if (!result.Succeeded)
                    {
                        //Adds result error to model if unsuccessful 
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }

                    //Calls create event from the Event class and returns the event
                    var logEvent = Event.CreateEvent(Event.EditUser, Guid.Parse(user.Id), user.UserName, User.Identity.Name, Guid.Empty, justification);
                    //Adds and saves event to database 
                    db.Events.Add(logEvent);
                    db.SaveChanges();

                    //Redirects user to Index view 
                    return RedirectToAction("Index");
                }
                //Creates error message 
                ModelState.AddModelError("", "Something failed.");
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Edit"));
            }
        }
        //
        // GET: /Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                //Checks if ID exists
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Finds user by ID
                var user = await UserManager.FindByIdAsync(id);
                //Returns error if user not found 
                if (user == null)
                {
                    return HttpNotFound();
                }
                //Returns user to view 
                return View(user);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Index"));
            }
        }
        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(string id, string justification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Checks if ID exists
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    //Finds user by ID 
                    var user = await UserManager.FindByIdAsync(id);
                    //Returns error if user not found
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    //Creates result list of users to delete  
                    var result = await UserManager.DeleteAsync(user);
                    //Checks if result successful 
                    if (!result.Succeeded)
                    {
                        //Adds result error to model if not successful 
                        ModelState.AddModelError("", result.Errors.First());
                        return View();
                    }

                    //Calls create event from the Event class and returns the event
                    var logEvent = Event.CreateEvent(Event.ArchiveUser, Guid.Parse(user.Id), user.UserName, User.Identity.Name, Guid.Empty, justification);
                    //Adds and saves event to database
                    db.Events.Add(logEvent);
                    db.SaveChanges();

                    //Redirects user to Index view 
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserAdmin", "Delete"));
            }
        }
    }
}
