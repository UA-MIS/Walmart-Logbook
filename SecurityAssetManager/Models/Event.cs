using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SecurityAssetManager.Models
{
    public class Event
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public Event()
        {
            this.DateTimeCreated = DateTime.Now; // auto-populates DateTimeCreated field
        }

        //All attributes associated with event
        [Key]
        public Guid EventID { get; set; }

        [Required]
        [Display(Name = "Event Code")]
        public int EventCode { get; set; }

        [Required]
        [Display(Name = "Description")]
        public String EventDescription { get; set; }

        [Required]
        public String Justification { get; set; }

        [Required]
        public String Action { get; set; }

        public string User { get; set; }

        [Display(Name = "Date")]
        public DateTime DateTimeCreated { get; set; }

        public Guid ItemID { get; set; }

        public String ItemName { get; set; }

        public Guid? DomainID { get; set; }
        public string DomainName { get; set; }

        //Creates a new event
        public static Event CreateEvent(int eventCode, Guid id, string name, string userName, Guid domainId, string justification)
        {
            string actionString = "";
            string eventDesc = "";

            //Sets description and action based on event code
            switch (eventCode)
            {
                // Login Event
                case 0: // user logged in
                    actionString = "Logged In";
                    eventDesc = "Login";
                    break;

                // CRUD Item Events
                case 1: // add item
                    actionString = "Item Created";
                    eventDesc = "Item";
                    break;
                case 2: // edit item
                    actionString = "Item Edited";
                    eventDesc = "Item";
                    break;
                case 3: // archive item
                    actionString = "Item Archived";
                    eventDesc = "Item";
                    break;
                case 4: // duplicate item
                    actionString = "Item Duplicated";
                    eventDesc = "Item";
                    break;

                // CRUD Container Events
                case 5: // add container
                    actionString = "Container Created";
                    eventDesc = "Container";
                    break;
                case 6: // edit container
                    actionString = "Container Edited";
                    eventDesc = "Container";
                    break;
                case 7: // archived container
                    actionString = "Container Archived";
                    eventDesc = "Container";
                    break;

                // CRUD Location Events
                case 8: // add location
                    actionString = "Location Created";
                    eventDesc = "Location";
                    break;
                case 9: // edit location
                    actionString = "Location Edited";
                    eventDesc = "Location";
                    break;
                case 10: // archive location
                    actionString = "Location Archived";
                    eventDesc = "Location";
                    break;

                // CRUD Domain Events
                case 11: // add domain
                    actionString = "Domain Created";
                    eventDesc = "Domain";
                    break;
                case 12: // edit domain
                    actionString = "Domain Edited";
                    eventDesc = "Domain";
                    break;
                case 13: // archived domain
                    actionString = "Domain Archived";
                    eventDesc = "Domain";
                    break;
                case 14: // add domain to user
                    actionString = "Domain Added to User";
                    eventDesc = "Domain";
                    break;
                case 15: // remove domain from user
                    actionString = "Domain Removed from User";
                    eventDesc = "Domain";
                    break;

                // CRUD User Events
                case 16: // add user
                    actionString = "User Created";
                    eventDesc = "User";
                    break;
                case 17: // edit user
                    actionString = "User Edited";
                    eventDesc = "User";
                    break;
                case 18: // archived user
                    actionString = "User Archived";
                    eventDesc = "User";
                    break;

                // Checking In/Out Event
                case 19: // check in item
                    actionString = "Item Checked In";
                    eventDesc = "Item";
                    break;
                case 20: // check out item
                    actionString = "Item Checked Out";
                    eventDesc = "Item";
                    break;
            }
            string domainName = "N/A";

            if (domainId != Guid.Empty && eventCode != AddDomain)
            {
                var domain = db.Domains.FirstOrDefault(i => i.DomainID == domainId);
                domainName = domain.DomainName;
            }

            //Creates the event
            Event logEvent = (new Event()
            {
                EventID = Guid.NewGuid(),
                EventCode = eventCode,
                EventDescription = eventDesc,
                Action = actionString,
                User = userName,
                Justification = justification,
                ItemID = id,
                ItemName = name,
                DomainID = domainId,
                DomainName = domainName
            });

            return logEvent;
        }

        public static Event CreateVerboseEvent(int eventCode, Guid id, string name, string userName, Guid domainId, string justification, string verboseDesc)
        {
            string eventDesc = "";
            string actionString = verboseDesc;
            switch (eventCode)
            {
                // Login Event
                case 0: // user logged in
                    eventDesc = "Login";
                    break;

                // CRUD Item Events
                case 1: // add item
                    eventDesc = "Item";
                    break;
                case 2: // edit item
                    eventDesc = "Item";
                    break;
                case 3: // archive item
                    eventDesc = "Item";
                    break;
                case 4: // duplicate item
                    eventDesc = "Item";
                    break;

                // CRUD Container Events
                case 5: // add container
                    eventDesc = "Container";
                    break;
                case 6: // edit container
                    eventDesc = "Container";
                    break;
                case 7: // archived container
                    eventDesc = "Container";
                    break;

                // CRUD Location Events
                case 8: // add location
                    eventDesc = "Location";
                    break;
                case 9: // edit location
                    eventDesc = "Location";
                    break;
                case 10: // archive location
                    eventDesc = "Location";
                    break;

                // CRUD Domain Events
                case 11: // add domain
                    eventDesc = "Domain";
                    break;
                case 12: // edit domain
                    eventDesc = "Domain";
                    break;
                case 13: // archived domain
                    eventDesc = "Domain";
                    break;
                case 14: // add domain to user
                    eventDesc = "Domain";
                    break;
                case 15: // remove domain from user
                    eventDesc = "Domain";
                    break;

                // CRUD User Events
                case 16: // add user
                    eventDesc = "User";
                    break;
                case 17: // edit user
                    eventDesc = "User";
                    break;
                case 18: // archived user
                    eventDesc = "User";
                    break;

                // Checking In/Out Event
                case 19: // check in item
                    eventDesc = "Item";
                    break;
                case 20: // check out item
                    eventDesc = "Item";
                    break;
            }

            string domainName = "N/A";

            if (domainId != Guid.Empty && eventCode != AddDomain)
            {
                var domain = db.Domains.FirstOrDefault(i => i.DomainID == domainId);
                domainName = domain.DomainName;
            }

            //Creates the event
            Event logEvent = (new Event()
            {
                EventID = Guid.NewGuid(),
                EventCode = eventCode,
                EventDescription = eventDesc,
                Action = actionString,
                User = userName,
                Justification = justification,
                ItemID = id,
                ItemName = name,
                DomainID = domainId,
                DomainName = domainName
            });

            return logEvent;
        }

        //Event Codes
        public static readonly int Login = 0;

        public static readonly int AddItem = 1;
        public static readonly int EditItem = 2;
        public static readonly int ArchiveItem = 3;
        public static readonly int DuplicateItem = 4;

        public static readonly int AddContainer = 5;
        public static readonly int EditContainer = 6;
        public static readonly int ArchiveContainer = 7;

        public static readonly int AddLocation = 8;
        public static readonly int EditLocation = 9;
        public static readonly int ArchiveLocation = 10;

        public static readonly int AddDomain = 11;
        public static readonly int EditDomain = 12;
        public static readonly int ArchiveDomain = 13;
        public static readonly int AddDomainToUser = 14;
        public static readonly int RemoveDomainFromUser = 15;

        public static readonly int AddUser = 16;
        public static readonly int EditUser = 17;
        public static readonly int ArchiveUser = 18;

        public static readonly int CheckedIn = 19;
        public static readonly int CheckedOut = 20;


    }
}