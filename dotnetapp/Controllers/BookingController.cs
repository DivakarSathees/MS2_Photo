using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult WorkshopEnrollmentForm(int workshopId)
        {
            var workshop =  _context.Workshops.Find(workshopId);
            if (workshop == null)
            {
                return NotFound();
            }

            return View(workshop);
            // return View();
        }

        [HttpPost]
        public IActionResult WorkshopEnrollmentForm(int workshopId, string name, string email)
        {
            var workshop =  _context.Workshops.Include(w => w.Participants)
                                                   .FirstOrDefault(w => w.WorkshopID == workshopId);

            if (workshop == null)
            {
                return NotFound();
            }

            if (workshop.Participants.Count == workshop.Capacity)
            {
                throw new WorkshopBookingException("Maximum Number Reached");
            }

            var participant = new Participant
            {
                Name = name,
                Email = email,
                WorkshopID = workshopId
            };

            _context.Participants.Add(participant);
            // workshop.Participants.Add(participant);
            _context.SaveChanges();

            return RedirectToAction("EnrollmentConfirmation", new { participantId = participant.ParticipantID });
        }

        public IActionResult EnrollmentConfirmation(int participantId)
        {
            var participant = _context.Participants.Include(p => p.Workshop)
                                                         .FirstOrDefault(p => p.ParticipantID == participantId);
            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }
    }
}