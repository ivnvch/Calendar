using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Application.Abstractions.Database;

public interface ITransactionScope : IDisposable
{
    Task<UnitResult<Error>> CommitAsync(CancellationToken cancellationToken);
    Task<UnitResult<Error>> RollbackAsync(CancellationToken cancellationToken);
}