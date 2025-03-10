#if TFF_TRANSACTIONS

using Gapotchenko.FX.IO.Properties;
using System.Transactions;

namespace Gapotchenko.FX.IO;

static class FileSystemTransactionManager
{
    #region Files

    sealed class FileEnlistment : IEnlistmentNotification
    {
        public FileEnlistment(string filePath, string transactionKey)
        {
            filePath = Path.GetFullPath(filePath);

            m_FilePath = filePath;
            m_TransactionKey = transactionKey;

            if (File.Exists(filePath))
            {
                m_TempFilePath = Path.GetTempFileName();
                File.Copy(filePath, m_TempFilePath, true);
            }
            else if (Directory.Exists(filePath))
            {
                throw new IOException(string.Format(Resources.PathPointsToDirectoryNotFileTX, filePath));
            }
        }

        string? m_FilePath;
        string? m_TransactionKey;
        string? m_TempFilePath;

        void Forget()
        {
            if (m_TempFilePath != null)
            {
                try
                {
                    File.Delete(m_TempFilePath);
                }
                catch
                {
                }

                m_TempFilePath = null;
            }

            if (m_FilePath != null)
            {
                if (m_TransactionKey != null)
                    m_TransactionEnlistedFiles.Remove(m_TransactionKey, m_FilePath);

                m_FilePath = null;
            }

            m_TransactionKey = null;
        }

        public void Commit(Enlistment enlistment)
        {
            Forget();
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment) => enlistment.Done();

        public void Prepare(PreparingEnlistment preparingEnlistment) => preparingEnlistment.Prepared();

        public void Rollback(Enlistment enlistment)
        {
            if (m_FilePath != null)
            {
                FileSystem.WaitForFileWriteAccess(m_FilePath);

                try
                {
                    if (m_TempFilePath != null)
                        File.Copy(m_TempFilePath, m_FilePath, true);
                    else
                        File.Delete(m_FilePath);
                }
                catch (DirectoryNotFoundException)
                {
                    // Target directory of a file does not exist anymore.
                }
            }

            Forget();

            enlistment.Done();
        }
    }

    static TransactionEnlistedEntries m_TransactionEnlistedFiles = new();

    public static void EnlistFileInTransaction(string path, Transaction transaction)
    {
        path = Path.GetFullPath(path);
        string transactionKey = transaction.TransactionInformation.LocalIdentifier;

        if (!m_TransactionEnlistedFiles.Add(transactionKey, path))
            return;

        transaction.EnlistVolatile(
            new FileEnlistment(path, transactionKey),
            EnlistmentOptions.None);
    }

    #endregion

    #region Directories

    sealed class DirectoryEnlistment : IEnlistmentNotification
    {
        public DirectoryEnlistment(string directoryPath, string transactionKey)
        {
            directoryPath = Path.GetFullPath(directoryPath);

            m_DirectoryPath = directoryPath;
            m_TransactionKey = transactionKey;

            if (Directory.Exists(directoryPath))
            {
                m_DirectoryExists = true;
            }
            else if (File.Exists(directoryPath))
            {
                throw new IOException(string.Format(Resources.PathPointsToFileNotDirectoryTX, directoryPath));
            }
        }

        string? m_DirectoryPath;
        string? m_TransactionKey;
        bool m_DirectoryExists;

        void Forget()
        {
            if (m_DirectoryPath != null)
            {
                if (m_TransactionKey != null)
                    m_TransactionEnlistedDirectories.Remove(m_TransactionKey, m_DirectoryPath);

                m_DirectoryPath = null;
            }

            m_TransactionKey = null;
        }

        public void Commit(Enlistment enlistment)
        {
            Forget();
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment) => enlistment.Done();

        public void Prepare(PreparingEnlistment preparingEnlistment) => preparingEnlistment.Prepared();

        public void Rollback(Enlistment enlistment)
        {
            if (m_DirectoryPath != null)
            {
                if (!m_DirectoryExists)
                {
                    try
                    {
                        Directory.Delete(m_DirectoryPath);
                    }
                    catch (IOException)
                    {
                    }
                }
            }

            Forget();
            enlistment.Done();
        }
    }

    static TransactionEnlistedEntries m_TransactionEnlistedDirectories = new();

    public static void EnlistDirectoryInTransaction(string path, Transaction transaction)
    {
        path = Path.GetFullPath(path);
        string transactionKey = transaction.TransactionInformation.LocalIdentifier;

        if (!m_TransactionEnlistedDirectories.Add(transactionKey, path))
            return;

        transaction.EnlistVolatile(
            new DirectoryEnlistment(path, transactionKey),
            EnlistmentOptions.None);
    }

    #endregion

    readonly struct TransactionEnlistedEntries
    {
        public TransactionEnlistedEntries()
        {
        }

        readonly Dictionary<string, HashSet<string>> m_Entries = new(StringComparer.Ordinal);

        public readonly bool Add(string transactionKey, string entryPath)
        {
            lock (m_Entries)
            {
                if (!m_Entries.TryGetValue(transactionKey, out var enlistedEntries))
                {
                    enlistedEntries = new HashSet<string>(FileSystem.PathComparer);
                    m_Entries.Add(transactionKey, enlistedEntries);
                }

                return enlistedEntries.Add(entryPath);
            }
        }

        public readonly void Remove(string transactionKey, string entryPath)
        {
            lock (m_Entries)
            {
                if (m_Entries.TryGetValue(transactionKey, out var enlistedEntries))
                {
                    if (enlistedEntries.Remove(entryPath) && enlistedEntries.Count == 0)
                        m_Entries.Remove(transactionKey);
                }
            }
        }
    }
}

#endif
