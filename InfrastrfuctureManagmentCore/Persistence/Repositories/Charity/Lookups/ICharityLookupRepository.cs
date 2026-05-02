using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastrfuctureManagmentCore.Domains.Payments;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Lookups
{
    public interface ICharityLookupRepository
    {
        Task<List<AidTypeLookup>> GetAidTypesAsync();
        Task<List<PaymentMethod>> GetActivePaymentMethodsAsync();
        Task<List<FinancialAccount>> GetActivePostingFinancialAccountsAsync();
    }
}
