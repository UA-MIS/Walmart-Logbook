using SecurityAssetManager.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SecurityAssetManager.Controllers
{
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index(Guid? filter)
        {
            try
            {
                //Creates new list of events
                var events = new List<Event>();

                //Checks if filter exists
                if (filter == null)
                {
                    //For users in admin or auditor role 
                    if (User.IsInRole("Admin") || User.IsInRole("Auditor"))
                    {
                        //Retrieves full list of events
                        events = db.Events.ToList();
                    }
                    //For users not in admin or auditor role 
                    else
                    {
                        //Retrieves events dependent on user's domain 
                        events = db.FilteredEvents.ToList();
                    }
                }
                else
                {
                    events = db.Events.Where(m => m.ItemID == filter).ToList();
                }

                //returns events to view
                return View(events);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Events", "Index"));
            }
        }


        // CSV METHOD:

        public void exportCSV()
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Domain\",\"Name\",\"Action\",\"User\",\"Date & Time\""); // Declaring header for each action in the event log

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Details.csv"); // Giving the file a name
            Response.ContentType = "text/csv"; // Declaring the file type of CSV

            var events = new List<Event>(); // creating the list of events
            events = db.Events.ToList(); // retrieving the list of events from the database

            foreach (var action in events) // displaying information for each action in the event log
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                    action.DomainName,
                    action.ItemName,
                    action.Action,
                    action.User,
                    action.DateTimeCreated));
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
