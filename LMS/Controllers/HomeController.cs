using LMS.Models;
using LMS.Models.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            if (Request.IsAuthenticated)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }

            return Redirect("~/");
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                User user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsStudent())
                    {
                        return Redirect("~/Student/");
                    }
                    else if (user.IsTeacher())
                    {
                        return Redirect("~/Teacher/");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Felaktig inloggning.");
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    }
                }
                else {
                    ModelState.AddModelError("", "Inloggad användare ej funnen.");
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user != null)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        if (user.IsStudent())
                        {
                            return Redirect("~/Student/");
                        }
                        else if (user.IsTeacher())
                        {
                            return Redirect("~/Teacher/");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Felaktig inloggning.");
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        }
                        break;
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "User lockout initiated, but not implemented.");
                        break;
                    case SignInStatus.RequiresVerification:
                        ModelState.AddModelError("", "Two way authorization is needed, but not implemented.");
                        break;
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Felaktig inloggning.");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("", "Felaktig inloggning.");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}