using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SecurityAssetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SecurityAssetManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
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
            set
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
        // GET: /Roles/
        public ActionResult Index()
        {
            try
            {
                return View(RoleManager.Roles);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Index"));
            }
        }
        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                // Get the list of Users in this Role
                var users = new List<ApplicationUser>();

                // Get the list of Users in this Role
                foreach (var user in UserManager.Users.ToList())
                {
                    if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                    {
                        users.Add(user);
                    }
                }

                ViewBag.Users = users;
                ViewBag.UserCount = users.Count();
                return View(role);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Details"));
            }
        }
        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Index"));
            }
        }
        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new ApplicationRole(roleViewModel.Name);
                    var roleresult = await RoleManager.CreateAsync(role);
                    if (!roleresult.Succeeded)
                    {
                        ModelState.AddModelError("", roleresult.Errors.First());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Create"));
            }
        }
        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
                return View(roleModel);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Index"));
            }
        }
        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = await RoleManager.FindByIdAsync(roleModel.Id);
                    role.Name = roleModel.Name;
                    await RoleManager.UpdateAsync(role);
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Edit"));
            }
        }
        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View("Archive", role);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Index"));
            }
        }
        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var role = await RoleManager.FindByIdAsync(id);
                    if (role == null)
                    {
                        return HttpNotFound();
                    }
                    IdentityResult result;
                    if (deleteUser != null)
                    {
                        result = await RoleManager.DeleteAsync(role);
                    }
                    else
                    {
                        result = await RoleManager.DeleteAsync(role);
                    }
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return View("Archive");
                    }
                    return RedirectToAction("Index");
                }
                return View("Archive");
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "RolesAdmin", "Delete"));
            }
        }
    }
}
