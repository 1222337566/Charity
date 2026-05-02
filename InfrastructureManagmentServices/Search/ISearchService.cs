using System;
using System.Collections.Generic;
using InfrastructureManagmentWebFramework.DTOs.Search;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public interface ISearchService
    {
        Task<SearchResult> SearchAsync(string query, int take, CancellationToken ct);
    }
}
