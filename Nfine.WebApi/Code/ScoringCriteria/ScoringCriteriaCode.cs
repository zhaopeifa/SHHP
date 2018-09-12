using Nfine.WebApi.Contracts;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Code.ScoringCriteria
{
    /// <summary>
    /// 评分标准
    /// </summary>
    public class ScoringCriteriaCode : IScoringCriteria
    {
        public Contracts.ApiScoringCriteriaClassifyContracts[] GetScoringCriteria(string typeId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var SClassifys = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().Where(d => d.STypeId == typeId).Select(d => new ApiScoringCriteriaClassifyContracts()
                {
                    SClassifyId = d.SClassifyId,
                    SClassifyName = d.SClassifyName,
                    Score = d.Score
                }).ToArray();
                foreach (var item in SClassifys)
                {
                    item.SNorms = db.IQueryable<ProfileScireCriteria_NormEntity>().Where(d => d.SClassifyId == item.SClassifyId).Select(d => new ApiScoringCriteriaNormContracts()
                    {
                        SNormId = d.SNormId,
                        Condition = d.Condition,
                        SNormProjectName = d.SNormProjectName,
                        SNormStandardName = d.SNormStandardName,
                        IsDeduct = d.IsDeduct
                    }).ToArray();
                }

                return SClassifys;
            }
        }


        public ApiScoringCriteriaClassifyContracts[] GetScoringCriteriaAndRecord(string taskEntryId, string typeId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var SClassifys = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().Where(d => d.STypeId == typeId).Select(d => new ApiScoringCriteriaClassifyContracts()
                {
                    SClassifyId = d.SClassifyId,
                    SClassifyName = d.SClassifyName,
                    Score = d.Score
                }).ToArray();

                foreach (var item in SClassifys)
                {

                    item.SNorms = db.IQueryable<ProfileScireCriteria_NormEntity>().Where(d => d.SClassifyId == item.SClassifyId).Select(d => new ApiScoringCriteriaNormContracts()
                    {
                        SNormId = d.SNormId,
                        Condition = d.Condition,
                        SNormProjectName = d.SNormProjectName,
                        SNormStandardName = d.SNormStandardName,
                        IsDeduct = d.IsDeduct
                    }).ToArray();

                    if (item.SNorms != null)
                    {
                        int deductScore = 0;
                        int doalScore = 0;
                        foreach (var sNormItem in item.SNorms)
                        {
                            var deduinsQuery = db.IQueryable<ProfileDeducInsEntity>().Where(d => d.TaskEntry_Id == taskEntryId && d.SCNorm_Id == sNormItem.SNormId);

                            if (deduinsQuery.Count() <= 0)
                                continue;

                            var deductScoreQuery = deduinsQuery.Where(d => d.SCNormIsDeduct);
                            var goalScoreQuery = deduinsQuery.Where(d => !d.SCNormIsDeduct);

                            if (deductScoreQuery.Count() > 0)
                            {
                                deductScore += deduinsQuery.Where(d => d.SCNormIsDeduct).Sum(d => d.DeductionScore);
                            }
                            if (goalScoreQuery.Count() > 0)
                            {
                                doalScore += deduinsQuery.Where(d => !d.SCNormIsDeduct).Sum(d => d.DeductionScore);
                            }
                        }

                        item.DeductScore = (deductScore - doalScore) > 0 ? (deductScore - doalScore) : 0;
                        item.GoalScore = item.Score - item.DeductScore;
                    }
                }

                return SClassifys;
            }
        }
    }
}