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
    /// 环评-环卫-垃圾箱房
    /// </summary>
    public class ProfileSanitationGarbageBoxApp
    {
        private ProfileSanitationGarbageBoxRepository service = new ProfileSanitationGarbageBoxRepository();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileSanitationGarbageBoxEntity> FildSql(string enCode)
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
        public List<ProfileSanitationGarbageBoxEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileSanitationGarbageBoxEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_Code = 0;
                int.TryParse(keyword, out intF_Code);

                expression = expression.And(t => t.Address.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_Code);
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
        public List<ProfileSanitationGarbageBoxEntity> GetList(Pagination pagination, string keyword, string projectId, string streetId)
        {
            var expression = ExtLinq.True<ProfileSanitationGarbageBoxEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_Code = 0;
                int.TryParse(keyword, out intF_Code);

                expression = expression.And(t => t.Address.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_Code);
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
        public ProfileSanitationGarbageBoxEntity GetForm(string keyValue)
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
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除环卫垃圾箱房信息【" + GetForm(keyValue).Address + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 提交，修改
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ProfileSanitationGarbageBoxEntity GarBoxEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                GarBoxEntity.Modify(keyValue);

                service.Update(GarBoxEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "修改环卫垃圾箱房信息【" + GarBoxEntity.Address + "】成功！");
                }
                catch { }
            }
            else
            {
                GarBoxEntity.Create();

                service.Insert(GarBoxEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "新建环卫垃圾箱房信息【" + GarBoxEntity.Address + "】成功！");
                }
                catch { }

            }
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        public void ImportData(ProfileSanitationGarbageBoxEntity[] Entitys, out int successfulCount, out int failureCount)
        {
            successfulCount = 0;
            failureCount = 0;
            for (int i = 0; i < Entitys.Length; i++)
            {
                if (Entitys[i] == null)
                    continue;

                Entitys[i].Create();

                try
                {
                    service.Insert(Entitys[i]);
                    successfulCount += 1;
                }
                catch
                {
                    failureCount += 1;
                }
            }
        }

        /// <summary>
        /// 单条
        /// 批量导入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="skipWhere"></param>
        /// <param name="coverWhere"></param>
        public void BatchSubmitFrom(ProfileSanitationGarbageBoxEntity entity, Func<ProfileSanitationGarbageBoxEntity, ProfileSanitationGarbageBoxEntity, bool> skipWhere, Func<ProfileSanitationGarbageBoxEntity, ProfileSanitationGarbageBoxEntity, bool> coverWhere)
        {
            if (skipWhere != null)
            {
                Func<ProfileSanitationGarbageBoxEntity, bool> dbSkipWhere = db => skipWhere(db, entity);

                var dbSkipQuery = this.service.dbcontext.Set<ProfileSanitationGarbageBoxEntity>().Where(dbSkipWhere);

                if (dbSkipQuery.Count() > 0)
                {
                    return;
                }
            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationGarbageBoxEntity, bool> dbCoverWhere = db => coverWhere(db, entity);

                var dbCoverQuery = this.service.dbcontext.Set<ProfileSanitationGarbageBoxEntity>().Where(dbCoverWhere);

                if (dbCoverQuery.Count() > 0)
                {
                    var dbEntity = dbCoverQuery.FirstOrDefault();

                    dbEntity.CityId = entity.CityId;
                    dbEntity.CountyId = entity.CountyId;
                    dbEntity.ProjectId = entity.ProjectId;
                    dbEntity.F_EnCode = entity.F_EnCode;
                    dbEntity.StreetId = entity.StreetId;
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
