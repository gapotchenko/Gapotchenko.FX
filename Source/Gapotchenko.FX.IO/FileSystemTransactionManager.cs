#if TFF_TRANSACTIONS

using System;
using System.Collections.Generic;
using System.IO;
using System.Transactions;

#nullable enable

namespace Gapotchenko.FX.IO
{
    static class FileSystemTransactionManager
    {
        sealed class FileEnlistment : IEnlistmentNotification
        {
            public FileEnlistment(string filePath, string transactionKey)
            {
                m_FilePath = filePath;
                m_TransactionKey = transactionKey;

                if (File.Exists(filePath))
                {
                    m_TempFilePath = Path.GetTempFileName();
                    File.Copy(filePath, m_TempFilePath, true);
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
                    {
                        lock (m_TransactionEnlistedFiles)
                        {
                            if (m_TransactionEnlistedFiles.TryGetValue(m_TransactionKey, out var enlistedFiles))
                            {
                                enlistedFiles.Remove(m_FilePath);
                                if (enlistedFiles.Count == 0)
                                    m_TransactionEnlistedFiles.Remove(m_TransactionKey);
                            }
                        }
                    }

                    m_FilePath = null;
                }

                m_TransactionKey = null;
            }

            public void Commit(Enlistment enlistment)
            {
                Forget();
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                preparingEnlistment.Prepared();
            }

            public void Rollback(Enlistment enlistment)
            {
                if (m_FilePath != null)
                {
                    FileSystem.WaitForFileWriteAccess(m_FilePath);

                    if (m_TempFilePath != null)
                        File.Copy(m_TempFilePath, m_FilePath, true);
                    else
                        File.Delete(m_FilePath);
                }

                Forget();

                enlistment.Done();
            }
        }

        static Dictionary<string, HashSet<string>> m_TransactionEnlistedFiles = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        public static void EnlistFileInTransaction(string path, Transaction transaction)
        {
            path = Path.GetFullPath(path);

            string transactionKey = transaction.TransactionInformation.LocalIdentifier;

            lock (m_TransactionEnlistedFiles)
            {
                if (!m_TransactionEnlistedFiles.TryGetValue(transactionKey, out var enlistedFiles))
                {
                    enlistedFiles = new HashSet<string>(FileSystem.PathComparer);
                    m_TransactionEnlistedFiles.Add(transactionKey, enlistedFiles);
                }

                if (!enlistedFiles.Add(path))
                    return;
            }

            transaction.EnlistVolatile(
                new FileEnlistment(path, transactionKey),
                EnlistmentOptions.None);
        }
    }
}

#endif
