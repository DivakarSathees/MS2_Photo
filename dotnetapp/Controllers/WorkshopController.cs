using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Controllers
{
    public class WorkshopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AvailableWorkshops()
        {
            Console.WriteLine("display");
            var workshops = await _context.Workshops.Include(a => a.Participants).ToListAsync();
            return View(workshops);
        }

        public async Task<IActionResult> BookedWorkshops()
        {
            var workshops = await _context.Workshops
                .Include(a => a.Participants)
                .Where(w => w.Participants.Count > 0)
                .ToListAsync();
            return View(workshops);
        }
    }
}