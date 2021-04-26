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
    public class LocationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Locations


        public ActionResult Index()
        {
            var locations = db.Locations.ToList().Where(i => i.isActive == true); ;
            return View(locations);
        }

        // GET: Locations/Details/5
        public ActionResult Details(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Location location = db.Locations.Find(id);
                if (location == null)
                {
                    return HttpNotFound();
                }
                IEnumerable<Container> containers = new List<Container>();
                if (User.IsInRole("Admin") || User.IsInRole("Auditor"))
                {
                    var containersFromDB = db.Containers.ToList().Where(c => c.LocationID == id);
                    containers = containersFromDB;

                }
                else
                {
                    var containersFromDB = db.FilteredContainers.ToList().Where(c => c.LocationID == id);
                    containers = containersFromDB;
                }
                LocationDetailViewModel ldvm = new LocationDetailViewModel()
                {
                    location = location,
                    containers = containers
                };
                return View(ldvm);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Locations", "Details"));
            }
        }

        // GET: Locations/Create
        [Authorize(Roles = RoleName.Admin)]

        public ActionResult Create()

        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]

        public ActionResult Create([Bind(Include = "LocationID,Name,Description")] Location location)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    location.LocationID = Guid.NewGuid();
                    location.isActive = true;
                    db.Locations.Add(location);

                    //Calls create event from the Event class and returns the event
                    var logEvent = Event.CreateEvent(Event.AddLocation, location.LocationID, location.Name, User.Identity.Name, Guid.Empty, "null");
                    db.Events.Add(logEvent);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(location);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Locations", "Create"));
            }
        }

        // GET: Locations/Edit/5
        [Authorize(Roles = RoleName.Admin)]

        public ActionResult Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Location location = db.Locations.Find(id);
                if (location == null)
                {
                    return HttpNotFound();
                }
                return View(location);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Locations", "Index"));
            }
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]

        public ActionResult Edit([Bind(Include = "LocationID,Name,Description,isActive")] Location location, string justification)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(location).State = EntityState.Modified;

                    //Calls create event from the Event class and returns the event
                    var logEvent = Event.CreateEvent(Event.EditLocation, location.LocationID, location.Name, User.Identity.Name, Guid.Empty, justification);
                    db.Events.Add(logEvent);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(location);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Locations", "Edit"));
            }
        }

        // GET: Locations/Archive/5
        [Authorize(Roles = RoleName.Admin)]

        public ActionResult Archive(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Location location = db.Locations.Find(id);
                if (location == null)
                {
                    return HttpNotFound();
                }
                return View(location);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Locations", "Index"));
            }
        }
        // POST: Locations/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Admin)]

        public ActionResult ArchiveConfirmed(Guid id, string justification)
        {
            try
            {
                Location location = db.Locations.Find(id);

                //Calls create event from the Event class and returns the event
                var logEvent = Event.CreateEvent(Event.ArchiveLocation, location.LocationID, location.Name, User.Identity.Name, Guid.Empty, justification);
                db.Events.Add(logEvent);

                location.isActive = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Locations", "Archive"));
            }
        }

        // CSV METHOD:

        public void exportCSV()

        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Location Name\",\"Description\""); // Declaring header for each location in the location index

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Details.csv"); // Giving the file a name
            Response.ContentType = "text/csv"; // Declaring the file type of CSV

            var locations = new List<Location>(); // creating the list of locations
            locations = db.Locations.ToList(); // retrieving the list of locations from the database

            foreach (var locations1 in locations) // displaying information for each location in the location index

            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\"",
                    locations1.Name,
                    locations1.Description));
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
