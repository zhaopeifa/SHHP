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
    /// 环评-环卫-倒粪池小便池（污水池)
    /// </summary>
    public class ProfileSanitationCesspoolApp
    {
        private ProfileSanitationCesspoolRepository service = new ProfileSanitationCesspoolRepository();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileSanitationCesspoolEntity> FildSql(string enCode)
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
        public List<ProfileSanitationCesspoolEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileSanitationCesspoolEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_code = 0;
                int.TryParse(keyword, out intF_code);

                expression = expression.And(t => t.Address.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_code);
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
        public List<ProfileSanitationCesspoolEntity> GetList(Pagination pagination, string keyword, string projectId, string streetId)
        {
            var expression = ExtLinq.True<ProfileSanitationCesspoolEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_code = 0;
                int.TryParse(keyword, out intF_code);

                expression = expression.And(t => t.Address.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_code);
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
        public ProfileSanitationCesspoolEntity GetForm(string keyValue)
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
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除倒粪池小便池信息【" + GetForm(keyValue).Address + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 提交，修改
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ProfileSanitationCesspoolEntity cessEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                cessEntity.Modify(keyValue);

                service.Update(cessEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "修改环卫倒粪池小便池信息【" + cessEntity.Address + "】成功！");
                }
                catch { }
            }
            else
            {
                cessEntity.Create();

                service.Insert(cessEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "新建环卫倒粪池小便池信息【" + cessEntity.Address + "】成功！");
                }
                catch { }

            }
        }

        /// <summary>
        /// 单条
        /// 批量导入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="skipWhere"></param>
        /// <param name="coverWhere"></param>
        public void BatchSubmitFrom(ProfileSanitationCesspoolEntity entity, Func<ProfileSanitationCesspoolEntity, ProfileSanitationCesspoolEntity, bool> skipWhere, Func<ProfileSanitationCesspoolEntity, ProfileSanitationCesspoolEntity, bool> coverWhere)
        {
            if (skipWhere != null)
            {
                Func<ProfileSanitationCesspoolEntity, bool> dbSkipWhere = db => skipWhere(db, entity);

                var dbSkipQuery = this.service.dbcontext.Set<ProfileSanitationCesspoolEntity>().Where(dbSkipWhere);

                if (dbSkipQuery.Count() > 0)
                {
                    return;
                }
            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationCesspoolEntity, bool> dbCoverWhere = db => coverWhere(db, entity);

                var dbCoverQuery = this.service.dbcontext.Set<ProfileSanitationCesspoolEntity>().Where(dbCoverWhere);

                if (dbCoverQuery.Count() > 0)
                {
                    var dbEntity = dbCoverQuery.FirstOrDefault();

                    dbEntity.CityId = entity.CityId;
                    dbEntity.CountyId = entity.CountyId;
                    dbEntity.ProjectId = entity.ProjectId;
                    dbEntity.StreetId = entity.StreetId;
                    dbEntity.F_EnCode = entity.F_EnCode;
                    dbEntity.Address = entity.Address;

                    dbEntity.Modify(dbEntity.F_Id);
                    this.service.Update(dbEntity);

                    return;
                }

            }

            entity.Create();
            this.service.Insert(entity);
        }
    }
}
