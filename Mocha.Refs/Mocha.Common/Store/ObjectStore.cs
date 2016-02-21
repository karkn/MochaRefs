using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mocha.Common.Store
{
    public class ObjectStore<T>: IObjectStore<T> where T: class
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private const int Retry = 10;
        private const int Interval = 200;

        private string _basePath;
        private Func<string, Task<T>> _asyncObjectProvider;

        public ObjectStore(string basePath, Func<string, Task<T>> asyncObjectProvider)
        {
            _basePath = basePath;
            _asyncObjectProvider = asyncObjectProvider;
        }

        public async Task<T> Read(string id)
        {
            var path = GetPath(id);
            if (!File.Exists(path))
            {
                await Update(id);
            }

            /// Write待ち
            WaitForLock(id);

            /// Loadはだめでもリトライしない
            return Load(path);
        }

        public async Task Update(string id)
        {
            var path = GetPath(id);
            var obj = await _asyncObjectProvider(id);

            /// Write待ち
            WaitForLock(id);

            Lock(id);
            try
            {
                /// Read待ちリトライ
                for (var i = 0; i < Retry; ++i)
                {
                    try
                    {
                        Save(obj, id);
                        return;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(Interval);
                        _logger.Warn("Updateをリトライ: " + id);
                    }
                }

                _logger.Error("Update失敗: " + id);
                throw new IOException("Update失敗: " + id);
            }
            catch(Exception)
            {
                /// 最後に削除を試みておく
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception)
                {
                }
                throw;
            }
            finally
            {
                Unlock(id);
            }
        }

        private string GetPath(string id)
        {
            return Path.Combine(_basePath, id.ToString());
        }

        private string GetLockPath(string id)
        {
            return Path.Combine(GetPath(id), ".lock");
        }

        private void Lock(string id)
        {
            for (var i= 0; i < Retry; ++i)
            {
                try
                {
                    var path = GetLockPath(id);
                    if (File.Exists(path))
                    {
                        return;
                    }
                    using (var s = File.Create(path))
                    {
                        s.Close();
                        return;
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(Interval);
                    _logger.Warn("ロックをリトライ: " + id);
                }
            }
            _logger.Error("ロックに失敗: " + id);
            throw new IOException("ロックに失敗: " + id);
        }

        private void Unlock(string id)
        {
            var path = GetLockPath(id);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void WaitForLock(string id)
        {
            for (var i = 0; i < Retry; ++i)
            {
                if (File.Exists(GetLockPath(id)))
                {
                    Thread.Sleep(Interval);
                }
                else
                {
                    return;
                }
            }

            _logger.Error("ロックが解除されませんでした: " + id);
            throw new IOException("ロックが解除されませんでした: " + id);
        }

        private T Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var f = new BinaryFormatter();
                var obj = (T)f.Deserialize(fs);
                fs.Close();
                return obj;
            }
        }

        private void Save(object obj, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
                fs.Close();
            }
        }

    }
}
