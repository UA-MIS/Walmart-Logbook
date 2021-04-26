using SecurityAssetManager.Models;
using SecurityAssetManager.Models.ViewModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SecurityAssetManager.Controllers
{
    [Authorize]
    public class DomainsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /*Purpose: Return a list of domains to the Domain Index view 
         * Creates list of containers from database that includes the location it is found in but
         is limited to instances only where the domain object is present in the database.*/
        // GET: Domains
        public ActionResult Index()
        {
            return View(db.Domains.ToList());
        }
        /*Purpose: Display Domain data  
        * Need to check if domain exists to make sure its data can be displayed 
        If ID is NOT null: 
         * Searches for domain with that name
        If domain is NOT null: 
        * Creates list of users that exist under domain but only for instances
         where users are present in database and domain has a name         
        * Once this is done, an instance of a viewmodel can be created and returned to Detail view
        in order to allow the domain and its users to be displayed.*/
        // GET: Domains/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find domain by ID
            Domain domain = db.Domains.Find(id);
            //New domain view model
            DomainDetailViewModel ddvm = new DomainDetailViewModel();
            //If no domain found return error
            if (domain == null)
            {
                return HttpNotFound();
            }
            ddvm.domain = domain;
            var userDomains = db.UserDomains.Include(m => m.Domain).ToList().Where(m => m.Domain == domain);
            List<ApplicationUser> users = new List<ApplicationUser>();
            //Find users in each domain
            foreach (UserDomain u in userDomains)
            {
                var user = db.Users.Find(u.UserID.ToString());
                users.Add(user);
            }
            ddvm.users = users;

            return View(ddvm);
        }
        /*Purpose: Retrieve information from view 
         * HttpPost is called after information is filled out and submitted in view
         and checks to make sure everything selected/typed in view was done properly. 
         Data is bound to the model to specify exact properties needed, will be verified 
         if was bound correctly.
         
         If this data is valid and the user is an admin role they will be able to create an domain:
         * A domain can then be created and added to the database. 
         * A new event to log the domain creation can also then be created and added to the database.
         * After this, the user will be rediected to the Index view. 
         If data is not valid: 
         * Viewbags will be recreated and the view will reload. 
         */
        // GET: Domains/Create
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult Create()
        {
            return View();
        }


        // POST: Domains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult Create([Bind(Include = "DomainID, Description, DomainName")] Domain domain)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (domain.Description == null || domain.Description.Length < 6 || domain.Description.Length > 255)
                    {
                        ModelState.AddModelError("Description", "Description must be between 6 and 255 characters.");
                    }
                    else
                    {
                        //Create new domain and assign new GUID
                        domain.DomainID = Guid.NewGuid();
                        //Add new domain to DB
                        db.Domains.Add(domain);

                        //Calls create event from the Event class and returns the event
                        var logEvent = Event.CreateEvent(Event.AddDomain, domain.DomainID, domain.DomainName, User.Identity.Name, domain.DomainID, "null");
                        db.Events.Add(logEvent);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                return View(domain);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Domains", "Create"));
            }
        }
        /*Purpose: Retrieve information from view 
         * HttpPost is called after information is filled out and submitted in view
         and checks to make sure everything selected/typed in view was done properly. 
         Data is bound to the model to specify exact properties needed, will be verified 
         if was bound correctly.


         If this data is valid and the user is in admin role:
         *First ensures that domain data can be updated.
         * A new event to log that the domain has been edited can also then be 
         created and added to the database.
         *Confirms domain is still present and saves event to database.
         * After this, the user will be rediected to the Index view to see these changes made to domain. 
         If data is not valid: 
         * Viewbags will be recreated and the view will reload. 
         */
        // GET: Domains/Edit/5
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find domain by ID
                Domain domain = db.Domains.Find(id);
                //If domain is not found return error
                if (domain == null)
                {
                    return HttpNotFound();
                }
                return View(domain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Domains", "Index"));
            }
        }
        // POST: Domains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult Edit([Bind(Include = "DomainID, Description, DomainName")] Domain domain, string justification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (domain.Description == null || domain.Description.Length < 6 || domain.Description.Length > 255)
                    {
                        ModelState.AddModelError("Description", "Description must be between 6 and 255 characters.");
                    }
                    else
                    {
                        db.Entry(domain).State = EntityState.Modified;

                        //Calls create event from the Event class and returns the event
                        var logEvent = Event.CreateEvent(Event.EditDomain, domain.DomainID, domain.DomainName, User.Identity.Name, domain.DomainID, justification);
                        db.Events.Add(logEvent);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                return View(domain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Domains", "Edit"));
            }
        }
        /*Purpose: Retrieve information from view 
         * HttpPost is called after information is filled out and submitted in view
         and checks to make sure everything selected/typed in view was done properly. 
         Data is bound to the model to specify exact properties needed, will be verified 
         if was bound correctly.*/

        /* Purpose: Send information to archive view 
         * Authorizes admin to perform this action
       * Need to check if domain exists in order to confirm that it can be archived 
       * If ID is not null: 
       * Searches for domain in that ID
       * If domain is not null: 
       * Will then return view and user will be brought to Archive view.
       */
        // GET: Domains/Archive/5
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult Archive(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Find domain by ID
                Domain domain = db.Domains.Find(id);
                //If domain is not found return error
                if (domain == null)
                {
                    return HttpNotFound();
                }
                return View(domain);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Domains", "Index"));
            }
        }
        // POST: Domains/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]
        public ActionResult ArchiveConfirmed(Guid id, string justification)
        {
            try
            {
                Domain domain = db.Domains.Find(id);

                //Calls create event from the Event class and returns the event

                var logEvent = Event.CreateEvent(Event.ArchiveDomain, domain.DomainID, domain.DomainName, User.Identity.Name, domain.DomainID, justification);

                db.Events.Add(logEvent);
                //Remove domain from DB
                db.Domains.Remove(domain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Domains", "Archive"));
            }
        }

        // CSV METHOD:



        public void exportCSV()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Domain Name\",\"Description\""); // Declaring header for each domain in the domain index

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Details.csv"); // Giving the file a name
            Response.ContentType = "text/csv"; // Declaring the file type of CSV

            var domains = new List<Domain>(); // creating the list of domains
            domains = db.Domains.ToList(); // retrieving the list of domains from the database

            foreach (var domains1 in domains) // displaying information for each action in the domain index
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\"",
                    domains1.DomainName,
                    domains1.Description));
            }

            Response.Write(sw.ToString());
            Response.End();
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
