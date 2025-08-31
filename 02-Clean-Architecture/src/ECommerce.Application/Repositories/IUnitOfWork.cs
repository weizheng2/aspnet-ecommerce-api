namespace ECommerce.Application.Repositories;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}