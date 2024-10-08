// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Threading.Tasks;
using System.Data;
using System.Data.Linq;

namespace Gapotchenko.FX.Data.Linq;

/// <summary>
/// LINQ to SQL data context extensions.
/// </summary>
public static class DataContextExtensions
{
    /// <summary>
    /// Asynchronously executes the query.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the returned collection.</typeparam>
    /// <param name="dataContext">The data context.</param>
    /// <param name="query">The query.</param>
    /// <returns>A collection of objects returned by the query.</returns>
    public static Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(this DataContext dataContext, IQueryable<TResult> query) =>
        ExecuteQueryAsync(dataContext, query, CancellationToken.None);

    /// <summary>
    /// Asynchronously executes the query.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the returned collection.</typeparam>
    /// <param name="dataContext">The data context.</param>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of objects returned by the query.</returns>
    public static async Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(
        this DataContext dataContext,
        IQueryable<TResult> query,
        CancellationToken cancellationToken)
    {
        if (dataContext == null)
            throw new ArgumentNullException(nameof(dataContext));
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var command = dataContext.GetCommand(query);

        var connection = command.Connection;
        if (connection.State == ConnectionState.Closed)
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        // Execute Translate via TaskBridge because it can potentially use synchronous operations
        // like Read() and NextResult() on a specified reader instance.
        // This allows to avoid possible thread pool pollution on a large number of concurrent operations.
        // In essence, this is a best-effort workaround.
        // There is no better way without modifying the stock LINQ to SQL implementation.
        return await
            TaskBridge.ExecuteAsync(
                () => dataContext.Translate<TResult>(reader),
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc cref="SubmitChangesAsync(DataContext, CancellationToken)"/>
    public static Task SubmitChangesAsync(this DataContext dataContext) =>
        SubmitChangesAsync(dataContext, CancellationToken.None);

    /// <summary>
    /// Computes the set of modified objects to be inserted, updated, or deleted,
    /// and asynchronously executes the appropriate commands to apply the changes to the database.
    /// <inheritdoc cref="DataContext.SubmitChanges()"/>
    /// </summary>
    /// <param name="dataContext">The data context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public static Task SubmitChangesAsync(this DataContext dataContext, CancellationToken cancellationToken) =>
        SubmitChangesAsync(dataContext, ConflictMode.FailOnFirstConflict, cancellationToken);

    /// <inheritdoc cref="SubmitChangesAsync(DataContext, ConflictMode, CancellationToken)"/>
    public static Task SubmitChangesAsync(this DataContext dataContext, ConflictMode failureMode) =>
        SubmitChangesAsync(dataContext, failureMode, CancellationToken.None);

    /// <summary>
    /// Asynchronously sends changes that were made to retrieved objects to the underlying database,
    /// and specifies the action to be taken if the submission fails.
    /// </summary>
    /// <param name="dataContext">The data context.</param>
    /// <param name="failureMode">The action to be taken if the submission fails.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    public static Task SubmitChangesAsync(this DataContext dataContext, ConflictMode failureMode, CancellationToken cancellationToken)
    {
        if (dataContext == null)
            throw new ArgumentNullException(nameof(dataContext));

        // Using TaskBridge-based async polyfill as the best-effort workaround.
        // There is no better way without modifying the stock LINQ to SQL implementation.

        return TaskBridge.ExecuteAsync(
            () => dataContext.SubmitChanges(failureMode),
            cancellationToken);
    }
}
