using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Application.Abstractions.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit(CancellationToken cancellationToken);
    UnitResult<Error> Rollback(CancellationToken cancellationToken);
}