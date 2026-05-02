using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.MinutesAndDecisions;
using Microsoft.AspNetCore.Mvc;
using Skote.Models.Charity.MinutesAndDecisions;
using System.Security.Claims;

namespace Skote.Controllers
{
    public class BoardDecisionFollowUpsController : Controller
    {
        private readonly IBoardDecisionFollowUpRepository _followUpRepository;
        private readonly IBoardDecisionRepository _decisionRepository;

        public BoardDecisionFollowUpsController(
            IBoardDecisionFollowUpRepository followUpRepository,
            IBoardDecisionRepository decisionRepository)
        {
            _followUpRepository = followUpRepository;
            _decisionRepository = decisionRepository;
        }

        public async Task<IActionResult> Index(Guid boardDecisionId)
        {
            ViewBag.Decision = await _decisionRepository.GetByIdAsync(boardDecisionId);
            ViewBag.BoardDecisionId = boardDecisionId;
            var items = await _followUpRepository.GetByDecisionIdAsync(boardDecisionId);
            return View(items);
        }

        public async Task<IActionResult> Create(Guid boardDecisionId)
        {
            ViewBag.Decision = await _decisionRepository.GetByIdAsync(boardDecisionId);
            return View(new BoardDecisionFollowUpVm { BoardDecisionId = boardDecisionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardDecisionFollowUpVm vm)
        {
            ViewBag.Decision = await _decisionRepository.GetByIdAsync(vm.BoardDecisionId);

            if (!ModelState.IsValid)
                return View(vm);

            var entity = new BoardDecisionFollowUp
            {
                Id = Guid.NewGuid(),
                BoardDecisionId = vm.BoardDecisionId,
                FollowUpDate = vm.FollowUpDate,
                Status = vm.Status,
                ProgressPercent = vm.ProgressPercent,
                Details = vm.Details,
                NextAction = vm.NextAction,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await _followUpRepository.AddAsync(entity);

            var decision = await _decisionRepository.GetByIdAsync(vm.BoardDecisionId);
            if (decision != null)
            {
                if (vm.ProgressPercent.HasValue && vm.ProgressPercent.Value >= 100)
                    decision.Status = "Completed";
                else if (!string.IsNullOrWhiteSpace(vm.Status))
                    decision.Status = vm.Status;

                await _decisionRepository.UpdateAsync(decision);
            }

            TempData["Success"] = "تم حفظ المتابعة بنجاح.";
            return RedirectToAction(nameof(Index), new { boardDecisionId = vm.BoardDecisionId });
        }
    }
}
