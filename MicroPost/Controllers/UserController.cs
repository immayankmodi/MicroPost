using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MicroPost.DataModel;
using MicroPost.Helpers;
using MicroPost.Models;
using PagedList;

namespace MicroPost.Controllers {
    public class UserController : Controller {

        [HttpGet]
        public ActionResult Index(string sortingOrder, string searchText, string filterValue, int? pageNo) {
            UserModel model = new UserModel();
            try {
                ViewBag.CurrentSortOrder = sortingOrder;
                if (searchText != null) {
                    pageNo = 1;
                } else {
                    searchText = filterValue;
                }
                ViewBag.FilterValue = searchText;
                model.GetAllUsers(model, sortingOrder, searchText);

                int pageSize = 3; //change the number you want to show users per page
                int pageNumber = (pageNo ?? 1); //if no of pages are less than 3 then take by default 1
                return View(model.Users.ToPagedList(pageNumber, pageSize));
            } catch (Exception ex) { }

            return View(model.Users.ToPagedList(1, 3));
        }

        [HttpGet]
        public ActionResult Register() {
            UserModel model = new UserModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(UserModel model) {
            try {
                ModelState.Remove("EmailId");
                if (ModelState.IsValid) {
                    int userId = model.RegisterUser(model);
                    FormsAuthentication.SetAuthCookie(userId.ToString(), false);
                    return RedirectToAction("Profiles", "User", new { id = userId });

                } else {
                    ModelState.AddModelError("", "All fields are required");
                }
            } catch (Exception ex) {
                ModelState.AddModelError("", "Error while registration");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel model) {
            try {
                model.CheckLogin(model);
                if (model.User != null && model.User.UserId > 0) {
                    FormsAuthentication.SetAuthCookie(model.User.UserId.ToString(), false);
                    return RedirectToAction("Profiles", "User", new { id = model.User.UserId });
                } else {
                    ModelState.AddModelError("", "Incorrect login details");
                }
            } catch (Exception ex) {
                ModelState.AddModelError("", "Error while login");
            }

            return View();
        }

        public ActionResult Logout() {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "User");
        }

        [HttpGet]
        public ActionResult EditProfile() {
            UserModel model = new UserModel();
            try {
                int userId = Convert.ToInt32(User.Identity.Name);
                model.GetUserProfile(model, userId);
                model.EmailId = model.User.Email;
            } catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditProfile(UserModel model) {
            try {
                ModelState.Remove("Email");
                int userId = Convert.ToInt32(User.Identity.Name);
                model.EditProfile(model, userId);
                ViewBag.Message = "";
            } catch (Exception ex) {
                ViewBag.Message = "Error while updating profile.";
                ModelState.AddModelError("", "Error while updating details");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ForgetPassword() {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(UserModel model) {
            try {
                model.User = model.CheckEmail(model.EmailId);
                if (model.User != null && model.User.UserId > 0) {
                    string to = model.User.Email;
                    string subject = "Micro Post - Password Reminder";
                    string body = "Login details are as follows. <hr/> Email: " + to + " <br/> Password: " + model.User.Password;
                    bool IsSent = EmailManager.SendEmail(to, subject, body);
                    ViewBag.Message = IsSent ? "" : "Error while sending email.";
                }
            } catch (Exception ex) {
                ViewBag.Message = "Error while sending email.";
            }

            return View(model);
        }

        public ActionResult Profiles(int? id) {
            UserModel model = new UserModel();
            try {
                model.GetProfiles(model, (id ?? 0));
            } catch { }
            return View(model);
        }

        [HttpPost]
        public JsonResult IsEmailAvailable(string email) {
            UserModel model = new UserModel();
            try {
                model.User = model.CheckEmail(email);
            } catch { }
            return Json(model.User == null);
        }
    }
}