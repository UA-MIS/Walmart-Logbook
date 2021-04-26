using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SecurityAssetManager.Models
{

    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin,
        ApplicationUserRole, ApplicationUserClaim>
    {

        public virtual ICollection<UserDomain> UserDomains { get; set; }


        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            // Add any custom User properties/code here
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    // Must be expressed in terms of our custom UserRole:
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }

        // Add any custom Role properties/code here
    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,
        string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, SecurityAssetManager.Migrations.Configuration>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<UserDomain> UserDomains { get; set; }

        //Filters Container DBSet to only containers belonging to the current user
        public IQueryable<Container> FilteredContainers
        {
            get
            {
                //Gets the current user
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                //Gets all domains of the current user
                var userDomains = UserDomains.Include(i => i.Domain).Where(i => i.UserID.ToString() == user.Id).ToList();
                //Empty IQueryable that will get populated after the foreach statements 
                IQueryable<Container> filteredContainers = Enumerable.Empty<Container>().AsQueryable();
                //Empty List that will get populated by the foreach statements
                List<Container> containerList = filteredContainers.ToList();

                
                foreach (var container in Containers.ToList())
                {
                    //Checks if the container is active and if it belongs to the user
                    if (container.isActive && container.UserID == System.Web.HttpContext.Current.User.Identity.GetUserId())
                    {
                        //If true, add the container to the filtered list
                        containerList.Add(container);
                    }
                }

                //Converts list to IQueryable so it can be returned
                filteredContainers = containerList.AsQueryable();
                return filteredContainers;
            }
        }

        //Filters Container DBSet to only containers in the current user's domains
        public IQueryable<Container> DomainFilteredContainers
        {
            get
            {
                //Gets the current user
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                //Gets all domains of the current user
                var userDomains = UserDomains.Include(i => i.Domain).Where(i => i.UserID.ToString() == user.Id).ToList();
                //Empty IQueryable that will get populated after the foreach statements 
                IQueryable<Container> filteredContainers = Enumerable.Empty<Container>().AsQueryable();
                //Empty List that will get populated by the foreach statements
                List<Container> containerList = filteredContainers.ToList();


                foreach (var container in Containers.ToList())
                {
                    //Checks if the container is active
                    if (container.isActive)
                    {
                        //If true, add the container to the filtered list
                        containerList.Add(container);
                    }
                }

                //Converts list to IQueryable so it can be returned
                filteredContainers = containerList.AsQueryable();
                return filteredContainers;
            }
        }

        //Filters Item DBSet to only items in the current user's containers
        public IQueryable<Item> FilteredItems
        {
            get
            {
                //Gets the containers filtered to the current user
                var filteredContainers = FilteredContainers.Include(c => c.Location).ToList();
                //Empty IQueryable that will get populated after the foreach statements 
                IQueryable<Item> filteredItems = Enumerable.Empty<Item>().AsQueryable();
                //Empty List that will get populated by the foreach statements
                List<Item> itemList = filteredItems.ToList();

                foreach (var item in Items.ToList())
                {
                    foreach (var container in filteredContainers)
                    {
                        if (item.isActive && item.ContainerID == container.ContainerID)
                        {
                            itemList.Add(item);
                        }
                    }
                }

                filteredItems = itemList.AsQueryable();
                return filteredItems;
            }
        }

        //Filters Item DBSet to only items in the current user's domains
        public IQueryable<Item> DomainFilteredItems
        {
            get
            {
                //Gets the containers filtered to the current user
                var filteredContainers = DomainFilteredContainers.Include(c => c.Location).ToList();
                //Empty IQueryable that will get populated after the foreach statements 
                IQueryable<Item> filteredItems = Enumerable.Empty<Item>().AsQueryable();
                //Empty List that will get populated by the foreach statements
                List<Item> itemList = filteredItems.ToList();

                foreach (var item in Items.ToList())
                {
                    foreach (var container in filteredContainers)
                    {
                        if (item.isActive && item.ContainerID == container.ContainerID)
                        {
                            itemList.Add(item);
                        }
                    }
                }

                filteredItems = itemList.AsQueryable();
                return filteredItems;
            }
        }

        //Filters Event DBSet to only events in the current user's domains
        public IQueryable<Event> FilteredEvents
        {
            get
            {
                //Gets the current user
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                //Gets all domains of the current user
                var userDomains = UserDomains.Include(i => i.Domain).Where(i => i.UserID.ToString() == user.Id).ToList();
                //Empty IQueryable that will get populated after the foreach statements 
                IQueryable<Event> filteredEvents = Enumerable.Empty<Event>().AsQueryable();
                //Empty List that will get populated by the foreach statements
                List<Event> eventList = filteredEvents.ToList();

                foreach (var e in Events.ToList())
                {
                    foreach (var domain in userDomains)
                    {
                        //Checks if the container is active and if it belongs to one of the user's domains
                        if (e.DomainID == domain.DomainID)
                        {
                            //If true, add the container to the filtered list
                            eventList.Add(e);
                        }
                    }
                }

                //Converts list to IQueryable so it can be returned
                filteredEvents = eventList.AsQueryable();
                return filteredEvents;
            }
        }


    }


    public class ApplicationUserStore
        : UserStore<ApplicationUser, ApplicationRole, string,
            ApplicationUserLogin, ApplicationUserRole,
            ApplicationUserClaim>, IUserStore<ApplicationUser, string>,
        IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationRoleStore
    : RoleStore<ApplicationRole, string, ApplicationUserRole>,
    IQueryableRoleStore<ApplicationRole, string>,
    IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }

}