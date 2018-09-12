﻿using Nfine.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfine.WebApi.Code.ScoringCriteria
{
    /// <summary>
    /// 评分标准
    /// </summary>
    public interface IScoringCriteria
    {
        ApiScoringCriteriaClassifyContracts[] GetScoringCriteria(string typeId);

        ApiScoringCriteriaClassifyContracts[] GetScoringCriteriaAndRecord(string taskEntryId, string typeId);
    }
}
