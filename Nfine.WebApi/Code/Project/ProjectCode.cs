using Nfine.WebApi.Contracts;
using NFine.Application.SystemManage;
using NFine.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nfine.WebApi.Code.Project
{
    public class ProjectCode : IProject
    {
        private ProfileProjectApp ProfileCountyApp = new ProfileProjectApp();

        public List<ApiProjectContracts> GetProject(string countyId = "")
        {
            StringBuilder sqlStr = new StringBuilder();

            sqlStr.Append("SELECT * FROM ProfileProject WHERE 1=1 ");

            if (!string.IsNullOrEmpty(countyId))
            {
                sqlStr.Append(" and CountyId='" + countyId + "'");
            }
            sqlStr.Append(" ORDER BY CountyId DESC,F_CreatorTime");

            using (var db = new NFine.Data.Extensions.LinqSQLExtensions())
            {
                var model =db.IQueryable<NFine.Domain.Entity.SystemManage.ProfileProjectEntity>().Select(d => new ApiProjectContracts()
                {
                    Id = d.F_Id,
                    CountyId = d.CountyId,
                    Name = d.ProjectName
                });
            }


            var data =ProfileCountyApp.FildSql(sqlStr.ToString()).Select(d => new ApiProjectContracts()
            {
                Id = d.F_Id,
                CountyId = d.CountyId,
                Name = d.ProjectName
            }).ToList();
            //排序
            return data;
        }
    }
}