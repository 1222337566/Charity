using Azure.Core;
using InfrastructureManagmentCore.Domains.Jobs;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentCore.Domains.NewFolder1;
using InfrastructureManagmentCore.Domains.Requests;
using InfrastructureManagmentDataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess
{
    public interface IUnitofWork : IDisposable
    {
        IBaseRepository<BackgroundJob> BackgroundjobRepository { get; }
        IBaseRepository<Requests> RequestRepository { get; }
        IBaseRepository<Error> ErrorRepository { get; }
        IBaseRepository<Balance> BalanceRepository { get; }
        IMessageRepository MessageRepository { get; }

        ISenderRepository SenderRepository { get; }
        IReceiverRepository ReceiverRepository { get; }

        ICategoryRepository CategoryRepository { get; }
        int Complete();
        Task SaveChangesAsync(CancellationToken ct);



    }
}
