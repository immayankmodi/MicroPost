using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MicroPost.DataModel;

namespace MicroPost.Models {
    public class UserModel {

        MicroPostEntities db = new MicroPostEntities();

        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name: ")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsEmailAvailable", "User", HttpMethod = "POST", ErrorMessage = "Email already exists. Please enter a different one.")]
        [StringLength(50)]
        [Display(Name = "Email: ")]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        [Display(Name = "Email: ")]
        public string EmailId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Address: ")]
        public string Address { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password: ")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        public string PageCount { get; set; }

        public string PageNumber { get; set; }

        public User User { get; set; }

        public List<User> Users { get; set; }

        public List<Post> Posts { get; set; }

        public UserModel GetAllUsers(UserModel model, string sortingOrder, string searchText) {

            if (!string.IsNullOrEmpty(searchText))
                model.Users = db.Users.Where(u => u.Name.ToLower().Contains(searchText.ToLower())).ToList();
            else
                model.Users = db.Users.ToList();

            switch (sortingOrder) {
                case "Name":
                    model.Users = model.Users.OrderByDescending(u => u.Name).ToList();
                    break;
                case "Email":
                    model.Users = model.Users.OrderByDescending(u => u.Email).ToList();
                    break;
                case "Address":
                    model.Users = model.Users.OrderByDescending(u => u.Address).ToList();
                    break;
                default:
                    model.Users = model.Users.OrderByDescending(u => u.UserId).ToList();
                    break;
            }

            return model;
        }

        public int RegisterUser(UserModel model) {
            int userId = 0;
            User user = new User();
            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address;
            user.Password = model.Password;
            db.Users.Add(user);
            db.SaveChanges();
            userId = user.UserId;
            return userId;
        }

        public UserModel CheckLogin(UserModel model) {
            model.User = db.Users.Where(u => u.Email == model.EmailId && u.Password == model.Password).FirstOrDefault();
            return model;
        }

        public UserModel GetUserProfile(UserModel model, int userId) {
            model.User = db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (model.User != null && model.User.UserId > 0) {
                model.UserId = model.User.UserId;
                model.Name = model.User.Name;
                model.Email = model.User.Email;
                model.Password = model.User.Password;
                model.Address = model.User.Address;
            }
            return model;
        }

        public UserModel EditProfile(UserModel model, int userId) {
            model.User = db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (model.User != null && model.User.UserId > 0) {
                model.User.Name = model.Name;
                model.User.Password = model.Password;
                model.User.Address = model.Address;
                model.User.Email = model.EmailId;
                db.SaveChanges();
            }
            return model;
        }

        public UserModel GetProfiles(UserModel model, int userId) {
            model.User = db.Users.Where(u => u.UserId == userId).FirstOrDefault();
            model.Posts = db.Posts.Where(u => u.UserId == userId && u.IsPublicPost == true).OrderByDescending(x => x.PostId).ToList();
            return model;
        }

        public User CheckEmail(string email) {
            User user = new User();
            user = db.Users.Where(p => p.Email == email).FirstOrDefault();
            return user;
        }
    }
}