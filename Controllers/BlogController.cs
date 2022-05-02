using DataAccess.Applications;
using DataAccess.Models;
using DataAccess.Models.ExtraModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace berkucdag.com.Controllers
{
    public class BlogController : Controller
    {
        BlogRepository blogRepository = new BlogRepository(new DBContext());

        public IActionResult Index()
        {
            var index = Convert.ToInt32(Request.Query["pageIndex"]);
            if (index == 0)
            {
                index = 1;
            }

            var homePageModel = new HomePageModel();
            homePageModel.HomeOneCikan = blogRepository.OneCikanlar();
            homePageModel.Categories = blogRepository.GetCategories();
            homePageModel.PageSize = blogRepository.GetPageCount();
            homePageModel.Blogs = blogRepository.GetBlogsByPageIndex(index);
            homePageModel.BlogLast = blogRepository.GetBlogsLast();
            homePageModel.PageIndex = index;            

            return View(homePageModel);
        }

        public IActionResult BlogByCategory(int categoryId)
        {
            var index = Convert.ToInt32(Request.Query["pageIndex"]);
            if (index == 0)
            {
                index = 1;
            }
            var categoryPageModel = new CategoryPageModel();
            categoryPageModel.Categories = blogRepository.GetCategories();
            categoryPageModel.BlogLast = blogRepository.GetBlogsLast();
            categoryPageModel.Blogs = blogRepository.GetBlogsCategoryByPageIndex(categoryId, index);
            categoryPageModel.PageSize = blogRepository.GetPageCountByCategory(categoryId);
            categoryPageModel.Category = blogRepository.GetCategoryById(categoryId);
            categoryPageModel.PageIndex = index;
            categoryPageModel.CategoryId = categoryId;
            return View(categoryPageModel);
        }

        [HttpPost]
        public IActionResult AddComment([FromForm] AddCommentModel addCommentModel)
        {
            
            bool isAuthenticated = User.Identity.IsAuthenticated;
            if (isAuthenticated == true)
            {
                var result = blogRepository.AddComment(addCommentModel);

                if (result != null)
                {
                    return Json(new { status = true, message = "Yorum Eklendi!!!" });
                }
                else
                {
                    return Json(new { status = false, message = "Bir Hata Oluştu!!!" });
                }
            }
            else
            {
                return Json(new { status = false, message = "Yorum yapmadan önce giriş yapmalısınız!!!" });
            }
        }

        public IActionResult BlogDetail(int blogId)
        {
            var blogDetailModel = new BlogDetailModel();
            blogDetailModel.GetBlog = blogRepository.GetBlogById(blogId);
            blogDetailModel.Categories = blogRepository.GetCategories();
            blogDetailModel.BlogLast = blogRepository.GetBlogsLast();
            bool isAuthenticated = User.Identity.IsAuthenticated;
            if (isAuthenticated == true)
            {
                blogDetailModel.UserId = blogRepository.GetUserByEmail(User.Identity.Name).Id;
            }
                return View(blogDetailModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {

            var userKontrol = blogRepository.GetUserByEmail(user.Email);

            if (userKontrol == null)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] dizi = Encoding.UTF8.GetBytes(user.Password);
                dizi = md5.ComputeHash(dizi);
                StringBuilder sb = new StringBuilder();
                foreach (byte ba in dizi)
                {
                    sb.Append(ba.ToString("x2").ToLower());
                }

                user.Password = sb.ToString();

                blogRepository.AddUser(user);
                return Json(new { status = true, message = "Kayıt Başarılı!!!" });
            }
            else
            {
                return Json(new { status = false, message = "Bu Mail Adresiyle Kayıt Mevcut!!!" });
            }

        }

        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email,string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] dizi = Encoding.UTF8.GetBytes(password);
            dizi = md5.ComputeHash(dizi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            var result = blogRepository.GetUser(email, sb.ToString());
            if (result != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,email)
                };
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Blog");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Blog");
        }

    }
}
