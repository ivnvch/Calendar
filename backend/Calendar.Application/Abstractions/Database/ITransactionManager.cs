using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Application.Abstractions.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransaction(CancellationToken cancellationToken);
    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}