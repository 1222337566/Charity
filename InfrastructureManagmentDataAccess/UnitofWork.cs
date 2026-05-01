using InfrastructureManagmentCore.Domains.Jobs;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentCore.Domains.NewFolder1;
using InfrastructureManagmentCore.Domains.Requests;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess
{
    public class UnitofWork : IUnitofWork
    {
        private readonly AppDbContext _dbContext;
        
        public UnitofWork(AppDbContext appDBContext,
            IBaseRepository<BackgroundJob> backgroundJobrepository, 
            IBaseRepository<Requests> requestRepository,
            IBaseRepository<Balance> baseRepository
          )
        {
            _dbContext = appDBContext;
            BackgroundjobRepository = backgroundJobrepository;
            RequestRepository = requestRepository;
            MessageRepository = new MessageRepository(appDBContext);
            CategoryRepository = new CategoryRepository(appDBContext);
            SenderRepository = new SenderRepository(appDBContext);
            ReceiverRepository = new ReceiverRepository(appDBContext);
            BalanceRepository = baseRepository;
        }
        public IBaseRepository<BackgroundJob> BackgroundjobRepository { get; private set ;}

        public IBaseRepository<Requests> RequestRepository { get; private set; }
        public IBaseRepository<Balance> BalanceRepository { get; private set; }
        public IBaseRepository<Error> ErrorRepository { get; private set; }

        public IMessageRepository MessageRepository { get; private set; }

       
        public IReceiverRepository ReceiverRepository { get; private set; }

        public ICategoryRepository CategoryRepository { get; private set; }

        public ISenderRepository SenderRepository { get; private set; }

        public int Complete()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
           await _dbContext.SaveChangesAsync(ct);   
        }
    }
}
