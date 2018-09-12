using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Data.Extensions
{
    public class LinqSQLExtensions : IDisposable
    {
        private NFineDbContext dbcontext = new NFineDbContext();

        /// <summary>
        /// 获取数据库数据上下文
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> IQueryable<TEntity>() where TEntity : class
        {
            return dbcontext.Set<TEntity>();
        }

        public void Dispose()
        {
            this.dbcontext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
