using DiamondShop.Application.Services.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly DiamondShopDbContext _dbContext;
        private IDbContextTransaction? _transaction { get; set; }
        public UnitOfWork(DiamondShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken);
                _transaction = null;
            }
        }

        public async Task RollBackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                _transaction = null;
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
