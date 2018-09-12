using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfine.WebApi.Contracts
{
    /// <summary>
    /// api检查点类型
    /// </summary>
    public class ApiCheckingPointTypeContracts
    {
        public string TypeId{ get; set; }
        public string Name { get; set; }
        public string EntryId { get; set; }

        /// <summary>
        /// 当前存在任务数
        /// </summary>
        public int TaskCount { get; set; }
    }
}
