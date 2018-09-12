using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    public class ApiScoringCriteriaNormContracts
    {
        /// <summary>
        /// 评分明细Id
        /// </summary>
        public string SNormId { get; set; }

        /// <summary>
        /// 评分明细检查项目
        /// </summary>
        public string SNormProjectName { get; set; }

        /// <summary>
        /// 评分明细评分标准
        /// </summary>
        public string SNormStandardName { get; set; }

        /// <summary>
        /// 起扣条件 几处
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        /// 是否扣分项
        /// </summary>
        public bool IsDeduct { get; set; }
    }
}