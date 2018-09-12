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
    public class ProfileSanitationTandasApp
    {
        private ProfileSanitationTandasRepository service = new ProfileSanitationTandasRepository();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileSanitationTandasEntity> FildSql(string enCode)
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
        public List<ProfileSanitationTandasEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileSanitationTandasEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_code = 0;
                int.TryParse(keyword, out intF_code);

                expression = expression.And(t => t.Address.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_code);
                expression = expression.Or(t => t.CleaningUnit.Contains(keyword));
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
        public List<ProfileSanitationTandasEntity> GetList(Pagination pagination, string keyword, string projectId, string streetId)
        {
            var expression = ExtLinq.True<ProfileSanitationTandasEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {

                int intF_code = 0;
                int.TryParse(keyword, out intF_code);

                expression = expression.And(t => t.Address.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_code);
                expression = expression.Or(t => t.CleaningUnit.Contains(keyword));
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
        public ProfileSanitationTandasEntity GetForm(string keyValue)
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
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除环卫公厕信息【" + GetForm(keyValue).Address + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 提交，修改
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ProfileSanitationTandasEntity tandasEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                tandasEntity.Modify(keyValue);

                service.Update(tandasEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "修改环卫公厕信息【" + tandasEntity.Address + "】成功！");
                }
                catch { }
            }
            else
            {
                tandasEntity.Create();

                service.Insert(tandasEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "新建环卫公厕信息【" + tandasEntity.Address + "】成功！");
                }
                catch { }

            }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="tandasEntitys"></param>
        public void SubmitDatas(List<ProfileSanitationTandasEntity> tandasEntitys)
        {
            service.SubmitDatas(tandasEntitys);
        }

        /// <summary>
        /// 单条
        /// 批量导入使用
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="skipWhere"></param>
        /// <param name="coverWhere"></param>
        public void BatchSubmitFrom(ProfileSanitationTandasEntity tandasEntity, Func<ProfileSanitationTandasEntity, ProfileSanitationTandasEntity, bool> skipWhere, Func<ProfileSanitationTandasEntity, ProfileSanitationTandasEntity, bool> coverWhere)
        {

            if (skipWhere != null)
            {
                Func<ProfileSanitationTandasEntity, bool> dbSkipWhere = db => skipWhere(db, tandasEntity);

                var skipQuery = service.dbcontext.Set<ProfileSanitationTandasEntity>().Where(dbSkipWhere);

                if (skipQuery.Count() > 0)
                    return;
            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationTandasEntity, bool> dbCoverWhere = db => coverWhere(db, tandasEntity);

                var coverQuery = service.dbcontext.Set<ProfileSanitationTandasEntity>().Where(dbCoverWhere);

                if (coverQuery.Count() > 0)
                {
                    var dbEnity = coverQuery.FirstOrDefault();

                    dbEnity.CityId = tandasEntity.CityId;
                    dbEnity.CountyId = tandasEntity.CountyId;
                    dbEnity.ProjectId = tandasEntity.ProjectId;
                    dbEnity.StreetId = tandasEntity.StreetId;
                    dbEnity.Address = tandasEntity.Address;
                    dbEnity.Grade = tandasEntity.Grade;
                    dbEnity.F_EnCode = tandasEntity.F_EnCode;
                    dbEnity.ManagementForm = tandasEntity.ManagementForm;
                    dbEnity.CleaningUnit = tandasEntity.CleaningUnit;

                    dbEnity.Modify(dbEnity.F_Id);

                    service.Update(dbEnity);

                    return;
                }
            }

            //添加
            tandasEntity.Create();

            service.Insert(tandasEntity);
        }

    }
}
