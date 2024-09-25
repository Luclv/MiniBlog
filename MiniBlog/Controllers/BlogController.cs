using AutoMapper;
using Microsoft.AspNet.Identity;
using MiniBlog.Helper;
using MiniBlog.Models;
using MiniBlog.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniBlog.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public BlogController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Blogs
        // Show all blogs (paginated)
        public ActionResult Index(int? page = 1)
        {
            List<Blog> blogs = _context.Blogs.ToList();

            var state = User.IsInRole(Const.AdminRole) ? "Edit" : "View";
            return View(PrepareViewModel(blogs, state, page));
        }

        [Authorize]
        public ActionResult ViewOwnBlogs(int? page)
        {
            string userId = User.Identity.GetUserId();
            List<Blog> blogs = _context.Blogs.Where(b => b.BlogOwnerId == userId).ToList();

            return View("Index", PrepareViewModel(blogs, "Edit", page));
        }

        // New Blog
        // Create a blog, must be logged in
        [Authorize]
        public ActionResult New()
        {
            var model = new BlogViewModel();
            return View(model);
        }

        // Edit
        // Edit an existing blog
        [Authorize]
        public ActionResult Edit(int id)
        {
            Blog blog = null;
            if (id == 0)
            {
                // we are creating a new blog
                blog = new Blog();
            }
            else
            {
                // We are editing an existing blog

                blog = _context.Blogs.SingleOrDefault(b => b.Id == id);

                if (blog is null)
                {
                    return View("Error");
                }
                if (blog.BlogOwnerId != User.Identity.GetUserId() && !User.IsInRole(Const.AdminRole))
                {
                    return View("Unauthorized");
                }
            }
            var viewModel = Mapper.Map<BlogViewModel>(blog);

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BlogViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return model.Id > 0 ? View("Edit", model) : View("New", model);
            }

            HttpPostedFileBase file = Request.Files["ImageData"];

            if (model.Id == 0)
            {
                // create new blog
                var blog = new Blog
                {
                    Content = model.Content,
                    Title = model.Title,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                    Status = StatusEnum.Draft.ToString(),
                    Banner = ConvertToBytes(file),
                    BlogOwnerId = User.Identity.GetUserId()
                };

                _context.Blogs.Add(blog);
            }
            else
            {
                // edit existing blog
                Blog oldBlog = _context.Blogs.SingleOrDefault(b => b.Id == model.Id);
                if (oldBlog is null)
                {
                    // something went wrong
                    return View("Error");
                }
                oldBlog.ModifiedAt = DateTime.Now;
                var uploadData = ConvertToBytes(file);
                if (uploadData != null)
                {
                    oldBlog.Banner = uploadData;
                }
                oldBlog.Title = model.Title;
                oldBlog.Content = model.Content;
                oldBlog.Status = GetValueFromEnum(model.Status);
            }

            _context.SaveChanges();

            return RedirectToAction("ViewOwnBlogs");
        }

        // View Blog
        // Show one blog in reading format
        public ActionResult Read(int id)
        {
            Blog blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return View("Error");
            }

            var viewModel = Mapper.Map<BlogDetailViewModel>(blog);

            return View(viewModel);
        }

        public ActionResult GetImage(int id)
        {
            Blog blog = _context.Blogs.Find(id);
            return blog.Banner != null ? File(blog.Banner, "image/png") : null;
        }

        private ShowAllBlogsViewModel PrepareViewModel(List<Blog> blogs, string viewOrEdit, int? page)
        {
            int pageNumber = page ?? 1;

            var blogModel = Mapper.Map<List<BlogListViewModel>>(blogs);
            var users = _context.Users.ToList();
            blogModel.ForEach(it => it.Owner = users.First(b => b.Id == it.BlogOwnerId).Email);

            var pageBlogs = blogModel.ToPagedList(pageNumber, Const.PageSize);

            var viewModel = new ShowAllBlogsViewModel
            {
                BlogList = pageBlogs,
                ViewOrEdit = viewOrEdit
            };
            return viewModel;
        }

        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes;
            if (image is null || image.ContentLength == 0)
            {
                return null;
            }

            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }

        private string GetValueFromEnum(string enumValue)
        {
            return int.TryParse(enumValue, out int status) ? ((StatusEnum)status).ToString() : StatusEnum.Draft.ToString();
        }
    }
}