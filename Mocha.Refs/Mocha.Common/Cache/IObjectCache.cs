using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Cache
{
    public interface IObjectCache<T>
    {
        bool Exists(string key);
        T Get(string key);
        void Add(string key, T data, int cacheMinutes);
        void Remove(string key);
    }
}
