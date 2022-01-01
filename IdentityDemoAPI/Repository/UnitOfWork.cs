using IdentityDemoAPI.Data;
using IdentityDemoAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
        }
        public IBookRepository BookRepository => new BookRepository(context);
        public IOrderRepository OrderRepository => new OrderRepository(context);
        public IUserRepository UserRepository => new UserRepository(context);
        public IRefreshTokenRepository RefreshTokenRepository => new RefreshTokenRepository(context);
        public async Task<bool> Complete()
        {
            return (await context.SaveChangesAsync()) > 0;
        }
        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
