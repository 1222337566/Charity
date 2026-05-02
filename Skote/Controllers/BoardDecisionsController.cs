using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skote.Models.Charity.MinutesAndDecisions;
using System.Linq;

namespace Skote.Controllers
{
    public class BoardDecisionsController : Controller
    {
        private readonly IBoardDecisionRepository _decisionRepository;
        private readonly IBoardMeetingRepository _meetingRepository;

        public BoardDecisionsController(
            IBoardDecisionRepository decisionRepository,
            IBoardMeetingRepository meetingRepository)
        {
            _decisionRepository = decisionRepository;
            _meetingRepository = meetingRepository;
        }

        public async Task<IActionResult> Index(Guid meetingId)
        {
            ViewBag.MeetingId = meetingId;
            ViewBag.Meeting = await _meetingRepository.GetByIdAsync(meetingId);
            var items = await _decisionRepository.GetByMeetingIdAsync(meetingId);
            return View(items);
        }

        public async Task<IActionResult> Create(Guid boardMeetingId)
        {
            ViewBag.Meeting = await _meetingRepository.GetByIdAsync(boardMeetingId);
            return View(new BoardDecisionVm { BoardMeetingId = boardMeetingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardDecisionVm vm)
        {
            ViewBag.Meeting = await _meetingRepository.GetByIdAsync(vm.BoardMeetingId);

            if (!ModelState.IsValid)
                return View(vm);

            if (await _decisionRepository.DecisionNumberExistsAsync(vm.DecisionNumber))
            {
                ModelState.AddModelError(nameof(vm.DecisionNumber), "رقم القرار مستخدم من قبل.");
                return View(vm);
            }

            var entity = new BoardDecision
            {
                Id = Guid.NewGuid(),
                BoardMeetingId = vm.BoardMeetingId,
                DecisionNumber = vm.DecisionNumber.Trim(),
                Title = vm.Title.Trim(),
                DecisionKind = vm.DecisionKind,
                Description = vm.Description,
                ResponsibleParty = vm.ResponsibleParty,
                DueDate = vm.DueDate,
                Status = vm.Status,
                Priority = vm.Priority,
                RelatedEntityType = vm.RelatedEntityType,
                RelatedEntityId = vm.RelatedEntityId,
                Notes = vm.Notes
            };

            await _decisionRepository.AddAsync(entity);
            TempData["Success"] = "تم حفظ القرار بنجاح.";
            return RedirectToAction(nameof(Index), new { meetingId = vm.BoardMeetingId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _decisionRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            ViewBag.Meeting = await _meetingRepository.GetByIdAsync(entity.BoardMeetingId);
            return View(new BoardDecisionVm
            {
                Id = entity.Id,
                BoardMeetingId = entity.BoardMeetingId,
                DecisionNumber = entity.DecisionNumber,
                Title = entity.Title,
                DecisionKind = entity.DecisionKind,
                Description = entity.Description,
                ResponsibleParty = entity.ResponsibleParty,
                DueDate = entity.DueDate,
                Status = entity.Status,
                Priority = entity.Priority,
                RelatedEntityType = entity.RelatedEntityType,
                RelatedEntityId = entity.RelatedEntityId,
                Notes = entity.Notes
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BoardDecisionVm vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Meeting = await _meetingRepository.GetByIdAsync(vm.BoardMeetingId);
                return View(vm);
            }

            var entity = await _decisionRepository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (await _decisionRepository.DecisionNumberExistsAsync(vm.DecisionNumber, id))
            {
                ModelState.AddModelError(nameof(vm.DecisionNumber), "رقم القرار مستخدم من قبل.");
                ViewBag.Meeting = await _meetingRepository.GetByIdAsync(vm.BoardMeetingId);
                return View(vm);
            }

            entity.DecisionNumber = vm.DecisionNumber.Trim();
            entity.Title = vm.Title.Trim();
            entity.DecisionKind = vm.DecisionKind;
            entity.Description = vm.Description;
            entity.ResponsibleParty = vm.ResponsibleParty;
            entity.DueDate = vm.DueDate;
            entity.Status = vm.Status;
            entity.Priority = vm.Priority;
            entity.RelatedEntityType = vm.RelatedEntityType;
            entity.RelatedEntityId = vm.RelatedEntityId;
            entity.Notes = vm.Notes;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _decisionRepository.UpdateAsync(entity);
            TempData["Success"] = "تم تحديث القرار بنجاح.";
            return RedirectToAction(nameof(Index), new { meetingId = entity.BoardMeetingId });
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var entity = await _decisionRepository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }
    // ── البحث الذكي في القرارات ──
    [HttpGet]
    public async Task<IActionResult> Search(string? q, CancellationToken ct)
    {
        var query =_decisionRepository.GetAll().Result 
            .AsNoTracking()
            .Include(x => x.BoardMeeting)
            .Where(x=> true); // كل القرارات

        if (!string.IsNullOrWhiteSpace(q))
        {
            var qLower = q.Trim();
            query = query.Where(x =>
                x.Title.Contains(qLower)           ||
                (x.Description    != null && x.Description.Contains(qLower))    ||
                (x.ResponsibleParty != null && x.ResponsibleParty.Contains(qLower)) ||
                (x.Notes          != null && x.Notes.Contains(qLower))          ||
                x.DecisionNumber.Contains(qLower)  ||
                (x.BoardMeeting != null && x.BoardMeeting.Title.Contains(qLower)) ||
                (x.BoardMeeting != null && x.BoardMeeting.MeetingNumber.Contains(qLower))
            );
        }

        var results = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .ToListAsync(ct);

        ViewBag.Query      = q;
        ViewBag.TotalCount = results.Count();
        return View(results);
    }
    }
}