using Nfine.WebApi.Contracts;
using Nfine.WebApi.Data.Enums;
using Nfine.WebApi.Enums;
using Nfine.WebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nfine.WebApi.Controllers
{

    public class TaskController : ApiController
    {
        private Code.Task.ITask code = new Code.Task.TaskCode();

        [HttpPost]
        [HttpGet]
        public IHttpActionResult Get(string userId, string entryId, string typeId)
        {
            int ProfileTaskEntryType = -1;
            var data = code.GetTask(userId, entryId, typeId, NFine.Domain.Enums.ProfileTaskStateEnum.ToAudit, out ProfileTaskEntryType);

            int isCarTask = 0;

            //越加约乱 移动端那边要这样返回数据，日后整理此处 ，这样写太垃圾代码
            switch (ProfileTaskEntryType)
            {
                case (int)NFine.Domain.Enums.ProfileTaskEntryTypeEnum.Car:
                    isCarTask = 1;
                    break;
                default:
                    break;
            }

            var result = ApiBackParameter<Contracts.ApiTaskDataEntryContracts[]>.Get((api) =>
            {
                if (data == null)
                {
                    api.StatusCode = StatusCodeEnum.失败.GetIntValue();
                    api.Data = data;

                    //暂时这样做，移动端要这样的数据
                    api.Message = isCarTask.ToString();
                }
                else
                {
                    api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                    api.Data = data;

                    //暂时这样做，移动端要这样的数据
                    api.Message = isCarTask.ToString();
                }

            });

            return Ok(result);
        }


        [HttpPost]
        [HttpGet]
        [Route("api/Task/CompletedTask")]
        public IHttpActionResult GetCompletedTask(string userId, string entryId, string typeId)
        {
            int ProfileTaskEntryType = -1;
            var data = code.GetTask(userId, entryId, typeId, NFine.Domain.Enums.ProfileTaskStateEnum.HavePutAnEndTo, out ProfileTaskEntryType);


            int isCarTask = 0;

            //越加约乱 移动端那边要这样返回数据，日后整理此处 ，这样写太垃圾代码
            switch (ProfileTaskEntryType)
            {
                case (int)NFine.Domain.Enums.ProfileTaskEntryTypeEnum.Car:
                    isCarTask = 1;
                    break;
                default:
                    break;
            }

            var result = ApiBackParameter<Contracts.ApiTaskDataEntryContracts[]>.Get((api) =>
            {
                if (data == null)
                {
                    api.StatusCode = StatusCodeEnum.失败.GetIntValue();
                    api.Data = data;

                    //暂时这样做，移动端要这样的数据
                    api.Message =isCarTask.ToString();
                }
                else
                {
                    api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                    api.Data = data;


                    //暂时这样做，移动端要这样的数据
                    api.Message = isCarTask.ToString();
                }

            });

            return Ok(result);
        }
    }
}
