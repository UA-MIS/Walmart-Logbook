using SecurityAssetManager.Models;
using SecurityAssetManager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace SecurityAssetManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserDomainsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserDomains
        public ActionResult Index()
        {
            try
            {
                var userDomains = db.UserDomains.Include(u => u.Domain);
                return View(userDomains.ToList());
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Index"));
            }
        }
        // GET: UserDomains/Details/5
        public ActionResult Details(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find user domain by ID
                UserDomain userDomain = db.UserDomains.Find(id);
                //If domain not found return error
                if (userDomain == null)
                {
                    return HttpNotFound();
                }
                return View(userDomain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Details"));
            }
        }
        // GET: UserDomains/Create
        public ActionResult Create(string id)
        {
            try
            {
                //Find user by ID
                ApplicationUser user = db.Users.Find(id);
                //Create new user domain and assign user ID
                UserDomain userDomain = new UserDomain
                {
                    UserID = Guid.Parse(user.Id)
                };
                //Temp user ID
                Guid copyID = Guid.Parse(id);
                //Return list of user domains with user ID
                var userDomains = db.UserDomains.Where(i => i.UserID == copyID).ToList();
                //Return list of domains
                var domainList = db.Domains.ToList();
                //Remove all domains that users are in from the list
                foreach (var item in userDomains)
                {
                    domainList.RemoveAll(i => i.DomainID == item.DomainID);
                }

                //View bag of all available domains that users are not in 
                ViewBag.DomainID = new SelectList(domainList, "DomainID", "DomainName");
                //View bag of selected user    
                ViewBag.UserID = new SelectList(db.Users.Where(i => i.Id == id), "Id", "Email");
                return View(userDomain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Index"));
            }
        }
        // POST: UserDomains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,DomainID")] UserDomain userDomain)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Add user domain to DB
                    db.UserDomains.Add(userDomain);

                    //Calls create event from the Event class and returns the event
                    var domain = db.Domains.Find(userDomain.DomainID);
                    string domainName = domain.DomainName;
                    ApplicationUser thisUser = new ApplicationUser();
                    thisUser = db.Users.Find(userDomain.UserID.ToString());
                    var logEvent = Event.CreateEvent(Event.AddDomainToUser, userDomain.DomainID, domainName, thisUser.UserName, userDomain.DomainID, "null");
                    db.Events.Add(logEvent);

                    userDomain.Selected = false;

                    db.SaveChanges();
                    return RedirectToAction("Details", "UsersAdmin", new { id = userDomain.UserID.ToString() });
                }

                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName", userDomain.DomainID);
                return View(userDomain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Create"));
            }
        }
        // GET: UserDomains/Edit/5
        public ActionResult Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find user domain by ID
                UserDomain userDomain = db.UserDomains.Find(id);
                //If user domain not found return error
                if (userDomain == null)
                {
                    return HttpNotFound();
                }
                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName", userDomain.DomainID);
                return View(userDomain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Index"));
            }
        }
        // POST: UserDomains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,DomainID")] UserDomain userDomain)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(userDomain).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.DomainID = new SelectList(db.Domains, "DomainID", "DomainName", userDomain.DomainID);
                return View(userDomain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Edit"));
            }
        }
        // GET: UserDomains/Archive/5
        public ActionResult Archive(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var userDomain = db.UserDomains.Where(i => i.UserID == id).ToList();
                ViewBag.UserDomains = new SelectList(userDomain, "DomainID", "Domain.DomainName");
                UserDomainVM vm = new UserDomainVM
                {
                    UserID = id
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Index"));
            }
        }
        // POST: UserDomains/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public ActionResult ArchiveConfirmed([Bind(Include = "UserID,DomainID")] UserDomainVM userDomain)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    UserDomain d1 = null;
                    List<UserDomain> userDomains = db.UserDomains.Where(i => i.UserID == userDomain.UserID).ToList();
                    foreach (var d in userDomains)
                    {
                        if (d.DomainID == userDomain.DomainID)
                        {
                            d1 = d;
                        }
                    }

                    var containers = db.Containers.Where(i => i.DomainID == d1.DomainID && i.UserID == d1.UserID.ToString() && i.isActive).ToList();

                    //If user owns containers in the domain then the domain cannot be removed
                    if (containers.Count > 0)
                    {
                        ApplicationUser user = db.Users.Find(userDomain.UserID.ToString());
                        ModelState.AddModelError("", "Cannot remove " + d1.Domain.DomainName + " from " + user.UserName + " because " + user.UserName + " owns containers in this domain");
                        ModelState.AddModelError("", "Please reassign " + user.UserName + " containers in " + d1.Domain.DomainName + " before removing domain ");
                    }

                    //If the user has no containers in the domain then the domain can be remove
                    else
                    {
                        db.UserDomains.Remove(d1);

                        //Find domain, user and create event
                        var domain = db.Domains.Find(userDomain.DomainID);
                        ApplicationUser userName = db.Users.Find(userDomain.UserID.ToString());
                        var logEvent = Event.CreateEvent(Event.RemoveDomainFromUser, domain.DomainID, domain.DomainName, userName.UserName, domain.DomainID, "null");
                        db.Events.Add(logEvent);
                        db.SaveChanges();
                        return RedirectToAction("Details", "UsersAdmin", new { id = userDomain.UserID });

                        /*
                        string domainName = d1.Domain.DomainName;
                        ApplicationUser user = db.Users.Find(userDomain.UserID.ToString());
                        string userName = user.UserName;

                        //String for alert box
                        ViewBag.Message = "Cannot remove " + domainName + " from " + userName + " until " + userName + " containers are reassigned";

                        var domains = db.UserDomains.Where(i => i.UserID == userDomain.UserID).ToList();
                        ViewBag.UserDomains = new SelectList(domains, "DomainID", "Domain.DomainName");
                        UserDomainVM viewModel = new UserDomainVM
                        {
                            UserID = userDomain.UserID
                        };
                        return View(viewModel);
                        */
                    }
                }
                var domainList = db.UserDomains.Where(i => i.UserID == userDomain.UserID).ToList();
                ViewBag.UserDomains = new SelectList(domainList, "DomainID", "Domain.DomainName");
                UserDomainVM vm = new UserDomainVM
                {
                    UserID = userDomain.UserID
                };
                return View(vm);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UserDomains", "Archive"));
            }
        }
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
