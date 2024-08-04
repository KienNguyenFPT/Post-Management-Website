using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment_ASM3.Models;
using System.Diagnostics;
using System.Security.Claims;
using static NuGet.Packaging.PackagingConstants;

namespace SignalRAssignment_ASM3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PostsManagementContext _context;

        public HomeController(ILogger<HomeController> logger, PostsManagementContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index(int pageNumber = 1, int pageSize = 10, string searchString = null)
        {
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            var isStaff = User.IsInRole("Staff");

            IQueryable<Post> posts = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Include(p => p.User)
                             .Include(p => p.PostCategory)
                             .Where(p =>
                                 p.PostId.ToString().Contains(searchString) ||
                                 p.Title.Contains(searchString) ||
                                 p.Content.Contains(searchString));
            }
            else
            {
                posts = posts.Include(p => p.User)
                             .Include(p => p.PostCategory);
            }

            posts = posts.Where(p => p.UserId.ToString() == userId || p.PublishStatus.Equals("0"));

            posts = posts.OrderByDescending(p => p.CreateDate)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            var viewModel = new PaginatedList<Post>(posts.ToList(), posts.Count(), pageNumber, pageSize);

            return View(viewModel);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
