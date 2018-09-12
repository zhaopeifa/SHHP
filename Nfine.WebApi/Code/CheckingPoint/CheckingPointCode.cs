using Nfine.WebApi.Contracts;
using Nfine.WebApi.Data.Enums;
using NFine.Application.SystemManage;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Nfine.WebApi.Code.CheckingPoint
{
    public class CheckingPointCode : ICheckingPoint
    {
        private Code.Task.ITask taskCode =new Code.Task.TaskCode();

        private ProfileProjectApp App = new ProfileProjectApp();
        public List<ApiCheckingPointContracts> GetCheckingPoint(string ProjectId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                List<ApiCheckingPointContracts> result = null;

                //根据项目Id获取当前项目
                string projectType = App.FildSql<string>(d => d.F_Id == ProjectId, d => d.ProjectType).FirstOrDefault();

                switch (projectType.ToLower())
                {
                    case "sanitation"://环卫
                        result = db.IQueryable<ProfileScoreCriteria_EntryEntity>().OrderBy(d=>d.SortingCode).Select(d => new ApiCheckingPointContracts()
                        {
                            EntryId = d.SEntryId,
                            ProjectId = ProjectId,
                            Name = d.Name
                        }).ToList();

                        break;
                    case "amenities": //市容
                        break;
                    case "fivechaos": //五乱
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public List<ApiCheckingPointContracts> GetCheckingPointHavTaskCount(string userId, string ProjectId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                List<ApiCheckingPointContracts> result = null;

                //根据项目Id获取当前项目
                string projectType = App.FildSql<string>(d => d.F_Id == ProjectId, d => d.ProjectType).FirstOrDefault();

                switch (projectType.ToLower())
                {
                    case "sanitation"://环卫
                        result = db.IQueryable<ProfileScoreCriteria_EntryEntity>().OrderBy(d=>d.SortingCode).Select(d => new ApiCheckingPointContracts()
                        {
                            EntryId = d.SEntryId,
                            ProjectId = ProjectId,
                            Name = d.Name
                        }).ToList();

                        foreach (var item in result)
                        {
                            item.TaskCount=this.taskCode.GetUndoneTaskCount(userId, item.EntryId);
                        }

                        break;
                    case "amenities": //市容
                        break;
                    case "fivechaos": //五乱
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public List<ApiCheckingPointTypeContracts> GetCheckingPointClassification(string entryId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                return db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.SEntryId == entryId).Select(d => new ApiCheckingPointTypeContracts()
                {
                    TypeId = d.STypeId,
                    Name = d.Name,
                    EntryId = entryId
                }).ToList();
            }

        }


        
    }
}