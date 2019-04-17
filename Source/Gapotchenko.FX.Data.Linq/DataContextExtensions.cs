using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Linq
{
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of objects returned by the query.</returns>
        public static async Task<IEnumerable<TResult>> ExecuteQueryAsync<TResult>(
            this DataContext dataContext,
            IQueryable<TResult> query,
            CancellationToken cancellationToken = default)
        {
            if (dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var command = dataContext.GetCommand(query);

            DbDataReader reader;
            if (command is SqlCommand sqlCommand)
            {
                // Microsoft SQL Server data provider natively supports async operations.

                var connection = sqlCommand.Connection;
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                reader = await sqlCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                // Use TaskBridge-based async polyfill for other providers.

                reader = await
                    TaskBridge.ExecuteAsync(
                        () =>
                        {
                            var connection = command.Connection;
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();

                            return command.ExecuteReader();
                        },
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            return dataContext.Translate<TResult>(reader);
        }

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

            // Using TaskBridge-based async polyfill because there is no better way.

            return TaskBridge.ExecuteAsync(
                () => dataContext.SubmitChanges(failureMode),
                cancellationToken);
        }

        /// <summary>
        /// Computes the set of modified objects to be inserted, updated, or deleted,
        /// and asynchronously executes the appropriate commands to apply the changes to the database.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task.</returns>
        public static Task SubmitChangesAsync(this DataContext dataContext, CancellationToken cancellationToken) =>
            SubmitChangesAsync(dataContext, ConflictMode.FailOnFirstConflict, cancellationToken);

        /// <summary>
        /// Computes the set of modified objects to be inserted, updated, or deleted,
        /// and asynchronously executes the appropriate commands to apply the changes to the database.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="failureMode">The action to be taken if the submission fails.</param>
        /// <returns>The task.</returns>
        public static Task SubmitChangesAsync(this DataContext dataContext, ConflictMode failureMode) =>
            SubmitChangesAsync(dataContext, failureMode, CancellationToken.None);

        /// <summary>
        /// Computes the set of modified objects to be inserted, updated, or deleted,
        /// and asynchronously executes the appropriate commands to apply the changes to the database.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <returns>The task.</returns>
        public static Task SubmitChangesAsync(this DataContext dataContext) => SubmitChangesAsync(dataContext, CancellationToken.None);
    }
}
