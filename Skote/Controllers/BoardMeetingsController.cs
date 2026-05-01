using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using Microsoft.AspNetCore.Mvc;
using Skote.Models.Charity.MinutesAndDecisions;

namespace Skote.Controllers
{
    public class BoardMeetingsController : Controller
    {
        private readonly IBoardMeetingRepository _repository;

        public BoardMeetingsController(IBoardMeetingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var meetings = await _repository.GetAllAsync();
            return View(meetings);
        }

        public IActionResult Create()
        {
            var vm = new BoardMeetingVm();
            vm.Attendees.Add(new BoardMeetingAttendeeVm());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardMeetingVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (await _repository.MeetingNumberExistsAsync(vm.MeetingNumber))
            {
                ModelState.AddModelError(nameof(vm.MeetingNumber), "رقم الاجتماع مستخدم من قبل.");
                return View(vm);
            }

            var entity = MapToEntity(vm, new BoardMeeting());
            await _repository.AddAsync(entity);
            TempData["Success"] = "تم حفظ الاجتماع بنجاح.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();

            var vm = MapToVm(entity);
            if (vm.Attendees.Count == 0)
                vm.Attendees.Add(new BoardMeetingAttendeeVm());

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BoardMeetingVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();

            if (await _repository.MeetingNumberExistsAsync(vm.MeetingNumber, id))
            {
                ModelState.AddModelError(nameof(vm.MeetingNumber), "رقم الاجتماع مستخدم من قبل.");
                return View(vm);
            }

            entity = MapToEntity(vm, entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);

            TempData["Success"] = "تم تحديث الاجتماع بنجاح.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        public async Task<IActionResult> Print(Guid id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        private static BoardMeeting MapToEntity(BoardMeetingVm vm, BoardMeeting entity)
        {
            entity.MeetingNumber = vm.MeetingNumber?.Trim() ?? string.Empty;
            entity.Title = vm.Title?.Trim() ?? string.Empty;
            entity.MeetingDate = vm.MeetingDate;
            entity.StartTime = vm.StartTime;
            entity.EndTime = vm.EndTime;
            entity.Location = vm.Location?.Trim();
            entity.MeetingType = vm.MeetingType?.Trim();
            entity.Status = vm.Status?.Trim() ?? "Draft";
            entity.Agenda = vm.Agenda;
            entity.Notes = vm.Notes;

            if (entity.Minute == null)
                entity.Minute = new BoardMeetingMinute { Id = Guid.NewGuid(), BoardMeetingId = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id };

            entity.Minute.LegalOpeningText = vm.LegalOpeningText;
            entity.Minute.DiscussionSummary = vm.DiscussionSummary;
            entity.Minute.RecommendationsSummary = vm.RecommendationsSummary;
            entity.Minute.LegalClosingText = vm.LegalClosingText;
            entity.Minute.FullMinuteText = vm.FullMinuteText;
            entity.Minute.UpdatedAtUtc = DateTime.UtcNow;

            entity.Attendees.Clear();
            foreach (var item in vm.Attendees.Where(x => !string.IsNullOrWhiteSpace(x.FullName)))
            {
                entity.Attendees.Add(new BoardMeetingAttendee
                {
                    Id = item.Id ?? Guid.NewGuid(),
                    FullName = item.FullName.Trim(),
                    PositionTitle = item.PositionTitle?.Trim(),
                    AttendanceStatus = string.IsNullOrWhiteSpace(item.AttendanceStatus) ? "Present" : item.AttendanceStatus,
                    Notes = item.Notes
                });
            }

            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                if (entity.Minute != null)
                    entity.Minute.BoardMeetingId = entity.Id;
            }

            return entity;
        }

        private static BoardMeetingVm MapToVm(BoardMeeting entity)
        {
            return new BoardMeetingVm
            {
                Id = entity.Id,
                MeetingNumber = entity.MeetingNumber,
                Title = entity.Title,
                MeetingDate = entity.MeetingDate,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Location = entity.Location,
                MeetingType = entity.MeetingType,
                Status = entity.Status,
                Agenda = entity.Agenda,
                Notes = entity.Notes,
                LegalOpeningText = entity.Minute?.LegalOpeningText,
                DiscussionSummary = entity.Minute?.DiscussionSummary,
                RecommendationsSummary = entity.Minute?.RecommendationsSummary,
                LegalClosingText = entity.Minute?.LegalClosingText,
                FullMinuteText = entity.Minute?.FullMinuteText,
                Attendees = entity.Attendees
                    .Select(a => new BoardMeetingAttendeeVm
                    {
                        Id = a.Id,
                        FullName = a.FullName,
                        PositionTitle = a.PositionTitle,
                        AttendanceStatus = a.AttendanceStatus,
                        Notes = a.Notes
                    }).ToList()
            };
        }
    }
}
