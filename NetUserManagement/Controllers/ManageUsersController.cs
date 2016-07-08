

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using NetUserManagement.Models;

/// <summary>
/// This allows the admin to manage who has use. There are also user roles involved: admin, researcher, developer, locked.
/// <remarks>The only role that does anything right now is the developer is able to see pages in development and some comments and buttons. Only the admin is able to make user changes, and locked would lock people out. There was a delete user button, but this won't work unless their role is first removed. This wouldn't be to hard to implement</remarks>
/// </summary>
namespace NetUserManagement.Controllers
{

    [Authorize(Roles = "Developer,Admin")]
    public class ManageUsersController : Controller
    {
       
        private MVCdevelopment_QAEntities db = new MVCdevelopment_QAEntities();

        // GET: ManageUsers
        public async Task<ActionResult> Index()
        {
            return View(await db.AspNetUsers.ToListAsync());
        }

        // GET: ManageUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }
        /// <summary>
        /// This doesn't do anything anymore, the creat user is located in the Account controller allowing us to create a new user. If everyone is locked out the programmer may reenable the register page just for him.
        /// </summary>
        /// <returns></returns>
        // GET: ManageUsers/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: ManageUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

       


            //access create user from account controller
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////need to just take username, email, and password, leave others blank
        //public async Task<ActionResult> Create([Bind(Include = "Email,PasswordHash,PhoneNumber,PhoneNumberConfirmed,UserName")] AspNetUser aspNetUser)
        //    //"Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspNetUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.AspNetUsers.Add(aspNetUser);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(aspNetUser);
        //}

            ///<summary>allow for changes of minor details like username, email, and phone</summary>
        // GET: ManageUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = id;
            return View(aspNetUser);
        }
        /// <summary>
        /// Important Function, this doesnt really add, but changes the role. First it removes all roles the person may have and then ads the role selected. Multiple roles may be used, but really only one role per person is needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> AddRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = id;
            return View(/*aspNetUser*/);
        }
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(string id, string roleDrop)
        {
            System.Diagnostics.Debug.WriteLine("new role:" + roleDrop);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser =  db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            System.Diagnostics.Debug.WriteLine("user to edit" + aspNetUser.UserName + "id:" + aspNetUser.Id);
            if (roleDrop != "")
            {
                using (var context = new Models.ApplicationDbContext())
                {
                    var userStore = new UserStore<Models.ApplicationUser>(context);
                    var userManager = new UserManager<Models.ApplicationUser>(userStore);
                    //AspNetUser CurrentUser = db.AspNetUsers.Find(id);
                    //AspNetUser UserBeingEdited = db.AspNetUsers.Find(id);
                    string[] allUserRoles = userManager.GetRoles(id).ToArray();
                    if (allUserRoles.Length > 0)
                    {


                        System.Diagnostics.Debug.WriteLine(allUserRoles[0]);

                        userManager.RemoveFromRoles(id, allUserRoles);
                    }
                    //UserManager.RemoveFromRoles(id, allUserRoles);
                    //if (roles.Count() > 0)
                    //{
                    //    UserManager.AddToRoles(id, roles);
                    //}

                    //db.AspNetRoles.Select(s => s.Name).ToArray();
                    //userManager.clear
                    //userManager.RemoveFromRoles(id,["Admin, Researcher,Developer"]);
                    userManager.AddToRole(id, roleDrop);
                    if (id == User.Identity.GetUserId())
                    {
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return RedirectToAction("Index", "Home");
                    }


                }

            }
            //ViewBag.UserID = id;
            ViewBag.UserID = id;
            return View(/*aspNetUser*/);

        }
        /// <summary>
        /// I really do not think that this does anything
        /// </summary>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // POST: ManageUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }
        /// <summary>
        /// Like stated earlier delete will not work until roles are also removed from the person, besides locked is better anyways, if you wish to hide people locked just chagne the .tolist() to not show locked people
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: ManageUsers/Delete/5
       /* public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: ManageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            db.AspNetUsers.Remove(aspNetUser);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        */
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
