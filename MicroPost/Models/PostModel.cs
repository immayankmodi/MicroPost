using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MicroPost.DataModel;

namespace MicroPost.Models {
    public class PostModel {

        MicroPostEntities db = new MicroPostEntities();

        public int PostId { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Post: ")]
        public string PostText { get; set; }

        public bool IsPublicPost { get; set; }

        public Post Post { get; set; }

        public List<Post> Posts { get; set; }

        public PostModel GetAllPosts(PostModel model) {
            model.Posts = db.Posts.Where(p => p.IsPublicPost == true).OrderByDescending(x => x.PostId).ToList();
            return model;
        }

        public void AddPost(PostModel model, int userId) {
            Post post = new Post();
            post.PostText = model.PostText;
            post.IsPublicPost = model.IsPublicPost;
            post.UserId = userId;
            db.Posts.Add(post);
            db.SaveChanges();
        }

        public PostModel GetMyPosts(PostModel model, int userId) {
            model.Posts = db.Posts.Where(u => u.UserId == userId).OrderByDescending(x => x.PostId).ToList();
            return model;
        }
    }
}