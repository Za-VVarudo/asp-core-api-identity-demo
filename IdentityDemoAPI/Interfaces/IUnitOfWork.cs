using IdentityDemoAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Interfaces
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }
        IOrderRepository OrderRepository { get; }
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository {get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
