using NFine.Code;
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
    public class ProfileSanitationWayApp
    {
        private ProfileSanitationWayRepositoty service = new ProfileSanitationWayRepositoty();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileSanitationWayEntity> FildSql(string enCode)
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
        public List<ProfileSanitationWayEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileSanitationWayEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.WayName.Contains(keyword));
                expression = expression.Or(t => t.Origin.Contains(keyword));
                expression = expression.Or(t => t.Destination.Contains(keyword));
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
        public List<ProfileSanitationWayEntity> GetList(Pagination pagination, string keyword, string projectId, string streetId)
        {
            var expression = ExtLinq.True<ProfileSanitationWayEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                int intF_code = 0;
                int.TryParse(keyword, out intF_code);
                expression = expression.And(t => t.WayName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode == intF_code);
                expression = expression.Or(t => t.Origin.Contains(keyword));
                expression = expression.Or(t => t.Destination.Contains(keyword));
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
        public ProfileSanitationWayEntity GetForm(string keyValue)
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
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除环卫道路信息【" + GetForm(keyValue).WayName + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 提交，修改
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ProfileSanitationWayEntity wayEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                wayEntity.Modify(keyValue);

                service.Update(wayEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "修改环卫道路信息【" + wayEntity.WayName + "】成功！");
                }
                catch { }
            }
            else
            {
                wayEntity.Create();

                service.Insert(wayEntity);

                try
                {
                    //添加日志
                    LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "新建环卫道路信息【" + wayEntity.WayName + "】成功！");
                }
                catch { }

            }
        }

        /// <summary>
        /// 获取字典 
        /// KEy为主键Id
        /// Value为岂止点
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetDictionaryToIDMoreThan(string enCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(enCode);

            return service.dbcontext.Database.SqlQuery<ProfileSanitationWayEntity>(strSql.ToString()).Select(d => new KeyValuePair<string, string>(d.F_Id, d.Origin + "-" + d.Destination)).ToList();
        }

        /// <summary>
        /// 批量添加数据
        /// </summary>
        public void BatchSubmitFrom(ProfileSanitationWayEntity[] wayEntitys, Func<ProfileSanitationWayEntity, ProfileSanitationWayEntity, bool> coverWhere)
        {
            ProfileSanitationWayEntity wayEntity = null;

            Func<ProfileSanitationWayEntity, bool> where = null;
            foreach (var item in wayEntitys)
            {
                if (item == null)
                    continue;

                if (coverWhere != null)
                {
                    where = (ProfileSanitationWayEntity dbEntity) =>
                    {
                        return coverWhere(dbEntity, item);
                    };

                    var query = service.dbcontext.Set<ProfileSanitationWayEntity>().Where(where);

                    //满足覆盖条件 则修改逻辑
                    if (query.Count() > 0)
                    {
                        wayEntity = query.FirstOrDefault();

                        wayEntity.CityId = item.CityId;
                        wayEntity.CountyId = item.CountyId;
                        wayEntity.ProjectId = item.ProjectId;
                        wayEntity.StreetId = item.StreetId;
                        wayEntity.MainWayId = item.MainWayId;
                        wayEntity.WayName = item.WayName;
                        wayEntity.Origin = item.Origin;
                        wayEntity.Destination = item.Destination;
                        wayEntity.WayGrade = item.WayGrade;

                        wayEntity.Modify(wayEntity.F_Id);
                        this.service.Update(wayEntity);

                        continue;
                    }
                }

                //添加新
                item.Create();
                this.service.Insert(item);
            }
        }

        public void BatchSubmitFrom(ProfileSanitationWayEntity wayEntity, Func<ProfileSanitationWayEntity, ProfileSanitationWayEntity, bool> skipWhere, Func<ProfileSanitationWayEntity, ProfileSanitationWayEntity, bool> coverWhere)
        {
            if (wayEntity == null)
                return;


            //是否跳过
            if (skipWhere != null)
            {
                //装载跳过where query条件
                Func<ProfileSanitationWayEntity, bool> dbSkipWhere = (db) =>
                {
                    return skipWhere(db, wayEntity);
                };
                var skipQuery = service.dbcontext.Set<ProfileSanitationWayEntity>().Where(dbSkipWhere);

                if (skipQuery.Count() > 0)
                    return;
            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationWayEntity, bool> dbCoverWhere = (db) =>
                {
                    return coverWhere(db, wayEntity);
                };

                var coverQuery = service.dbcontext.Set<ProfileSanitationWayEntity>().Where(dbCoverWhere);
                //存在覆盖
                if (coverQuery.Count() > 0)
                {
                    //覆盖

                    var dbWayEntity = coverQuery.FirstOrDefault();

                    dbWayEntity.CityId = wayEntity.CityId;
                    dbWayEntity.CountyId = wayEntity.CountyId;
                    dbWayEntity.ProjectId = wayEntity.ProjectId;
                    dbWayEntity.StreetId = wayEntity.StreetId;
                    dbWayEntity.MainWayId = wayEntity.MainWayId;
                    dbWayEntity.WayName = wayEntity.WayName;
                    dbWayEntity.Origin = wayEntity.Origin;
                    dbWayEntity.Destination = wayEntity.Destination;
                    dbWayEntity.WayGrade = wayEntity.WayGrade;
                    dbWayEntity.F_EnCode = wayEntity.F_EnCode;

                    wayEntity.Modify(dbWayEntity.F_Id);
                    this.service.Update(dbWayEntity);

                    return;
                }
            }


            wayEntity.Create();

            //添加
            service.Insert(wayEntity);
        }
    }
}
