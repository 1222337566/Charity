using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastructureManagmentServices.Charity.ProjectProposals
{
    public interface IProjectProposalConversionService
    {
        Task<CharityProject> ConvertToProjectAsync(ProjectProposal proposal, string? approvedByUserId = null);
    }
}
