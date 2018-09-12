﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfine.WebApi.Contracts
{
    /// <summary>
    /// Api当中使用的检查点Model
    /// </summary>
    public class ApiCheckingPointContracts
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 名称  
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 存在任务数
        /// </summary>
        public int TaskCount { get; set; }
    }
}
