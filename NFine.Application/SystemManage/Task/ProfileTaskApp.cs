using NFine.Code;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Contracts;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enums;
using NFine.Repository.SystemManage;
using NFine.Web.Function;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Application.SystemManage
{
    /// <summary>
    /// 环评-环卫-任务
    /// </summary>
    public class ProfileTaskApp
    {
        private ProfileTaskRepository service = new ProfileTaskRepository();
        private ProfileTaskEntryRepository taskEntryservice = new ProfileTaskEntryRepository();

        /// <summary>
        /// 使用sql查询
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public List<ProfileTaskEntity> FildSql(string enCode)
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
        public List<ProfileTaskEntity> GetList(Pagination pagination, string keyword)
        {
            var expression = ExtLinq.True<ProfileTaskEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_EnCode.Contains(keyword));
            }

            return service.FindList(expression, pagination);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ProfileTaskContracts> GetContractsList(Pagination pagination, string keyword, int? taskStateTypeInt = null)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var taskQuery = db.IQueryable<ProfileTaskEntity>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    taskQuery = taskQuery.Where(t => t.F_EnCode.Contains(keyword));
                }

                if (taskStateTypeInt != null)
                {
                    taskQuery = taskQuery.Where(t => t.State == taskStateTypeInt);
                }

                taskQuery = taskQuery.OrderByDescending(d => d.F_LastModifyTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                var contractsQuery = from taskEntityQ in taskQuery
                                     join userEntityQ in db.IQueryable<UserEntity>()
                                     on taskEntityQ.PersonInChargeId equals userEntityQ.F_Id
                                     select new ProfileTaskContracts
                                     {
                                         F_Id = taskEntityQ.F_Id,
                                         IsFixedPoint = taskEntityQ.IsFixedPoint,
                                         State = taskEntityQ.State,
                                         F_EnCode = taskEntityQ.F_EnCode,
                                         CityId = taskEntityQ.CityId,
                                         CountyId = taskEntityQ.CountyId,
                                         ProjectType = taskEntityQ.ProjectType,
                                         CompanyId = taskEntityQ.CompanyId,
                                         StreetId = taskEntityQ.StreetId,
                                         PersonInChargeId = taskEntityQ.PersonInChargeId,
                                         PersonInChargeRealName = userEntityQ.F_RealName,
                                         DeliveryTime = taskEntityQ.DeliveryTime,
                                         CompletionTime = taskEntityQ.CompletionTime
                                     };

                return contractsQuery.ToList();
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ProfileTaskContracts> GetContractsListOrderbyCreateTime(Pagination pagination, string keyword, int? taskStateTypeInt = null)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var taskQuery = db.IQueryable<ProfileTaskEntity>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    taskQuery = taskQuery.Where(t => t.F_EnCode.Contains(keyword));
                }

                if (taskStateTypeInt != null)
                {
                    taskQuery = taskQuery.Where(t => t.State == taskStateTypeInt);
                }

                taskQuery = taskQuery.OrderByDescending(d => d.F_CreatorTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                var contractsQuery = from taskEntityQ in taskQuery
                                     join userEntityQ in db.IQueryable<UserEntity>()
                                     on taskEntityQ.PersonInChargeId equals userEntityQ.F_Id
                                     select new ProfileTaskContracts
                                     {
                                         F_Id = taskEntityQ.F_Id,
                                         IsFixedPoint = taskEntityQ.IsFixedPoint,
                                         State = taskEntityQ.State,
                                         F_EnCode = taskEntityQ.F_EnCode,
                                         CityId = taskEntityQ.CityId,
                                         CountyId = taskEntityQ.CountyId,
                                         ProjectType = taskEntityQ.ProjectType,
                                         CompanyId = taskEntityQ.CompanyId,
                                         StreetId = taskEntityQ.StreetId,
                                         PersonInChargeId = taskEntityQ.PersonInChargeId,
                                         PersonInChargeRealName = userEntityQ.F_RealName,
                                         DeliveryTime = taskEntityQ.DeliveryTime,
                                         CompletionTime = taskEntityQ.CompletionTime
                                     };

                return contractsQuery.ToList();
            }
        }

        /// <summary>
        /// 根据id获取单挑数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ProfileTaskEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        /// <summary>
        /// 获取任务子项
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ProfileTaskEntryEntity GetTaskEntryForm(string keyValue)
        {
            return taskEntryservice.FindEntity(keyValue);
        }

        public void SubmitForm(TaskInsertContracts taskEntity, string keyValue)
        {
            service.SubmitForm(taskEntity, keyValue);
        }

        public void SubmitForm(TaskInsertFixedPointContracts taskEntity, string keyValue)
        {
            service.SubmitFormFixedPoint(taskEntity, keyValue);
        }

        public void SunmitFixedForm(PerfectFixedFormPointContracts fixedEntity)
        {
            var entryType = (ProfileTaskEntryTypeEnum)fixedEntity.EntryType;

            var entity = taskEntryservice.FindEntity(fixedEntity.TaskEntryId);

            switch (entryType)
            {

                case ProfileTaskEntryTypeEnum.Wastebasket:
                case ProfileTaskEntryTypeEnum.StreetTrash:

                    entity.BYMESS2 = fixedEntity.Address;
                    entity.BYMESS3 = true;


                    break;
                case ProfileTaskEntryTypeEnum.MachineCleanCar:
                case ProfileTaskEntryTypeEnum.WashTheCar:
                case ProfileTaskEntryTypeEnum.GarbageTruckCar:
                case ProfileTaskEntryTypeEnum.FlyingCar:
                case ProfileTaskEntryTypeEnum.EightLadleCar:
                    entity.BYMESS2 = fixedEntity.CartId;
                    entity.BYMESS3 = true;
                    break;
                default:
                    break;
            }

            taskEntryservice.Update(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void DeleteForm(string keyValue)
        {
            var deleteEntuty = GetForm(keyValue);
            deleteEntuty.Remove();

            this.service.DeleteForm(deleteEntuty);
            try
            {
                //添加日志
                LogMess.addLog(DbLogType.Delete.ToString(), "删除成功", "删除任务单信息【" + deleteEntuty.F_DeleteUserId + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 任务派发
        /// </summary>
        public void TaskDistributed(string keyValue)
        {
            service.TaskDistributed(keyValue);
        }

        /// <summary>
        /// 任务单作废
        /// </summary>
        public void TaskInvalid(string keyValue)
        {
            service.TaskInvalid(keyValue);

            try
            {
                //添加日志
                LogMess.addLog(DbLogType.Delete.ToString(), "作废成功", "作废任务单信息【" + this.GetForm(keyValue).F_EnCode + "】成功！");
            }
            catch { }
        }

        /// <summary>
        /// 任务单审核通过
        /// </summary>
        public void TaskToAudit(string keyValue)
        {
            service.TaskToAudit(keyValue);
        }

        /// <summary>
        /// 获取任务单
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public TaskInsertContracts GetTaskInsertContractsForm(string keyValue)
        {

            TaskInsertContracts result = new TaskInsertContracts();

            var taskEntity = GetForm(keyValue);

            result.F_Id = taskEntity.F_Id;
            result.CityId = taskEntity.CityId;
            result.CountyId = taskEntity.CountyId;
            result.StreetId = taskEntity.StreetId;
            result.CompanyId = taskEntity.CompanyId;
            result.CompletionTime = taskEntity.CompletionTime;
            result.PersonInChargeId = taskEntity.PersonInChargeId;

            int wayTypeInt = ProfileTaskEntryTypeEnum.Way.GetIntValue();
            int tandaTypeInt = ProfileTaskEntryTypeEnum.Tandas.GetIntValue();
            int garbageBoxTypeInt = ProfileTaskEntryTypeEnum.GarbageBox.GetIntValue();
            int compressionStationTypeInt = ProfileTaskEntryTypeEnum.compressionStation.GetIntValue();
            int greeningTypeInt = ProfileTaskEntryTypeEnum.Greening.GetIntValue();
            int greenResidentialTypeInt = ProfileTaskEntryTypeEnum.GreenResidential.GetIntValue();
            int cesspoolTypeInt = ProfileTaskEntryTypeEnum.cesspool.GetIntValue();
            int wastebasketInt = ProfileTaskEntryTypeEnum.Wastebasket.GetIntValue();
            int streetTrashInt = ProfileTaskEntryTypeEnum.StreetTrash.GetIntValue();

            int machineCleanCarInt = ProfileTaskEntryTypeEnum.MachineCleanCar.GetIntValue();
            int washTheCarInt = ProfileTaskEntryTypeEnum.WashTheCar.GetIntValue();
            int garbageTruckCarInt = ProfileTaskEntryTypeEnum.GarbageTruckCar.GetIntValue();
            int flyingCarInt = ProfileTaskEntryTypeEnum.FlyingCar.GetIntValue();
            int eightLadleCarInt = ProfileTaskEntryTypeEnum.EightLadleCar.GetIntValue();



            service.Command<ProfileTaskEntryEntity>((query) =>
            {
                //道路的
                var taskWayEntnty = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(wayTypeInt)).FirstOrDefault();
                result.WayId = taskWayEntnty.EntryDataId;
                result.WayPlaceCount = taskWayEntnty.BYMESS1 == null ? 0 : (int)taskWayEntnty.BYMESS1;


                result.TandasCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(tandaTypeInt)).Count();
                result.GarbageBoxCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(garbageBoxTypeInt)).Count();
                result.CompressionCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(compressionStationTypeInt)).Count();
                result.GreeningCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(greeningTypeInt)).Count();
                result.GreenResidentialCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(greenResidentialTypeInt)).Count();
                result.CesspoolCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(cesspoolTypeInt)).Count();
                result.WastebasketCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(wastebasketInt)).Count();
                result.StreetTrashCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(streetTrashInt)).Count();

                result.MachineCleanCarCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(machineCleanCarInt)).Count();
                result.WashTheCarCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(washTheCarInt)).Count();
                result.GarbageTruckCarCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(garbageTruckCarInt)).Count();
                result.FlyingCarCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(flyingCarInt)).Count();
                result.EightLadleCarCount = query.Where(d => d.TaskId == taskEntity.F_Id && d.TaskEntryType.Equals(eightLadleCarInt)).Count();
            });

            //获取主路
            service.Command<ProfileSanitationWayEntity>((query) =>
            {
                result.MainWayId = query.Where(d => d.F_Id == result.WayId).Select(d => d.MainWayId).FirstOrDefault();
            });

            return result;
        }

        public TaskInsertContracts GetTaskInsertContractsFormFixePoint(string keyValue)
        {
            TaskInsertContracts result = new TaskInsertContracts();

            var taskEntity = GetForm(keyValue);
            result.F_Id = taskEntity.F_Id;
            result.CityId = taskEntity.CityId;
            result.CountyId = taskEntity.CountyId;
            result.StreetId = taskEntity.StreetId;
            result.CompanyId = taskEntity.CompanyId;
            result.CompletionTime = taskEntity.CompletionTime;
            result.PersonInChargeId = taskEntity.PersonInChargeId;

            int wastebasketInt = ProfileTaskEntryTypeEnum.Wastebasket.GetIntValue();
            int streetTrashInt = ProfileTaskEntryTypeEnum.StreetTrash.GetIntValue();
            int machineCleanCarInt = ProfileTaskEntryTypeEnum.MachineCleanCar.GetIntValue();
            int washTheCarInt = ProfileTaskEntryTypeEnum.WashTheCar.GetIntValue();
            int garbageTruckCarInt = ProfileTaskEntryTypeEnum.GarbageTruckCar.GetIntValue();
            int flyingCarInt = ProfileTaskEntryTypeEnum.FlyingCar.GetIntValue();
            int eightLadleCarInt = ProfileTaskEntryTypeEnum.EightLadleCar.GetIntValue();

            using (var db = new RepositoryBase().BeginTrans())
            {
                result.WastebasketCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == wastebasketInt);
                result.StreetTrashCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == streetTrashInt);
                result.MachineCleanCarCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == machineCleanCarInt);
                result.WashTheCarCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == washTheCarInt);
                result.GarbageTruckCarCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == garbageTruckCarInt);
                result.FlyingCarCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == flyingCarInt);
                result.EightLadleCarCount = db.IQueryable<ProfileTaskEntryEntity>().Count(d => d.TaskId == keyValue && d.TaskEntryType == eightLadleCarInt);

                return result;
            }
        }

        /// <summary>
        /// 任务分配获取街道下检查项各个的数量
        /// </summary>
        /// <param name="streetId"></param>
        /// <returns></returns>
        public TaskScreeningCheckPostCountContracts GetCheckPostCount(string streetId)
        {
            TaskScreeningCheckPostCountContracts result = new TaskScreeningCheckPostCountContracts();

            using (var db = new RepositoryBase().BeginTrans())
            {
                #region 获取街道下公厕数量

                service.Command<ProfileSanitationTandasEntity>((query) =>
                {
                    result.TandasCount = query.Where(d => d.StreetId == streetId).Count();
                });

                #endregion


                #region 获取街道下倒粪池小便池数量

                result.CesspoolCount = db.IQueryable<ProfileSanitationCesspoolEntity>().Where(d => d.StreetId == streetId).Count();

                #endregion

                #region 获取街道下垃圾箱房数量
                service.Command<ProfileSanitationGarbageBoxEntity>((query) =>
                {
                    result.GarbageBox = query.Where(d => d.StreetId == streetId).Count();
                });
                #endregion

                #region 获取街道下压缩站数量

                service.Command<ProfileSanitationCompressionStationEntity>((query) =>
                {
                    result.compressionStation = query.Where(d => d.StreetId == streetId).Count();
                });

                #endregion

                #region 获取街道下沿途绿化数量

                service.Command<ProfileSanitationGreeningEntity>((query) =>
                {
                    result.Greening = query.Where(d => d.StreetId == streetId).Count();
                });

                #endregion

                #region 获取街道下绿色账户小区数量
                service.Command<ProfileSanitationGreenResidentialEntity>((query) =>
                {
                    result.GreenResidential = query.Where(d => d.StreetId == streetId).Count();
                });
                #endregion

                //--------非定点------------



                return result;
            }

        }

        public ITaskDetail[] GetTaskDetail(Pagination pagination, string taskId, ProfileTaskEntryTypeEnum taskEntryType)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                ITaskDetail[] result = null;
                Func<string, bool> getCompleteStateFun = (entryId) =>
                {
                    return db.IQueryable<ProfileDeducInsEntity>().Count(d => d.TaskEntry_Id == entryId) > 0 ? true : false;
                };

                int taskEntryTypeInt = taskEntryType.GetIntValue();

                var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                                join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                                on taskQ.F_Id equals taskEntryQ.TaskId
                                where taskQ.F_Id == taskId &&
                                taskEntryQ.TaskEntryType == taskEntryTypeInt
                                select new
                                {
                                    F_Id = taskEntryQ.F_Id,
                                    DataId = taskEntryQ.EntryDataId,
                                    PersonInChargeId = taskQ.PersonInChargeId,
                                    CompletionTime = taskQ.CompletionTime,
                                    OrdeNo = taskQ.F_EnCode,
                                    StreetId = taskQ.StreetId,
                                    BYMESS2 = taskEntryQ.BYMESS2,
                                    BYMESS3 = taskEntryQ.BYMESS3,
                                    DeliveryTime = taskQ.DeliveryTime
                                };

                //查一下用户名
                var userQuery = from taskQ in taskQuery
                                join userQ in db.IQueryable<UserEntity>()
                                on taskQ.PersonInChargeId equals userQ.F_Id
                                select new
                                {
                                    F_Id = taskQ.F_Id,
                                    DataId = taskQ.DataId,
                                    PersonInChargeId = taskQ.PersonInChargeId,
                                    CompletionTime = taskQ.CompletionTime,
                                    StreetId = taskQ.StreetId,
                                    BYMESS2 = taskQ.BYMESS2,
                                    BYMESS3 = taskQ.BYMESS3,
                                    OrdeNo = taskQ.OrdeNo,
                                    DeliveryTime = taskQ.DeliveryTime,
                                    PersonInChargeName = userQ.F_RealName
                                };

                //根据不同的查询不同的数据

                switch (taskEntryType)
                {
                    #region 道路
                    case ProfileTaskEntryTypeEnum.Way://道路

                        var taskWayEntrysQuery = from dataQ in
                                                     (from taskQ in userQuery
                                                      join dataQ in db.IQueryable<ProfileSanitationWayEntity>()
                                                      on taskQ.DataId equals dataQ.F_Id
                                                      select new
                                                      {
                                                          F_Id = taskQ.F_Id,
                                                          DataId = taskQ.DataId,
                                                          StreetId = dataQ.StreetId,
                                                          PersonInChargeId = taskQ.PersonInChargeId,
                                                          CompletionTime = taskQ.CompletionTime,
                                                          OrdeNo = taskQ.OrdeNo,
                                                          DeliveryTime = taskQ.DeliveryTime,
                                                          PersonInChargeName = taskQ.PersonInChargeName,
                                                          WayName = dataQ.WayName,
                                                          F_EnCode = dataQ.F_EnCode,
                                                          Origin = dataQ.Origin,
                                                          Destination = dataQ.Destination
                                                      })
                                                 join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                 on dataQ.StreetId equals streetQ.F_Id
                                                 select new
                                                 {
                                                     F_Id = dataQ.F_Id,
                                                     DataId = dataQ.DataId,
                                                     StreetId = dataQ.StreetId,
                                                     PersonInChargeId = dataQ.PersonInChargeId,
                                                     OrdeNo = dataQ.OrdeNo,
                                                     DeliveryTime = dataQ.DeliveryTime,
                                                     CompletionTime = dataQ.CompletionTime,
                                                     PersonInChargeName = dataQ.PersonInChargeName,
                                                     WayName = dataQ.WayName,
                                                     F_EnCode = dataQ.F_EnCode,
                                                     Origin = dataQ.Origin,
                                                     Destination = dataQ.Destination,
                                                     StreetIdName = streetQ.StreetName
                                                 };




                        //设置总记录数
                        pagination.records = taskWayEntrysQuery.Count();
                        //设置分页数据
                        taskWayEntrysQuery = taskWayEntrysQuery.OrderBy(t => t.F_EnCode).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);


                        result = taskWayEntrysQuery.Select(t => new TaskDetailWayContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            WayName = t.WayName,
                            Origin = t.Origin,
                            Destination = t.Destination,
                            StreetId = t.StreetId,
                            StreetName = t.StreetIdName,
                            OrdeNo = t.OrdeNo,
                            DeliveryTime = t.DeliveryTime,

                        }).ToArray();

                        break;
                    #endregion

                    #region 公厕

                    case ProfileTaskEntryTypeEnum.Tandas://公厕

                        var taskTandasEntrysQuery = from dataQ in
                                                        (from taskQ in userQuery
                                                         join dataQ in db.IQueryable<ProfileSanitationTandasEntity>()
                                                         on taskQ.DataId equals dataQ.F_Id
                                                         select new
                                                         {
                                                             F_Id = taskQ.F_Id,
                                                             DataId = taskQ.DataId,
                                                             PersonInChargeId = taskQ.PersonInChargeId,
                                                             CompletionTime = taskQ.CompletionTime,
                                                             OrdeNo = taskQ.OrdeNo,
                                                             DeliveryTime = taskQ.DeliveryTime,
                                                             PersonInChargeName = taskQ.PersonInChargeName,
                                                             StreetId = dataQ.StreetId,
                                                             Address = dataQ.Address,
                                                             CleaningUnit = dataQ.CleaningUnit
                                                         })
                                                    join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                    on dataQ.StreetId equals streetQ.F_Id
                                                    select new
                                                    {
                                                        F_Id = dataQ.F_Id,
                                                        DataId = dataQ.DataId,
                                                        PersonInChargeId = dataQ.PersonInChargeId,
                                                        CompletionTime = dataQ.CompletionTime,
                                                        OrdeNo = dataQ.OrdeNo,
                                                        DeliveryTime = dataQ.DeliveryTime,
                                                        PersonInChargeName = dataQ.PersonInChargeName,
                                                        StreetId = dataQ.StreetId,
                                                        Address = dataQ.Address,
                                                        CleaningUnit = dataQ.CleaningUnit,
                                                        StreetIdName = streetQ.StreetName
                                                    };

                        //设置总记录数
                        pagination.records = taskTandasEntrysQuery.Count();
                        //设置分页数据
                        taskTandasEntrysQuery = taskTandasEntrysQuery.OrderBy(t => t.F_Id).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskTandasEntrysQuery.Select(t => new TaskDetailTandasContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            CleaningUnit = t.CleaningUnit,
                            StreetId = t.StreetId,
                            StreetName = t.StreetIdName,
                            OrdeNo = t.OrdeNo,
                            DeliveryTime = t.DeliveryTime
                        }).ToArray();

                        break;

                    #endregion

                    #region 垃圾箱房

                    case ProfileTaskEntryTypeEnum.GarbageBox:

                        var taskGarbageBoxEntrysQuery = from dataQ in
                                                            (from taskQ in userQuery
                                                             join dataQ in db.IQueryable<ProfileSanitationGarbageBoxEntity>()
                                                             on taskQ.DataId equals dataQ.F_Id
                                                             select new
                                                             {
                                                                 F_Id = taskQ.F_Id,
                                                                 DataId = taskQ.DataId,
                                                                 PersonInChargeId = taskQ.PersonInChargeId,
                                                                 CompletionTime = taskQ.CompletionTime,
                                                                 OrdeNo = taskQ.OrdeNo,
                                                                 DeliveryTime = taskQ.DeliveryTime,
                                                                 PersonInChargeName = taskQ.PersonInChargeName,
                                                                 Address = dataQ.Address,
                                                                 StreetId = dataQ.StreetId,
                                                             })
                                                        join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                        on dataQ.StreetId equals streetQ.F_Id
                                                        select new
                                                        {
                                                            F_Id = dataQ.F_Id,
                                                            DataId = dataQ.DataId,
                                                            PersonInChargeId = dataQ.PersonInChargeId,
                                                            CompletionTime = dataQ.CompletionTime,
                                                            OrdeNo = dataQ.OrdeNo,
                                                            DeliveryTime = dataQ.DeliveryTime,
                                                            PersonInChargeName = dataQ.PersonInChargeName,
                                                            Address = dataQ.Address,
                                                            StreetId = dataQ.StreetId,
                                                            StreetName = streetQ.StreetName
                                                        };

                        //设置总记录数
                        pagination.records = taskGarbageBoxEntrysQuery.Count();
                        //设置分页
                        taskGarbageBoxEntrysQuery = taskGarbageBoxEntrysQuery.OrderBy(t => t.F_Id).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskGarbageBoxEntrysQuery.Select(t => new TaskDetailGarbageBoxContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            OrdeNo = t.OrdeNo,
                            DeliveryTime = t.DeliveryTime
                        }).ToArray();

                        break;


                    #endregion

                    #region 压缩站

                    case ProfileTaskEntryTypeEnum.compressionStation:

                        var taskcompressionStationEntrysQuery = from dataQ in
                                                                    (from taskQ in userQuery
                                                                     join dataQ in db.IQueryable<ProfileSanitationCompressionStationEntity>()
                                                                     on taskQ.DataId equals dataQ.F_Id
                                                                     select new
                                                                     {
                                                                         F_Id = taskQ.F_Id,
                                                                         DataId = taskQ.DataId,
                                                                         OrdeNo = taskQ.OrdeNo,
                                                                         DeliveryTime = taskQ.DeliveryTime,
                                                                         PersonInChargeId = taskQ.PersonInChargeId,
                                                                         CompletionTime = taskQ.CompletionTime,
                                                                         PersonInChargeName = taskQ.PersonInChargeName,
                                                                         Address = dataQ.Address,
                                                                         OpeningHours = dataQ.OpeningHours,
                                                                         StreetId = dataQ.StreetId
                                                                     })
                                                                join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                                on dataQ.StreetId equals streetQ.F_Id
                                                                select new
                                                                {
                                                                    F_Id = dataQ.F_Id,
                                                                    DataId = dataQ.DataId,
                                                                    PersonInChargeId = dataQ.PersonInChargeId,
                                                                    CompletionTime = dataQ.CompletionTime,
                                                                    OrdeNo = dataQ.OrdeNo,
                                                                    DeliveryTime = dataQ.DeliveryTime,
                                                                    PersonInChargeName = dataQ.PersonInChargeName,
                                                                    Address = dataQ.Address,
                                                                    OpeningHours = dataQ.OpeningHours,
                                                                    StreetId = dataQ.StreetId,
                                                                    StreetName = streetQ.StreetName
                                                                };

                        //设置总记录数
                        pagination.records = taskcompressionStationEntrysQuery.Count();
                        //设置分页
                        taskcompressionStationEntrysQuery = taskcompressionStationEntrysQuery.OrderBy(t => t.F_Id).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskcompressionStationEntrysQuery.Select(t => new TaskDetailCompressionStationContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            OpeningHours = t.OpeningHours,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            DeliveryTime = t.DeliveryTime,
                            OrdeNo = t.OrdeNo
                        }).ToArray();


                        break;

                    #endregion

                    #region 沿途绿化

                    case ProfileTaskEntryTypeEnum.Greening:

                        var taskGreeningEntrysQuery = from dataQ in
                                                          (from taskQ in userQuery
                                                           join dataQ in db.IQueryable<ProfileSanitationGreeningEntity>()
                                                           on taskQ.DataId equals dataQ.F_Id
                                                           select new
                                                           {
                                                               F_Id = taskQ.F_Id,
                                                               DataId = taskQ.DataId,
                                                               OrdeNo = taskQ.OrdeNo,
                                                               DeliveryTime = taskQ.DeliveryTime,
                                                               PersonInChargeId = taskQ.PersonInChargeId,
                                                               CompletionTime = taskQ.CompletionTime,
                                                               PersonInChargeName = taskQ.PersonInChargeName,
                                                               Address = dataQ.Address,
                                                               Origin = dataQ.Origin,
                                                               Destination = dataQ.Destination,
                                                               StreetId = dataQ.StreetId
                                                           })
                                                      join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                      on dataQ.StreetId equals streetQ.F_Id
                                                      select new
                                                      {
                                                          F_Id = dataQ.F_Id,
                                                          DataId = dataQ.DataId,
                                                          PersonInChargeId = dataQ.PersonInChargeId,
                                                          CompletionTime = dataQ.CompletionTime,
                                                          OrdeNo = dataQ.OrdeNo,
                                                          DeliveryTime = dataQ.DeliveryTime,
                                                          PersonInChargeName = dataQ.PersonInChargeName,
                                                          Address = dataQ.Address,
                                                          Origin = dataQ.Origin,
                                                          Destination = dataQ.Destination,
                                                          StreetId = dataQ.StreetId,
                                                          StreetName = streetQ.StreetName
                                                      };

                        //设置分页总数量
                        pagination.records = taskGreeningEntrysQuery.Count();
                        //设置分页
                        taskGreeningEntrysQuery = taskGreeningEntrysQuery.OrderBy(t => t.F_Id).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskGreeningEntrysQuery.Select(t => new TaskDetailGreeningContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            Origin = t.Origin,
                            Destination = t.Destination,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            DeliveryTime = t.DeliveryTime,
                            OrdeNo = t.OrdeNo
                        }).ToArray();

                        break;

                    #endregion

                    #region 绿色账户小区

                    case ProfileTaskEntryTypeEnum.GreenResidential:

                        var taskGreenResidentialEntrysQuery = from dataQ in
                                                                  (from taskQ in userQuery
                                                                   join dataQ in db.IQueryable<ProfileSanitationGreenResidentialEntity>()
                                                                   on taskQ.DataId equals dataQ.F_Id
                                                                   select new
                                                                   {
                                                                       F_Id = taskQ.F_Id,
                                                                       DataId = taskQ.DataId,
                                                                       OrdeNo = taskQ.OrdeNo,
                                                                       DeliveryTime = taskQ.DeliveryTime,
                                                                       PersonInChargeId = taskQ.PersonInChargeId,
                                                                       CompletionTime = taskQ.CompletionTime,
                                                                       PersonInChargeName = taskQ.PersonInChargeName,
                                                                       Address = dataQ.Address,
                                                                       ResidentialName = dataQ.ResidentialName,
                                                                       StreetId = dataQ.StreetId
                                                                   })
                                                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                              on dataQ.StreetId equals streetQ.F_Id
                                                              select new
                                                              {
                                                                  F_Id = dataQ.F_Id,
                                                                  DataId = dataQ.DataId,
                                                                  PersonInChargeId = dataQ.PersonInChargeId,
                                                                  CompletionTime = dataQ.CompletionTime,
                                                                  OrdeNo = dataQ.OrdeNo,
                                                                  DeliveryTime = dataQ.DeliveryTime,
                                                                  PersonInChargeName = dataQ.PersonInChargeName,
                                                                  Address = dataQ.Address,
                                                                  ResidentialName = dataQ.ResidentialName,
                                                                  StreetId = dataQ.StreetId,
                                                                  StreetName = streetQ.StreetName
                                                              };

                        //设置分页总数量
                        pagination.records = taskGreenResidentialEntrysQuery.Count();
                        //设置分页
                        taskGreenResidentialEntrysQuery = taskGreenResidentialEntrysQuery.OrderBy(t => t.F_Id).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskGreenResidentialEntrysQuery.Select(t => new TaskDetailGreenResidentialContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            ResidentialName = t.ResidentialName,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            DeliveryTime = t.DeliveryTime,
                            OrdeNo = t.OrdeNo
                        }).ToArray();

                        break;

                    #endregion

                    #region 倒粪池小便池

                    case ProfileTaskEntryTypeEnum.cesspool:
                        var taskcesspoolEntrysQuery = from dataQ in
                                                          (from taskQ in userQuery
                                                           join dataQ in db.IQueryable<ProfileSanitationCesspoolEntity>()
                                                           on taskQ.DataId equals dataQ.F_Id
                                                           select new
                                                           {
                                                               F_Id = taskQ.F_Id,
                                                               DataId = taskQ.DataId,
                                                               PersonInChargeId = taskQ.PersonInChargeId,
                                                               CompletionTime = taskQ.CompletionTime,
                                                               OrdeNo = taskQ.OrdeNo,
                                                               DeliveryTime = taskQ.DeliveryTime,
                                                               PersonInChargeName = taskQ.PersonInChargeName,
                                                               Address = dataQ.Address,
                                                               StreetId = dataQ.StreetId
                                                           })
                                                      join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                      on dataQ.StreetId equals streetQ.F_Id
                                                      select new
                                                      {
                                                          F_Id = dataQ.F_Id,
                                                          DataId = dataQ.DataId,
                                                          PersonInChargeId = dataQ.PersonInChargeId,
                                                          CompletionTime = dataQ.CompletionTime,
                                                          OrdeNo = dataQ.OrdeNo,
                                                          DeliveryTime = dataQ.DeliveryTime,
                                                          PersonInChargeName = dataQ.PersonInChargeName,
                                                          Address = dataQ.Address,
                                                          StreetId = dataQ.StreetId,
                                                          StreetName = streetQ.StreetName
                                                      };

                        //设置分页总数量
                        pagination.records = taskcesspoolEntrysQuery.Count();
                        //设置分页
                        taskcesspoolEntrysQuery = taskcesspoolEntrysQuery.OrderBy(t => t.F_Id).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskcesspoolEntrysQuery.Select(t => new TaskDetailCesspoolContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            DeliveryTime = t.DeliveryTime,
                            OrdeNo = t.OrdeNo
                        }).ToArray();

                        break;

                    #endregion

                    #region 废纸箱 沿街垃圾桶

                    case ProfileTaskEntryTypeEnum.Wastebasket:
                    case ProfileTaskEntryTypeEnum.StreetTrash:
                        var taskWastebasketEntrysQuery = from dataQ in userQuery
                                                         join streetQ in db.IQueryable<ProfileStreetEntity>()
                                                      on dataQ.StreetId equals streetQ.F_Id
                                                         select new
                                                         {
                                                             F_Id = dataQ.F_Id,
                                                             DataId = dataQ.DataId,
                                                             PersonInChargeId = dataQ.PersonInChargeId,
                                                             CompletionTime = dataQ.CompletionTime,
                                                             OrdeNo = dataQ.OrdeNo,
                                                             DeliveryTime = dataQ.DeliveryTime,
                                                             PersonInChargeName = dataQ.PersonInChargeName,
                                                             Address = dataQ.BYMESS2,
                                                             IsPerfect = dataQ.BYMESS3,
                                                             StreetId = dataQ.StreetId,
                                                             StreetName = streetQ.StreetName
                                                         };

                        //设置分页总数量
                        pagination.records = taskWastebasketEntrysQuery.Count();
                        //设置分页
                        taskWastebasketEntrysQuery = taskWastebasketEntrysQuery.OrderBy(t => t.CompletionTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskWastebasketEntrysQuery.Select(t => new TaskDetailWastebasketContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            Address = t.Address,
                            IsPerfect = t.IsPerfect,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            DeliveryTime = t.DeliveryTime,
                            OrdeNo = t.OrdeNo
                        }).ToArray();

                        break;

                    #endregion

                    #region 车辆的
                    case ProfileTaskEntryTypeEnum.MachineCleanCar:
                    case ProfileTaskEntryTypeEnum.WashTheCar:
                    case ProfileTaskEntryTypeEnum.GarbageTruckCar:
                    case ProfileTaskEntryTypeEnum.FlyingCar:
                    case ProfileTaskEntryTypeEnum.EightLadleCar:

                        var taskCarEntrysQuery = from dataQ in userQuery
                                                 join streetQ in db.IQueryable<ProfileStreetEntity>()
                                              on dataQ.StreetId equals streetQ.F_Id
                                                 select new
                                                 {
                                                     F_Id = dataQ.F_Id,
                                                     DataId = dataQ.DataId,
                                                     PersonInChargeId = dataQ.PersonInChargeId,
                                                     CompletionTime = dataQ.CompletionTime,
                                                     OrdeNo = dataQ.OrdeNo,
                                                     DeliveryTime = dataQ.DeliveryTime,
                                                     PersonInChargeName = dataQ.PersonInChargeName,
                                                     CarId = dataQ.BYMESS2,
                                                     IsPerfect = dataQ.BYMESS3,
                                                     StreetId = dataQ.StreetId,
                                                     StreetName = streetQ.StreetName
                                                 };

                        //设置分页总数量
                        pagination.records = taskCarEntrysQuery.Count();
                        //设置分页
                        taskCarEntrysQuery = taskCarEntrysQuery.OrderBy(t => t.CompletionTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                        result = taskCarEntrysQuery.Select(t => new TaskDetailOperatingVehiclesContracts()
                        {
                            F_Id = t.F_Id,
                            DataId = t.DataId,
                            PersonInChargeId = t.PersonInChargeId,
                            CompletionTime = t.CompletionTime,
                            PersonInChargeName = t.PersonInChargeName,
                            CarId = t.CarId,
                            IsPerfect = t.IsPerfect,
                            StreetId = t.StreetId,
                            StreetName = t.StreetName,
                            DeliveryTime = t.DeliveryTime,
                            OrdeNo = t.OrdeNo
                        }).ToArray();

                        break;
                    #endregion

                    default:
                        break;
                }

                foreach (var item in result)
                {
                    item.CompleteState = getCompleteStateFun(item.F_Id);
                }

                return result;
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ProfileTaskContracts> GetNeedUpLoadTask(Pagination pagination, string keyword, string IsFixedPoint)
        {
            int toAuditInt = ProfileTaskStateEnum.ToAudit.GetIntValue();
            int backToInt = ProfileTaskStateEnum.BackTo.GetIntValue();

            using (var db = new RepositoryBase().BeginTrans())
            {

                var taskQyery = db.IQueryable<ProfileTaskEntity>().Where(d => d.State == toAuditInt || d.State == backToInt);


                var taskQuery = db.IQueryable<ProfileTaskEntity>();

                if (!string.IsNullOrEmpty(keyword))
                {
                    taskQuery = taskQuery.Where(t => t.F_EnCode.Contains(keyword));
                }
                if (!string.IsNullOrEmpty(IsFixedPoint)&&!IsFixedPoint.Equals("-1"))
                { 
                   
                    bool isFixedPoint=IsFixedPoint.Equals("1")?true:false;
                    taskQuery=taskQuery.Where(t=>t.IsFixedPoint==isFixedPoint);
                }

                taskQuery = taskQuery.Where(t => t.State == toAuditInt || t.State == backToInt);

                taskQuery = taskQuery.OrderByDescending(d => d.F_LastModifyTime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);

                var contractsQuery = from taskEntityQ in taskQuery
                                     join userEntityQ in db.IQueryable<UserEntity>()
                                     on taskEntityQ.PersonInChargeId equals userEntityQ.F_Id
                                     select new ProfileTaskContracts
                                     {
                                         F_Id = taskEntityQ.F_Id,
                                         State = taskEntityQ.State,
                                         F_EnCode = taskEntityQ.F_EnCode,
                                         CityId = taskEntityQ.CityId,
                                         CountyId = taskEntityQ.CountyId,
                                         ProjectType = taskEntityQ.ProjectType,
                                         CompanyId = taskEntityQ.CompanyId,
                                         StreetId = taskEntityQ.StreetId,
                                         PersonInChargeId = taskEntityQ.PersonInChargeId,
                                         PersonInChargeRealName = userEntityQ.F_RealName,
                                         DeliveryTime = taskEntityQ.DeliveryTime,
                                         CompletionTime = taskEntityQ.CompletionTime,
                                         IsFixedPoint = taskEntityQ.IsFixedPoint
                                     };

                return contractsQuery.ToList();
            }
        }


        /// <summary>
        /// 获取对应评分标准
        /// </summary>
        public List<ScireCriteriaContracts> GetScireCriteria(string taskEnryId, string keyword)
        {
            List<ScireCriteriaContracts> result = new List<ScireCriteriaContracts>();

            using (var db = new RepositoryBase().BeginTrans())
            {
                //获取对应评分标准
                var query = db.IQueryable<ProfileTaskEntryEntity>().Where(d => d.F_Id == taskEnryId);

                if (query.Count() <= 0)
                {
                    throw new Exception("未找到对应任务明细!");
                }

                var taskEntry = query.FirstOrDefault();

                string scEntryName = string.Empty;
                string scTypeName = string.Empty;

                //寻找对应评分标准
                switch ((ProfileTaskEntryTypeEnum)taskEntry.TaskEntryType)
                {
                    #region 道路评分标准
                    case ProfileTaskEntryTypeEnum.Way:
                        scEntryName = "道路";

                        //获取对应道路去拿等级
                        var wayDataQuery = db.IQueryable<ProfileSanitationWayEntity>().Where(d => d.F_Id == taskEntry.EntryDataId);
                        if (wayDataQuery.Count() > 0)
                        {
                            var wayData = wayDataQuery.FirstOrDefault();
                            switch ((ProfileWayGradeEnum)wayData.WayGrade)
                            {
                                case ProfileWayGradeEnum.一级道路:
                                    scTypeName = "一级道路";
                                    break;
                                case ProfileWayGradeEnum.二级道路:
                                    scTypeName = "二级道路";
                                    break;
                                case ProfileWayGradeEnum.三级及其它:
                                    scTypeName = "三级道路";
                                    break;
                                default:
                                    scTypeName = "无";
                                    break;
                            }
                        }

                        break;

                    #endregion
                    #region 公厕评分标准

                    case ProfileTaskEntryTypeEnum.Tandas:
                        scEntryName = "公厕";

                        var tandasDataQuery = db.IQueryable<ProfileSanitationTandasEntity>().Where(d => d.F_Id == taskEntry.EntryDataId);
                        if (tandasDataQuery.Count() > 0)
                        {
                            var tandasData = tandasDataQuery.FirstOrDefault();
                            switch ((ProfileTandasGradeEnum)tandasData.Grade)
                            {
                                case ProfileTandasGradeEnum.一类公厕:
                                    scTypeName = "一类公厕";
                                    break;
                                case ProfileTandasGradeEnum.二类公厕:
                                    scTypeName = "二类公厕";
                                    break;
                                case ProfileTandasGradeEnum.三类公厕:
                                    scTypeName = "三类公厕";
                                    break;
                                default:
                                    break;
                            }
                        }

                        break;

                    #endregion
                    #region 垃圾箱房

                    case ProfileTaskEntryTypeEnum.GarbageBox:
                        scEntryName = "垃圾箱房";

                        var garbageBoxQuery = db.IQueryable<ProfileSanitationGarbageBoxEntity>().Where(d => d.F_Id == taskEntry.EntryDataId);
                        if (garbageBoxQuery.Count() > 0)
                        {
                            var garbageBoxEntity = garbageBoxQuery.FirstOrDefault();
                            switch ((ProfileSanitationGarbageBoxTypeEnum)garbageBoxEntity.GarbageBoxType)
                            {
                                case ProfileSanitationGarbageBoxTypeEnum.小区压缩站:
                                    scTypeName = "小区内箱房";
                                    break;
                                case ProfileSanitationGarbageBoxTypeEnum.沿街压缩站:
                                    scTypeName = "沿街箱房";
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;

                    #endregion

                    #region 压缩站

                    case ProfileTaskEntryTypeEnum.compressionStation:
                        scEntryName = "压缩站";

                        var compressionStationQuery = db.IQueryable<ProfileSanitationCompressionStationEntity>().Where(d => d.F_Id == taskEntry.EntryDataId);
                        if (compressionStationQuery.Count() > 0)
                        {
                            var compressionStationEntiy = compressionStationQuery.FirstOrDefault();
                            switch ((ProfileCompressionStationType)compressionStationEntiy.CompType)
                            {
                                case ProfileCompressionStationType.沿街压缩站:
                                    scTypeName = "沿街压缩站";
                                    break;
                                case ProfileCompressionStationType.小区压缩站:
                                    scTypeName = "小区压缩站";
                                    break;
                                default:
                                    break;
                            }
                        }

                        break;

                    #endregion

                    #region 沿途绿化

                    case ProfileTaskEntryTypeEnum.Greening:
                        scEntryName = "绿化";
                        scTypeName = "绿化带";
                        break;

                    #endregion
                    #region 绿色账户小区
                    case ProfileTaskEntryTypeEnum.GreenResidential:
                        scEntryName = "绿色账户小区";
                        //scTypeName = "绿色账户小区";
                        break;

                    #endregion

                    #region 倒粪站 小便池
                    case ProfileTaskEntryTypeEnum.cesspool:
                        scEntryName = "沿街垃圾收集设施";
                        scTypeName = "倒粪站 小便池";

                        break;

                    #endregion
                    #region 废物箱

                    case ProfileTaskEntryTypeEnum.Wastebasket:
                        scEntryName = "沿街垃圾收集设施";
                        scTypeName = "废物箱";
                        break;

                    #endregion
                    #region 沿街垃圾桶
                    case ProfileTaskEntryTypeEnum.StreetTrash:
                        scEntryName = "沿街垃圾收集设施";
                        scTypeName = "沿街垃圾桶";
                        break;

                    #endregion
                    #region 车辆
                    case ProfileTaskEntryTypeEnum.MachineCleanCar:
                        scEntryName = "环卫车辆";
                        scTypeName = "机扫车";
                        break;
                    case ProfileTaskEntryTypeEnum.WashTheCar:
                        scEntryName = "环卫车辆";
                        scTypeName = "冲洗车";
                        break;
                    case ProfileTaskEntryTypeEnum.GarbageTruckCar:
                        scEntryName = "环卫车辆";
                        scTypeName = "清运车";
                        break;
                    case ProfileTaskEntryTypeEnum.FlyingCar:
                        scEntryName = "环卫车辆";
                        scTypeName = "电动机具";
                        break;
                    case ProfileTaskEntryTypeEnum.EightLadleCar:
                        scEntryName = "环卫车辆";
                        scTypeName = "电动机具";
                        break;
                    #endregion
                    default:
                        break;
                }

                StringBuilder sqlStr = new StringBuilder();
                sqlStr.Append("SELECT c.Name AS SEntryName,c.SEntryId AS SEntryId,b.Name AS STypeName,b.STypeId AS STypeId,a.SClassifyId as SClassifyId,a.SClassifyName as SClassifyName,a.Score as Score  ");
                sqlStr.Append("FROM ProfileScoreCriteria_Classify a LEFT JOIN ProfileScoreCriteria_Type b ");
                sqlStr.Append("ON a.STypeId=b.STypeId LEFT JOIN ProfileScoreCriteria_Entry c ");
                sqlStr.Append("  ON b.SEntryId=c.SEntryId where 1=1 ");

                if (!string.IsNullOrEmpty(scEntryName))
                {
                    sqlStr.Append(" AND c.Name='" + scEntryName + "' ");
                }
                if (!string.IsNullOrEmpty(scTypeName))
                {
                    sqlStr.Append("  AND b.Name='" + scTypeName + "'   ");
                }

                DataTable table = DbHelper.ExecuteDataTable(sqlStr.ToString(), null);

                ScireCriteriaContracts scContr = null;
                ScireCriteriaNormContracts scNormContr = null;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    scContr = new ScireCriteriaContracts();

                    if (table.Rows[i]["SEntryName"] != null)
                    {
                        scContr.SEntryName = table.Rows[i]["SEntryName"].ToString();
                    }
                    if (table.Rows[i]["SEntryId"] != null)
                    {
                        scContr.SEntryId = table.Rows[i]["SEntryId"].ToString();
                    }
                    if (table.Rows[i]["STypeName"] != null)
                    {
                        scContr.STypeName = table.Rows[i]["STypeName"].ToString();
                    }
                    if (table.Rows[i]["STypeId"] != null)
                    {
                        scContr.STypeId = table.Rows[i]["STypeId"].ToString();
                    }
                    if (table.Rows[i]["SClassifyId"] != null)
                    {
                        scContr.SClassifyId = table.Rows[i]["SClassifyId"].ToString();
                    }
                    if (table.Rows[i]["SClassifyName"] != null)
                    {
                        scContr.SClassifyName = table.Rows[i]["SClassifyName"].ToString();
                    }
                    if (table.Rows[i]["Score"] != null)
                    {
                        scContr.SClassifyScore = (int)table.Rows[i]["Score"];
                    }


                    sqlStr.Clear();
                    sqlStr.Append("SELECT SNormId,SNormProjectName,SNormStandardName,Condition,IsDeduct FROM ProfileScireCriteria_Norm WHERE SClassifyId='" + scContr.SClassifyId + "' ");


                    if (!string.IsNullOrEmpty(keyword))
                    {
                        sqlStr.Append("And (SNormProjectName LIKE '%" + keyword + "%' ");
                        sqlStr.Append("OR SNormStandardName LIKE '%" + keyword + "%' )");
                    }

                    var normTable = DbHelper.ExecuteDataTable(sqlStr.ToString(), null);

                    scContr.SNromCollecion = new List<ScireCriteriaNormContracts>();


                    for (int j = 0; j < normTable.Rows.Count; j++)
                    {
                        scNormContr = new ScireCriteriaNormContracts();

                        if (normTable.Rows[j]["SNormId"] != null)
                        {
                            scNormContr.SNormId = normTable.Rows[j]["SNormId"].ToString();
                        }
                        if (normTable.Rows[j]["SNormProjectName"] != null)
                        {
                            scNormContr.SNormProjectName = normTable.Rows[j]["SNormProjectName"].ToString();
                        }
                        if (normTable.Rows[j]["SNormStandardName"] != null)
                        {
                            scNormContr.SNormStandardName = normTable.Rows[j]["SNormStandardName"].ToString();
                        }
                        if (normTable.Rows[j]["Condition"] != null)
                        {
                            scNormContr.SNormCondition = (int)normTable.Rows[j]["Condition"];
                        }
                        if (normTable.Rows[j]["IsDeduct"] != null)
                        {
                            scNormContr.IsDeduct = (bool)normTable.Rows[j]["IsDeduct"];
                        }

                        scContr.SNromCollecion.Add(scNormContr);
                    }


                    result.Add(scContr);
                }


                return result;
            }
        }

        public ScorCriteriaClassifyTreeGridContracts GetScireCriteriaByNormId(string normId)
        {
            ScorCriteriaClassifyTreeGridContracts result = new ScorCriteriaClassifyTreeGridContracts();

            using (var db = new RepositoryBase().BeginTrans())
            {
                var normQuery = db.IQueryable<ProfileScireCriteria_NormEntity>().Where(d => d.SNormId == normId);
                if (normQuery.Count() <= 0)
                    return null;

                var scNormEntity = normQuery.FirstOrDefault();

                result.SNormProjectName = scNormEntity.SNormProjectName;
                result.SNormStandardName = scNormEntity.SNormStandardName;
                result.SNormCondition = scNormEntity.Condition;
                result.SNormIsDeduct = scNormEntity.IsDeduct;

                var classifyEntiy = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().Where(d => d.SClassifyId == scNormEntity.SClassifyId).FirstOrDefault();

                result.SClassifyName = classifyEntiy.SClassifyName;
                result.SClassifyScore = classifyEntiy.Score;

                var TypeEntity = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == classifyEntiy.STypeId).FirstOrDefault();

                result.STypeName = TypeEntity.Name;

                var entryEntity = db.IQueryable<ProfileScoreCriteria_EntryEntity>().Where(d => d.SEntryId == TypeEntity.SEntryId).FirstOrDefault();

                result.SEntryName = entryEntity.Name;

                return result;
            }
        }

        public bool GetTaskCount(out int notToSendCount, out int toAuditCount, out int havePutAnEndToCount, out int theCancellationCount)
        {

            int takNotToSendInt = ProfileTaskStateEnum.NotToSend.GetIntValue();
            int taskToAuditInt = ProfileTaskStateEnum.ToAudit.GetIntValue();
            int taskHavePutAnEndToInt = ProfileTaskStateEnum.HavePutAnEndTo.GetIntValue();
            int taskTheCancellationInt = ProfileTaskStateEnum.TheCancellation.GetIntValue();

            using (var db = new RepositoryBase().BeginTrans())
            {
                notToSendCount = db.IQueryable<ProfileTaskEntity>().Count(d => d.State == takNotToSendInt);
                toAuditCount = db.IQueryable<ProfileTaskEntity>().Count(d => d.State == taskToAuditInt);
                havePutAnEndToCount = db.IQueryable<ProfileTaskEntity>().Count(d => d.State == taskHavePutAnEndToInt);
                theCancellationCount = db.IQueryable<ProfileTaskEntity>().Count(d => d.State == taskTheCancellationInt);
            }

            return true;
        }
    }
}
