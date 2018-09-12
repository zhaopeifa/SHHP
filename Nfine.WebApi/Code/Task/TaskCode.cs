using NFine.Application.SystemManage;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enums;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Code.Task
{
    public class TaskCode : ITask
    {

        /// <summary>
        /// 获取未完成任务数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public int GetUndoneTaskCount(string userId, string entryId)
        {
            DateTime currentTime = DateTime.Now;
            int toAuditState = ProfileTaskStateEnum.ToAudit.GetIntValue();
            int taskType = -1;

            Func<int, bool> taskWhereFun = null;

            using (var db = new RepositoryBase().BeginTrans())
            {

                #region 获取条件
                var entryQuery = db.IQueryable<ProfileScoreCriteria_EntryEntity>().Where(d => d.SEntryId == entryId);

                var entryEntity = entryQuery.FirstOrDefault();

                switch (entryEntity.Name)
                {
                    case "道路":
                        taskType = ProfileTaskEntryTypeEnum.Way.GetIntValue();

                        taskWhereFun = (taskEntryId) => { return taskEntryId == taskType; };
                        break;
                    case "公厕":
                        taskType = ProfileTaskEntryTypeEnum.Tandas.GetIntValue();

                        taskWhereFun = (taskEntryId) => { return taskEntryId == taskType; };
                        break;
                    case "垃圾箱房":
                        taskType = ProfileTaskEntryTypeEnum.GarbageBox.GetIntValue();

                        taskWhereFun = (taskEntryId) => { return taskEntryId == taskType; };
                        break;
                    case "压缩站":
                        taskType = ProfileTaskEntryTypeEnum.compressionStation.GetIntValue();

                        taskWhereFun = (taskEntryId) => { return taskEntryId == taskType; };
                        break;
                    case "绿化":
                        taskType = ProfileTaskEntryTypeEnum.Greening.GetIntValue();

                        taskWhereFun = (taskEntryId) => { return taskEntryId == taskType; };
                        break;
                    case "绿色账户小区":
                        taskType = ProfileTaskEntryTypeEnum.GreenResidential.GetIntValue();

                        taskWhereFun = (taskEntryId) => { return taskEntryId == taskType; };
                        break;
                    case "沿街垃圾收集设施":


                        var cesspoolTaskType = ProfileTaskEntryTypeEnum.cesspool.GetIntValue();
                        var WastebasketTaskType = ProfileTaskEntryTypeEnum.cesspool.GetIntValue();
                        var StreetTrashTaskType = ProfileTaskEntryTypeEnum.cesspool.GetIntValue();

                        taskWhereFun = (taskEntryId) =>
                        {
                            return taskEntryId == cesspoolTaskType ||
                                taskEntryId == WastebasketTaskType ||
                                taskEntryId == StreetTrashTaskType;
                        };

                        break;
                    case "环卫车辆":
                       

                        var machineCleanCarTaskType = ProfileTaskEntryTypeEnum.MachineCleanCar.GetIntValue();
                        var WashTheCarTaskType = ProfileTaskEntryTypeEnum.WashTheCar.GetIntValue();
                        var garbageTruckCarTaskType = ProfileTaskEntryTypeEnum.GarbageTruckCar.GetIntValue();
                        var flyingCarTaskType = ProfileTaskEntryTypeEnum.FlyingCar.GetIntValue();
                        var eightLadleCarTaskType = ProfileTaskEntryTypeEnum.EightLadleCar.GetIntValue();

                        taskWhereFun = (taskEntryId) =>
                        {
                            return taskEntryId == machineCleanCarTaskType ||
                                taskEntryId == WashTheCarTaskType ||
                                taskEntryId == garbageTruckCarTaskType ||
                                taskEntryId == flyingCarTaskType ||
                                taskEntryId == eightLadleCarTaskType;
                        };
                        break;
                    default:
                        break;
                }
                #endregion

                var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                                 where taskQ.PersonInChargeId == userId &&
                                  taskQ.DeliveryTime <= currentTime &&
                                  taskQ.CompletionTime >= currentTime &&
                                  taskQ.State == toAuditState
                                 select new
                                 {
                                     f_id = taskQ.F_Id
                                 };

                if (taskIsHave.Count() <= 0)
                {
                    return 0;
                }

                var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                                join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                                on taskQ.F_Id equals taskEntryQ.TaskId
                                where taskQ.PersonInChargeId == userId &&
                                taskQ.State == toAuditState
                                select new
                                {
                                    TaskId = taskQ.F_Id,
                                    TaskEntryId = taskEntryQ.F_Id,
                                    TaskEntryType = taskEntryQ.TaskEntryType
                                };

                return taskQuery.Select(d => d.TaskEntryType).Where(taskWhereFun).Count();

                //return ssq.Count(taskWhereFun);
            }



            return 1;
        }

        /// <summary>
        /// 整体任务获取根据参数返回对应任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entryId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetTask(string userId, string entryId, string typeId, ProfileTaskStateEnum taskState, out int ProfileTaskEntryType)
        {

            using (var db = new RepositoryBase().BeginTrans())
            {
                //暂时这么写 移动端需要这样 ，日后整理
                ProfileTaskEntryType = -1;

                var entryQuery = db.IQueryable<ProfileScoreCriteria_EntryEntity>().Where(d => d.SEntryId == entryId);

                if (entryQuery.Count() <= 0)
                {
                    throw new Exception("未找到大类!");
                }

                Contracts.ApiTaskDataEntryContracts[] result = null;

                var entryEntity = entryQuery.FirstOrDefault();

                switch (entryEntity.Name)
                {
                    case "道路":
                        result = this.GetWayTask(db, userId, typeId, taskState);
                        break;
                    case "公厕":
                        result = this.GetTandasTask(db, userId, typeId, taskState);
                        break;
                    case "垃圾箱房":
                        result = this.GetGarbageBoxTask(db, userId, typeId, taskState);
                        break;
                    case "压缩站":
                        result = this.GetcompressionStationTask(db, userId, typeId, taskState);
                        break;
                    case "绿化":
                        result = this.GetGreeningTask(db, userId, typeId, taskState);
                        break;
                    case "绿色账户小区":
                        result = this.GetGreenResidentialTask(db, userId, typeId, taskState);
                        break;
                    case "沿街垃圾收集设施":

                        //获取二级查看要的是哪个
                        var typeEntity = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == typeId).FirstOrDefault();
                        if (typeEntity == null)
                        {
                            throw new Exception("未找到对应中类请检查typeId参数!");
                        }
                        if (typeEntity.Name.Equals("倒粪站 小便池"))
                        {
                            result = this.GetCesspoolTask(db, userId, typeId, taskState);
                        }
                        else if (typeEntity.Name.Equals("废物箱"))
                        {
                            result = this.GetWastebasketTask(db, userId, typeId, taskState);
                        }
                        else if (typeEntity.Name.Equals("沿街垃圾桶"))
                        {
                            result = this.GetStreetTrashTask(db, userId, typeId, taskState);
                        }

                        break;
                    case "环卫车辆":
                        var typecEntity = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == typeId).FirstOrDefault();

                        ProfileTaskEntryType = ProfileTaskEntryTypeEnum.Car.GetIntValue();

                        if (typecEntity == null)
                        {
                            throw new Exception("未找到对应中类请检查typeId参数!");
                        }

                        if (typecEntity.Name.Equals("机扫车"))
                        {
                            result = this.GetMachineCleanCarTask(db, userId, typeId, taskState);
                        }
                        else if (typecEntity.Name.Equals("冲洗车"))
                        {
                            result = this.GetStreetTrashTask(db, userId, typeId, taskState);
                        }
                        else if (typecEntity.Name.Equals("清运车"))
                        {
                            result = this.GetMachineCleanCarTask(db, userId, typeId, taskState);
                        }
                        else if (typecEntity.Name.Equals("电动机具"))
                        {
                            result = this.GetDDCarTask(db, userId, typeId, taskState);
                        }
                        break;
                    default:
                        break;
                }

                return result;

            }
        }

        /// <summary>
        /// 获取道路清扫任务
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetWayTask(IRepositoryBase db, string UserId, string typeId, ProfileTaskStateEnum taskState)
        {

            var classifyQuery = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == typeId);

            if (classifyQuery.Count() <= 0)
            {
                return null;
            }

            var classifyEntity = classifyQuery.FirstOrDefault();
            int wayGray = -1;

            switch (classifyEntity.Name)
            {
                case "特级道路":
                    wayGray = -1;
                    break;
                case "一级道路":
                    wayGray = (int)ProfileWayGradeEnum.一级道路;
                    break;
                case "二级道路":
                    wayGray = (int)ProfileWayGradeEnum.二级道路;
                    break;
                case "三级道路":
                    wayGray = (int)ProfileWayGradeEnum.三级及其它;
                    break;
                default:
                    break;
            }

            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.Way.GetIntValue();


            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == UserId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == UserId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查道路任务

            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == UserId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };


            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == UserId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }

            var dataQuery = from taskq in taskQuery
                            join wayData in db.IQueryable<ProfileSanitationWayEntity>()
                            on taskq.EntryDataId equals wayData.F_Id
                            where wayData.WayGrade == wayGray
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                Origin = wayData.Origin,
                                Destination = wayData.Destination,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = wayData.StreetId
                            };

            var StreetQuery = from dataQ in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataQ.TaskId,
                                  TaskEntryId = dataQ.TaskEntryId,
                                  Origin = dataQ.Origin,
                                  Destination = dataQ.Destination,
                                  DeliveryTime = dataQ.DeliveryTime,
                                  CompletionTime = dataQ.CompletionTime,
                                  StreetId = dataQ.StreetId,
                                  StreetName = streetQ.StreetName
                              };

            var result = StreetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "道路清扫",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                Address = d.Origin + "--" + d.Destination,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                type = taskType
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;

        }

        /// <summary>
        /// 获取环卫公厕任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetTandasTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            var classifyQuery = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == typeId);

            if (classifyQuery.Count() <= 0)
            {
                return null;
            }

            var classifyEntity = classifyQuery.FirstOrDefault();
            int tandasGray = -1;

            switch (classifyEntity.Name)
            {
                case "一类公厕":
                    tandasGray = (int)ProfileTandasGradeEnum.一类公厕;
                    break;
                case "二类公厕":

                    tandasGray = (int)ProfileTandasGradeEnum.二类公厕;
                    break;
                case "三类公厕":
                    tandasGray = (int)ProfileTandasGradeEnum.三类公厕;
                    break;

                default:
                    break;
            }

            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.Tandas.GetIntValue();


            #region 查找任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }


            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }
            #endregion

            var dataQuery = from taskq in taskQuery
                            join tandasq in db.IQueryable<ProfileSanitationTandasEntity>()
                            on taskq.EntryDataId equals tandasq.F_Id
                            where tandasq.Grade == tandasGray
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = tandasq.StreetId,
                                Address = tandasq.Address
                            };

            var streetQuery = from dataq in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataq.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataq.TaskId,
                                  TaskEntryId = dataq.TaskEntryId,
                                  DeliveryTime = dataq.DeliveryTime,
                                  CompletionTime = dataq.CompletionTime,
                                  StreetId = dataq.StreetId,
                                  Address = dataq.Address,
                                  StreetName = streetQ.StreetName
                              };

            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "公厕",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                Address = d.Address,
                type = taskType
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 获取垃圾厢房任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetGarbageBoxTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            var classifyQuery = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == typeId);

            if (classifyQuery.Count() <= 0)
            {
                return null;
            }

            var classifyEntity = classifyQuery.FirstOrDefault();
            int gray = -1;

            switch (classifyEntity.Name)
            {
                case "沿街箱房":
                    gray = (int)ProfileSanitationGarbageBoxTypeEnum.沿街压缩站;
                    break;
                case "小区内箱房":
                    gray = (int)ProfileSanitationGarbageBoxTypeEnum.小区压缩站;
                    break;
                default:
                    break;
            }

            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.GarbageBox.GetIntValue();

            #region 查找任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }


            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }
            #endregion

            var dataQuery = from taskq in taskQuery
                            join garbageBoxq in db.IQueryable<ProfileSanitationGarbageBoxEntity>()
                            on taskq.EntryDataId equals garbageBoxq.F_Id
                            where garbageBoxq.GarbageBoxType == gray
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = garbageBoxq.StreetId,
                                Address = garbageBoxq.Address
                            };

            var streetQuery = from dataq in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataq.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataq.TaskId,
                                  TaskEntryId = dataq.TaskEntryId,
                                  DeliveryTime = dataq.DeliveryTime,
                                  CompletionTime = dataq.CompletionTime,
                                  StreetId = dataq.StreetId,
                                  Address = dataq.Address,
                                  StreetName = streetQ.StreetName
                              };

            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "垃圾厢房",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                Address = d.Address,
                type = taskType
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 获取压缩站任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetcompressionStationTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            var classifyQuery = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == typeId);

            if (classifyQuery.Count() <= 0)
            {
                return null;
            }

            var classifyEntity = classifyQuery.FirstOrDefault();
            int gray = -1;

            switch (classifyEntity.Name)
            {
                case "沿街压缩站":
                    gray = (int)ProfileCompressionStationType.沿街压缩站;
                    break;
                case "小区压缩站":
                    gray = (int)ProfileCompressionStationType.小区压缩站;
                    break;
                default:
                    break;
            }

            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.compressionStation.GetIntValue();

            #region 查找任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }


            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }
            #endregion


            var dataQuery = from taskq in taskQuery
                            join compBoxq in db.IQueryable<ProfileSanitationCompressionStationEntity>()
                            on taskq.EntryDataId equals compBoxq.F_Id
                            where compBoxq.CompType == gray
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = compBoxq.StreetId,
                                Address = compBoxq.Address
                            };

            var streetQuery = from dataq in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataq.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataq.TaskId,
                                  TaskEntryId = dataq.TaskEntryId,
                                  DeliveryTime = dataq.DeliveryTime,
                                  CompletionTime = dataq.CompletionTime,
                                  StreetId = dataq.StreetId,
                                  Address = dataq.Address,
                                  StreetName = streetQ.StreetName
                              };

            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "压缩站",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                Address = d.Address,
                type = taskType
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 获取沿街绿化任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetGreeningTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {


            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.Greening.GetIntValue();

            #region 查找任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }


            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }
            #endregion


            var dataQuery = from taskq in taskQuery
                            join GreeningBoxq in db.IQueryable<ProfileSanitationGreeningEntity>()
                            on taskq.EntryDataId equals GreeningBoxq.F_Id
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = GreeningBoxq.StreetId,
                                Address = GreeningBoxq.Address,
                                Origin = GreeningBoxq.Origin,
                                Destination = GreeningBoxq.Destination
                            };

            var streetQuery = from dataq in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataq.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataq.TaskId,
                                  TaskEntryId = dataq.TaskEntryId,
                                  DeliveryTime = dataq.DeliveryTime,
                                  CompletionTime = dataq.CompletionTime,
                                  StreetId = dataq.StreetId,
                                  Address = dataq.Address,
                                  Origin = dataq.Origin,
                                  Destination = dataq.Destination,
                                  StreetName = streetQ.StreetName
                              };

            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "绿化",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                Address = d.Address,
                type = taskType,
                Name = d.Origin + "--" + d.Destination
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 获取绿色账户小区任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetGreenResidentialTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.GreenResidential.GetIntValue();

            #region 查找任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }


            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }
            #endregion


            var dataQuery = from taskq in taskQuery
                            join GreeninRegBoxq in db.IQueryable<ProfileSanitationGreenResidentialEntity>()
                            on taskq.EntryDataId equals GreeninRegBoxq.F_Id
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = GreeninRegBoxq.StreetId,
                                Address = GreeninRegBoxq.Address
                            };

            var streetQuery = from dataq in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataq.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataq.TaskId,
                                  TaskEntryId = dataq.TaskEntryId,
                                  DeliveryTime = dataq.DeliveryTime,
                                  CompletionTime = dataq.CompletionTime,
                                  StreetId = dataq.StreetId,
                                  Address = dataq.Address,
                                  StreetName = streetQ.StreetName
                              };

            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "绿色账户小区",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                Address = d.Address,
                type = taskType
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 获取倒粪站 小便池污水池任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetCesspoolTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.cesspool.GetIntValue();

            #region 查找任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }


            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };

            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                EntryDataId = taskEntryQ.EntryDataId
                            };
            }
            #endregion


            var dataQuery = from taskq in taskQuery
                            join cesspq in db.IQueryable<ProfileSanitationCesspoolEntity>()
                            on taskq.EntryDataId equals cesspq.F_Id
                            select new
                            {
                                TaskId = taskq.TaskId,
                                TaskEntryId = taskq.TaskEntryId,
                                DeliveryTime = taskq.DeliveryTime,
                                CompletionTime = taskq.CompletionTime,
                                StreetId = cesspq.StreetId,
                                Address = cesspq.Address
                            };

            var streetQuery = from dataq in dataQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on dataq.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = dataq.TaskId,
                                  TaskEntryId = dataq.TaskEntryId,
                                  DeliveryTime = dataq.DeliveryTime,
                                  CompletionTime = dataq.CompletionTime,
                                  StreetId = dataq.StreetId,
                                  Address = dataq.Address,
                                  StreetName = streetQ.StreetName
                              };

            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "沿街垃圾收集设施倒粪站小便池",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                Address = d.Address,
                type = taskType
            }).ToArray();

            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }

            return result;
        }

        /// <summary>
        /// 获取f废物箱任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetWastebasketTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //废纸箱
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.Wastebasket.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "沿街垃圾收集设废物箱",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取沿街垃圾箱任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetStreetTrashTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {

            //废纸箱
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.StreetTrash.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "沿街垃圾收集设废物箱",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取机扫车任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetMachineCleanCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //机扫车
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.MachineCleanCar.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "环卫作业车辆 机扫车",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType,
                IsHaveCarWorkItemSelect = true
            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取清扫车任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetWashTheCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //冲洗车
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.WashTheCar.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "环卫作业车辆 冲洗车",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType,
                IsHaveCarWorkItemSelect = true

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取垃圾清运车任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetGarbageTruckCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //冲洗车
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.GarbageTruckCar.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "环卫作业车辆 垃圾清运车",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType,
                IsHaveCarWorkItemSelect = true

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取飞行保洁车任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetFlyingCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //冲洗车
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.FlyingCar.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "环卫作业车辆 飞行保洁车",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType,
                IsHaveCarWorkItemSelect = true

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取四轮八桶车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetEightLadleCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //冲洗车
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.EightLadleCar.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "环卫作业车辆 四轮八桶车",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType,
                IsHaveCarWorkItemSelect = true

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 获取机扫车任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeId"></param>
        /// <param name="taskState"></param>
        /// <returns></returns>
        public Contracts.ApiTaskDataEntryContracts[] GetDDCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState)
        {
            //冲洗车
            DateTime currentTime = DateTime.Now;
            int toAuditState = taskState.GetIntValue();
            int taskType = ProfileTaskEntryTypeEnum.EightLadleCar.GetIntValue();
            int taskType2 = ProfileTaskEntryTypeEnum.FlyingCar.GetIntValue();

            #region 获取任务
            //寻找当前外勤人员是否存在要完成的任务
            var taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskIsHave = from taskQ in db.IQueryable<ProfileTaskEntity>()
                             where taskQ.PersonInChargeId == userId &&
                              taskQ.DeliveryTime <= currentTime &&
                              taskQ.CompletionTime >= currentTime &&
                              taskQ.State == toAuditState
                             select new
                             {
                                 f_id = taskQ.F_Id
                             };
            }

            if (taskIsHave.Count() <= 0)
            {
                throw new Exception("此用户当前未发现任何任务!");
            }

            //查找废纸箱
            var taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            if (taskState == ProfileTaskStateEnum.ToAudit)
            {
                taskQuery = from taskQ in db.IQueryable<ProfileTaskEntity>()
                            join taskEntryQ in db.IQueryable<ProfileTaskEntryEntity>()
                            on taskQ.F_Id equals taskEntryQ.TaskId
                            where taskQ.PersonInChargeId == userId &&
                            taskQ.State == toAuditState &&
                            taskQ.DeliveryTime <= currentTime &&
                            taskQ.CompletionTime >= currentTime &&
                            taskEntryQ.TaskEntryType == taskType
                            select new
                            {
                                TaskId = taskQ.F_Id,
                                TaskEntryId = taskEntryQ.F_Id,
                                DeliveryTime = taskQ.DeliveryTime,
                                CompletionTime = taskQ.CompletionTime,
                                StreetId = taskEntryQ.StreetId,
                                BYMESS2 = taskEntryQ.BYMESS2,
                                BYMESS3 = taskEntryQ.BYMESS3
                            };
            }
            #endregion

            var streetQuery = from taskQ in taskQuery
                              join streetQ in db.IQueryable<ProfileStreetEntity>()
                              on taskQ.StreetId equals streetQ.F_Id
                              select new
                              {
                                  TaskId = taskQ.TaskId,
                                  TaskEntryId = taskQ.TaskEntryId,
                                  DeliveryTime = taskQ.DeliveryTime,
                                  CompletionTime = taskQ.CompletionTime,
                                  StreetId = taskQ.StreetId,
                                  BYMESS2 = taskQ.BYMESS2,
                                  BYMESS3 = taskQ.BYMESS3,
                                  StreetName = streetQ.StreetName
                              };


            var result = streetQuery.Select(d => new Contracts.ApiTaskDataEntryContracts()
            {
                Title = "环卫作业车辆 电动机具",
                TaskId = d.TaskId,
                TaskEntryId = d.TaskEntryId,
                CompletionTime = (DateTime)d.DeliveryTime,
                DeliveryTime = d.CompletionTime,
                StreetId = d.StreetId,
                StreetName = d.StreetName,
                NotFiexdInfo = d.BYMESS2,
                IsFixedPoint = false,
                IsPerfect = d.BYMESS3 == null ? false : (bool)d.BYMESS3,
                type = taskType,
                IsHaveCarWorkItemSelect = true

            }).ToArray();


            foreach (var item in result)
            {
                item.IsComplete = IsHaveDeducIns(db, item.TaskEntryId) ? 1 : 0;
            }


            return result;
        }

        /// <summary>
        /// 是否存在上传记录
        /// </summary>
        /// <param name="taskEntryId"></param>
        /// <returns></returns>
        public bool IsHaveDeducIns(IRepositoryBase db, string taskEntryId)
        {
            return db.IQueryable<ProfileDeducInsEntity>().Count(d => d.TaskEntry_Id == taskEntryId) > 0 ? true : false;
        }



    }
}