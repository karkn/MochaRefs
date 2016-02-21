using Mocha.Refs.Core.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Data
{
    public class Repository<TEntity> :
        DbSet<TEntity>, IRepository<TEntity>, IDbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, 
        IQueryable, IEnumerable where TEntity : class
    {
    }
}
