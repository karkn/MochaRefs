using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Store
{
    public interface IObjectStore<T> where T: class
    {
        Task<T> Read(string id);
        Task Update(string id);
    }
}
