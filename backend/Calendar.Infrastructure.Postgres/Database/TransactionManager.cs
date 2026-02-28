using Calendar.Application.Abstractions.Database;
using Calendar.Domain.Calendars.Errors;
using Calendar.Shared.Errors;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Calendar.Infrastructure.Postgres.Database;

public class TransactionManager : ITransactionManager
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(
        CalendarDbContext context, 
        ILogger<TransactionManager> logger, 
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<Result<ITransactionScope, Error>> BeginTransaction(CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            var transactionScopeLogger = _loggerFactory.CreateLogger<TransactionScope>();
            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), transactionScopeLogger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured during transaction");
            return Error.Failure("database", "Error occured during transaction");
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes");
            return UnitResult.Failure(GeneralErrors.Failure());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            return UnitResult.Failure(GeneralErrors.Failure());
        }
    }
}