using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;

namespace Calendar.Application.CQRS;

public interface IQueryHandler<TResponse, in TQuery> where TQuery : IQuery
{
    Task<Result<TResponse, Errors>> Handle(TQuery query, CancellationToken cancellationToken);
}

/*public interface IQueryHandlerList<TResponse, in TQuery> where TQuery : IQuery
{
    Task<Result<IReadOnlyList<TResponse>, Errors>> Handle(TQuery query, CancellationToken cancellationToken);
}*/