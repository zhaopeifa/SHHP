using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    public class ApiScoringCriteriaClassifyContracts
    {
        public string SClassifyId { get; set; }

        public string SClassifyName { get; set; }

        public int Score { get; set; }

        /// <summary>
        /// 扣分值
        /// </summary>
        public int DeductScore { get; set; }

        /// <summary>
        /// 得分值
        /// </summary>
        public int GoalScore { get; set; }

        public ApiScoringCriteriaNormContracts[] SNorms { get; set; }
    }
}