using Application.Contracts.IRepo;

namespace Application.Contracts;
public interface IUnitofWork:IDisposable
{

    IUserRepo UserRepo { get; } 
    Task<int> CommitAsync();

}

