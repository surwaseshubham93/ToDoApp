using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApplication.Models;

namespace ToDoApplication.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext _context;

        public HomeController(ToDoContext context)
        {
            _context = context;
        }
        public IActionResult Index( string id)
        {
            var filters = new Filters(id);
            ViewBag.Filters = filters;

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo> query = _context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status);

            if (filters.HasCategory)
            {
                query = query.Where(t=>t.CategoryId == filters.CategoryId);
            }
            if (filters.HasStatus) 
            {
                query = query.Where(t=>t.StatusId == filters.StatusId);
            }
            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if(filters.IsToday) 
                {
                    query = query.Where(t=>t.DueDate == today);
                }
            }

            var tasks = query.OrderBy(t=> t.DueDate).ToList();

            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            var task = new ToDo { StatusId = "open" };
            return View(task);
        }

        [HttpPost]
        public IActionResult Add(ToDo todo)
        {
            if (ModelState.IsValid) 
            {
                _context.ToDos.Add(todo);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Statuses = _context.Statuses.ToList();
                return View(todo);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join("-", filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
        {
            selected = _context.ToDos.Find(selected.Id)!;

            if(selected != null)
            {
                selected.StatusId = "closed";
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new {Id = id});
        }

        [HttpPost]
        public IActionResult DeleteCompleted(string id)
        {
            var taskcomplete = _context.ToDos.Where(t => t.StatusId == "closed").ToList();
            foreach (var task in taskcomplete)
            {
                _context.ToDos.Remove(task);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", new {ID = id });
        }
    }
}
