using System;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Charity.HumanitarianResearch
{
    public interface IHumanitarianResearchWorkflowService
    {
        Task SubmitForReviewAsync(Guid researchId, string submittedByUserId);
        Task ReviewAsync(Guid researchId, string reviewerUserId, string decision, string reason, string? notes = null);
        Task SendToCommitteeAsync(Guid researchId, string userId);
        Task CommitteeDecisionAsync(Guid researchId, DateTime meetingDate, string decision, string? aidType, decimal? approvedAmount, int? durationMonths, string? notes, string? approvedByUserId);
    }
}
