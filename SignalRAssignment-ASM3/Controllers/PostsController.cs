using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment_ASM3.Hubs;
using SignalRAssignment_ASM3.Models;

namespace SignalRAssignment_ASM3.Controllers
{
    public class PostsController : Controller
    {
        private readonly PostsManagementContext _context;
        private readonly IHubContext<PostHub<Post>> _hubContext;

        public PostsController(PostsManagementContext context, IHubContext<PostHub<Post>> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Posts
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

            if (!isStaff)
            {
                posts = posts.Where(p => p.UserId.ToString() == userId);
            }
            else
            {
                posts = posts.Where(p => p.UserId.ToString() == userId || p.PublishStatus.Equals("0"));
            }

            posts = posts.OrderByDescending(p => p.CreateDate)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            var viewModel = new PaginatedList<Post>(posts.ToList(), posts.Count(), pageNumber, pageSize);

            return View(viewModel);
        }



        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            var posts = _context.Posts.Include(p => p.User).Include(po => po.PostCategory).OrderByDescending(x => x.UpdateDate).ToList();
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoryId", "CategoryName");
            return View(posts);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody][Bind("Title,Content,PublishStatus,PostCategoryCategoryId,CategoryName")] Post post)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserEmail");
            ModelState.Remove("PostCategory");
            ModelState.Remove("CategoryName");
            if (ModelState.IsValid)
            {
                post.UserId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
                post.CreateDate = DateTime.Now;
                post.UpdateDate = DateTime.Now;
                _context.Add(post);
                try
                {
                    await _context.SaveChangesAsync();

                }catch(Exception e)
                {
                    throw new Exception(e.Message);
                }
                post.UserEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                await _hubContext.Clients.All.SendAsync("ReceivePostUpdate", post);
                return Ok();
            }
            return NotFound();
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            await _hubContext.Clients.All.SendAsync("ReceivePostUpdate", post);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,UserId,CreateDate,UpdateDate,Title,Content,PublishStatus,CategoryId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("UserEmail");
            ModelState.Remove("PostCategory");
            ModelState.Remove("CategoryName");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        public IActionResult Report(DateTime startDate, DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            var isStaff = User.IsInRole("Staff");

            var posts = _context.Posts
                                .Where(p => p.UpdateDate >= startDate && p.UpdateDate <= endDate)
                                .OrderByDescending(p => p.UpdateDate)
                                .Include(p => p.User)
                                .Include(p => p.PostCategory)
                                .AsQueryable();

            if (!isStaff)
            {
                posts = posts.Where(p => p.UserId.ToString() == userId);
            }
            else
            {
                posts = posts.Where(p => p.UserId.ToString() == userId || p.PublishStatus.Equals("0"));
            }

            var postList = posts.ToList();
            var postCount = postList.Count;

            var userPostCounts = _context.Posts
                                         .Where(p => p.UpdateDate >= startDate && p.UpdateDate <= endDate && p.PublishStatus.Equals("0"))
                                         .GroupBy(p => p.User)
                                         .Select(g => new
                                         {
                                             User = g.Key,
                                             PostCount = g.Count()
                                         })
                                         .OrderByDescending(x => x.PostCount)
                                         .FirstOrDefault();

            var topUser = userPostCounts?.User;
            var topUserPostCount = userPostCounts?.PostCount ?? 0;

            var reportViewModel = new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                PostCount = postCount,
                Posts = postList,
                TopUser = topUser,
                TopUserPostCount = topUserPostCount
            };

            return View(reportViewModel);
        }


    }
}
