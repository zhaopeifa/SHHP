using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Contracts
{
    /// <summary>
    /// 完善非定点基础信息接收model
    /// </summary>
    public class PerfectFixedFormPointContracts
    {
        public string TaskEntryId { get; set; }

        public int EntryType { get; set; }

        public string Address { get; set; }

        public string CartId { get; set; }
    }
}
