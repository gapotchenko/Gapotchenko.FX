#if TFF_TRANSACTIONS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Gapotchenko.FX.IO
{
    static class FileSystemTransactionManager
    {
        sealed class FileEnlistment : IEnlistmentNotification
        {
            public FileEnlistment(string filePath, string transactionKey)
            {
                _FilePath = filePath;
                _TransactionKey = transactionKey;

                if (File.Exists(filePath))
                {
                    _TempFilePath = Path.GetTempFileName();
                    File.Copy(filePath, _TempFilePath, true);
                }
            }

            string _FilePath;
            string _TempFilePath;
            string _TransactionKey;

            void _Forget()
            {
                if (_TempFilePath != null)
                {
                    try
                    {
                        File.Delete(_TempFilePath);
                    }
                    catch
                    {
                    }
                    _TempFilePath = null;
                }

                if (_FilePath != null)
                {
                    lock (_EnlistedFiles)
                    {
                        HashSet<string> files;
                        if (_EnlistedFiles.TryGetValue(_TransactionKey, out files))
                        {
                            files.Remove(_FilePath);
                            if (files.Count == 0)
                                _EnlistedFiles.Remove(_TransactionKey);
                        }
                    }
                    _FilePath = null;
                }

                _TransactionKey = null;
            }

            public void Commit(Enlistment enlistment)
            {
                _Forget();
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
                FileSystem.WaitForFileWriteAccess(_FilePath);
                if (_TempFilePath != null)
                    File.Copy(_TempFilePath, _FilePath, true);
                else
                    File.Delete(_FilePath);

                _Forget();

                enlistment.Done();
            }
        }

        static Dictionary<string, HashSet<string>> _EnlistedFiles = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        public static void EnlistFileInTransaction(string path, Transaction transaction)
        {
            path = Path.GetFullPath(path);

            string transactionKey = transaction.TransactionInformation.LocalIdentifier;

            lock (_EnlistedFiles)
            {
                HashSet<string> files;
                if (!_EnlistedFiles.TryGetValue(transactionKey, out files))
                {
                    files = new HashSet<string>(FileSystem.PathComparer);
                    _EnlistedFiles.Add(transactionKey, files);
                }

                if (!files.Add(path))
                    return;
            }

            transaction.EnlistVolatile(
                new FileEnlistment(path, transactionKey),
                EnlistmentOptions.None);
        }
    }
}

#endif
