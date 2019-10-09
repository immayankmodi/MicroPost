using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroPost.DataModel;
using MicroPost.Models;

namespace MicroPost.Controllers {
    public class PostController : Controller {

        public ActionResult Index() {
            PostModel model = new PostModel();
            try {
                model.GetAllPosts(model);
            } catch (Exception ex) { }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PostModel model, FormCollection frm) {
            if (ModelState.IsValid) {
                try {
                    ViewBag.Error = "";
                    if (string.IsNullOrEmpty(model.PostText)) {
                        ViewBag.Error = "ERROR: Post data can not be empty.";
                        return View();
                    } else if (!string.IsNullOrEmpty(model.PostText) && model.PostText.Length > 250) {
                        ViewBag.Error = "ERROR: Post data can not be grater that 250.";
                        return View();
                    }
                    int userId = Convert.ToInt32(User.Identity.Name);
                    model.AddPost(model, userId);
                } catch (DbEntityValidationException e) {
                    foreach (var eve in e.EntityValidationErrors) {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors) {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                } catch (Exception ex) { }
            } else {
                ModelState.AddModelError("", "Post data is not correct");
            }

            return RedirectToAction("Index");
        }

        public ActionResult MyPosts() {
            PostModel model = new PostModel();
            try {
                int userId = Convert.ToInt32(User.Identity.Name);
                model.GetMyPosts(model, userId);
            } catch (Exception ex) { }

            return View(model);
        }

    }
}
