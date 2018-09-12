using NFine.Code;
using NFine.Domain.Contracts;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enums;
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
    /// 环评-环卫
    /// </summary>
    public class ProfileSanitationCarApp
    {
        private ProfileSanitationCarRepository service = new ProfileSanitationCarRepository();
        private ProfileSanitationCarWorkItemRepository workItemService = new ProfileSanitationCarWorkItemRepository();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileSanitationCarEntity> FildSql(string enCode)
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
        public List<ProfileSanitationCarEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileSanitationCarEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_EnCode.Contains(keyword));
                expression = expression.Or(t => t.WorkShift.Contains(keyword));
                expression = expression.Or(t => t.CarId.Contains(keyword));
            }

            return service.FindList(expression, pagination);
        }

        /// <summary>
        ///  获取数据列表
        /// </summary>
        /// <param name="carType">车辆类型</param>
        /// <param name="pagination">分页，排序参数</param>
        /// <param name="keyword">检索关键字</param>
        /// <returns></returns>
        public List<ProfileSanitationCarEntity> GetList(ProfileCarTypeEnum carType, Pagination pagination, string keyword)
        {
            int carTypeInt = carType.GetIntValue();

            var expression = ExtLinq.True<ProfileSanitationCarEntity>();

            expression = expression.And(t => t.CarType == carTypeInt);

            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_EnCode.Contains(keyword));
                expression = expression.Or(t => t.WorkShift.Contains(keyword));
                expression = expression.Or(t => t.CarId.Contains(keyword));
            }

            return service.FindList(expression, pagination);
        }

        /// <summary>
        /// 获取车辆的工作流程
        /// </summary>
        /// <param name="carId"></param>
        /// <returns></returns>
        public List<ProfileCarWorkItemContracts> GetCarWorkItem(string carId)
        {
            List<ProfileCarWorkItemContracts> result = null;

            var carEntity = service.FindEntity(carId);

            service.QueryCommand<ProfileSanitationCarWorkItemEntity>((query) =>
            {
                result = query.Where(d => d.WorkShift == carEntity.WorkShift).OrderBy(d => d.Subscript).Select(d => new ProfileCarWorkItemContracts()
                {
                    id = d.F_Id,
                    time = d.WorkTime,
                    rinseAddress = d.WorkAddress,
                    rinseName = d.WorkName,
                    subscript = d.Subscript,
                    Note = d.Note
                }).ToList();
            });

            return result;
        }

        /// <summary>
        /// 提交，修改
        /// </summary>
        /// <param name="tandasEntity"></param>
        /// <param name="keyValue"></param>
        public void SubmitForm(ProfileSanitationCarEntity entity, string keyValue, ProfileCarWorkItemContracts[] works)
        {

            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
            }
            else
            {
                entity.Create();
            }

            service.SubmitForm(entity, keyValue, works);

            try
            {
                LogMess.addLog(DbLogType.Update.ToString(), "修改成功", "新建环卫车辆信息【" + entity.CarId + "】成功！");
            }
            catch
            {
            }
        }


        public void SubmitForm(ProfileSanitationCarEntity entity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);

                service.Update(entity);

            }
            else
            {
                entity.Create();

                service.Insert(entity);

            }
        }

        public void SubmitWorkItemForm(ProfileSanitationCarWorkItemEntity entity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);

                workItemService.Update(entity);

            }
            else
            {
                entity.Create();

                workItemService.Insert(entity);

            }
        }

        /// <summary>
        /// 根据id获取单挑数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ProfileSanitationCarEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
            try
            {
                //添加日志
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除环卫车辆信息【" + GetForm(keyValue).CarId + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 单条
        /// 批量导入车辆信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="skipWhere"></param>
        /// <param name="coverWhere"></param>
        public void BatchCarSubmitFrom(ProfileSanitationCarEntity entity, Func<ProfileSanitationCarEntity, ProfileSanitationCarEntity, bool> skipWhere, Func<ProfileSanitationCarEntity, ProfileSanitationCarEntity, bool> coverWhere)
        {
            if (skipWhere != null)
            {
                Func<ProfileSanitationCarEntity, bool> dbSkipWhere = db => skipWhere(db, entity);

                var dbSkipQuery = this.service.dbcontext.Set<ProfileSanitationCarEntity>().Where(dbSkipWhere);

                if (dbSkipQuery.Count() > 0)
                {
                    return;
                }
            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationCarEntity, bool> dbCoverWhere = db => coverWhere(db, entity);

                var dbCoverQuery = this.service.dbcontext.Set<ProfileSanitationCarEntity>().Where(dbCoverWhere);

                if (dbCoverQuery.Count() > 0)
                {

                    var dbEntity = dbCoverQuery.FirstOrDefault();

                    dbEntity.CarId = entity.CarId;
                    dbEntity.CarType = entity.CarType;
                    dbEntity.CompanyId = entity.CompanyId;
                    dbEntity.F_EnCode = entity.F_EnCode;
                    dbEntity.WorkShift = entity.WorkShift;

                    dbEntity.Modify(dbEntity.F_Id);
                    this.service.Update(dbEntity);

                    return;
                }
            }

            entity.Create();
            this.service.Insert(entity);

        }

        /// <summary>
        /// 单条
        /// 批量导入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="skipWhere"></param>
        /// <param name="coverWhere"></param>
        public void BatchCarWorkItemSubmitFrom(ProfileSanitationCarWorkItemEntity entity, Func<ProfileSanitationCarWorkItemEntity, ProfileSanitationCarWorkItemEntity, bool> skipWhere, Func<ProfileSanitationCarWorkItemEntity, ProfileSanitationCarWorkItemEntity, bool> coverWhere)
        {
            if (skipWhere != null)
            {
                Func<ProfileSanitationCarWorkItemEntity, bool> dbskipWhere = db => skipWhere(db, entity);

                var dbSkipQuery = this.workItemService.dbcontext.Set<ProfileSanitationCarWorkItemEntity>().Where(dbskipWhere);

                if (dbSkipQuery.Count() > 0)
                {
                    return;
                }

            }

            if (coverWhere != null)
            {
                Func<ProfileSanitationCarWorkItemEntity, bool> dbCoverWhere = db => coverWhere(db, entity);

                var dbCoverQuery = this.workItemService.dbcontext.Set<ProfileSanitationCarWorkItemEntity>().Where(dbCoverWhere);

                if (dbCoverQuery.Count() > 0)
                {
                    var dbEntity = dbCoverQuery.FirstOrDefault();

                    dbEntity.Note = entity.Note;
                    dbEntity.Subscript = entity.Subscript;
                    dbEntity.WorkAddress = entity.WorkAddress;
                    dbEntity.WorkName = entity.WorkName;
                    dbEntity.WorkShift = entity.WorkShift;
                    dbEntity.WorkTime = entity.WorkTime;

                    dbEntity.Modify(dbEntity.F_Id);
                    this.workItemService.Update(dbEntity);

                    return;
                }

            }

            entity.Create();
            this.workItemService.Insert(entity);
        }
    }
}
