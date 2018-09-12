﻿using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using NFine.Repository.SystemManage;
using NFine.Web.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Application.SystemManage
{
    /// <summary>
    /// 环评-环卫-绿色账户小区
    /// </summary>
    public class ProfileSanitationGreenResidentialApp
    {
        private ProfileSanitationGreenResidentialRepository service = new ProfileSanitationGreenResidentialRepository();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileSanitationGreenResidentialEntity> FildSql(string enCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(enCode);
            return service.FindList(strSql.ToString());
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination">分页，排序参数</param>
        /// <param name="keyword">检索关键字</param>
        /// <returns></returns>
        public List<ProfileSanitationGreenResidentialEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileSanitationGreenResidentialEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_Code = 0;
                int.TryParse(keyword, out intF_Code);

                expression = expression.And(t => t.ResidentialName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_Code);
                expression = expression.Or(t => t.Address.Contains(keyword));
            }

            return service.FindList(expression, pagination);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination">分页，排序参数</param>
        /// <param name="keyword">检索关键字</param>
        /// <param name="projectId">关联项目Id</param>
        /// <returns></returns>
        public List<ProfileSanitationGreenResidentialEntity> GetList(Pagination pagination, string keyword, string projectId, string streetId)
        {
            var expression = ExtLinq.True<ProfileSanitationGreenResidentialEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_Code = 0;
                int.TryParse(keyword, out intF_Code);

                expression = expression.And(t => t.ResidentialName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_Code);
                expression = expression.Or(t => t.Address.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(streetId))
            {
                expression = expression.And(t => t.StreetId == streetId);
            }

            expression = expression.And(t => t.ProjectId == projectId);

            return service.FindList(expression, pagination);
        }

        /// <summary>
        /// 根据id获取单挑数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ProfileSanitationGreenResidentialEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(string keyValue)
        {
            service.Delete(GetForm(keyValue));
            try
            {
                //添加日志
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除绿色账户小区【" + GetForm(keyValue).ResidentialName + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 提交，修改
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ProfileSanitationGreenResidentialEntity Entity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                Entity.Modify(keyValue);

                service.Update(Entity);
            }
            else
            {
                Entity.Create();
                service.Insert(Entity);
            }

            try
            {
                //添加日志
                LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "修改环卫绿色账户小区信息【" + Entity.ResidentialName + "】成功！");
            }
            catch { }


        }

        /// <summary>
        /// 单条
        /// 批量导入
        /// </summary>
        /// <param name="Entity"></param>
        /// <param name="skipWhere"></param>
        /// <param name="coverWhere"></param>
        public void BatchSubmitFrom(ProfileSanitationGreenResidentialEntity Entity, Func<ProfileSanitationGreenResidentialEntity, ProfileSanitationGreenResidentialEntity, bool> skipWhere, Func<ProfileSanitationGreenResidentialEntity, ProfileSanitationGreenResidentialEntity, bool> coverWhere)
        {
            if (skipWhere != null)
            {
                Func<ProfileSanitationGreenResidentialEntity, bool> dbSkipWhere = db => skipWhere(db, Entity);

                var dbSkipQuery = service.dbcontext.Set<ProfileSanitationGreenResidentialEntity>().Where(dbSkipWhere);

                if (dbSkipQuery.Count() > 0)
                {
                    return;
                }
            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationGreenResidentialEntity, bool> dbCoverWhere = db => coverWhere(db, Entity);

                var dbCoverQuery = service.dbcontext.Set<ProfileSanitationGreenResidentialEntity>().Where(dbCoverWhere);

                if (dbCoverQuery.Count() > 0)
                {
                    var dbEntity = dbCoverQuery.FirstOrDefault();

                    dbEntity.CityId = Entity.CityId;
                    dbEntity.CountyId = Entity.CountyId;
                    dbEntity.ProjectId = Entity.ProjectId;
                    dbEntity.StreetId = Entity.StreetId;
                    dbEntity.SomeDigits = Entity.SomeDigits;
                    dbEntity.ResidentialName = Entity.ResidentialName;
                    dbEntity.Address = Entity.Address;
                    dbEntity.F_EnCode = Entity.F_EnCode;
                    dbEntity.ExchangeTime = Entity.ExchangeTime;
                    dbEntity.Note = Entity.Note;
                    dbEntity.PropertyName = Entity.PropertyName;

                    dbEntity.Modify(dbEntity.F_Id);
                    service.Update(dbEntity);

                    return;
                }
            }

            Entity.Create();

            service.Insert(Entity);
        }
    }
}
