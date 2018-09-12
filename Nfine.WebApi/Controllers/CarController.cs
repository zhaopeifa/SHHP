using Nfine.WebApi.Contracts;
using Nfine.WebApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nfine.WebApi.Controllers
{
    public class CarController : ApiController
    {
        Code.Car.ICar Code = new Code.Car.Car();

        /// <summary>
        /// 获取车辆工作明细 根据车辆作业班次
        /// 接口地址:api/Car/GetCarWorItemByWorkShift 
        /// 参数:workShift 工作明细名称  要完全名称
        /// </summary>
        /// <param name="workShift"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetCarWorItemByWorkShift")]
        public IHttpActionResult GetWorkItemByCarWorkShift(string workShift)
        {
            var data = Code.GetWorkItem(CarWhereType.WorkShift, workShift);

            var result = ApiBackParameter<ApiCarWorkItem[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        /// <summary>
        /// 获取车辆工作明细  根据车牌号
        /// 接口地址:api/Car/GetCarWorItemByCarId 
        /// 参数 CarId 车牌号
        /// </summary>
        /// <param name="CarId"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetCarWorItemByCarId")]
        public IHttpActionResult GetWorkItemByCarId(string CarId)
        {
            var data = Code.GetWorkItem(CarWhereType.CarId, CarId);

            var result = ApiBackParameter<ApiCarWorkItem[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }


        #region 获取车牌号

        /// <summary>
        /// 获取全部车牌号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetCarId")]
        public IHttpActionResult GetCarId()
        {
            var data = Code.GetCarId(null, string.Empty);


            var result = ApiBackParameter<ApiKeyValue<string, string>[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        /// <summary>
        /// 获取全部车牌号 模糊查询
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetCarId")]
        public IHttpActionResult GetCarId(string keyWord)
        {
            var data = Code.GetCarId(null, keyWord);


            var result = ApiBackParameter<ApiKeyValue<string, string>[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        /// <summary>
        /// 获取全部车牌号 
        /// 带分页的 和模糊查询的
        /// 参数:pageSize 页大小 PageIndex 当前显示页  keyWord 模糊查询车牌号
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetCarId")]
        public IHttpActionResult GetCarId(int pageSize, int PageIndex, string keyWord = null)
        {
            Nfine.WebApi.Contracts.ApiPagination pagination = null;
            pagination = new Contracts.ApiPagination()
            {
                page = (int)PageIndex,
                rows = (int)pageSize
            };


            var data = new ApiPaginationData<ApiKeyValue<string, string>[]>()
            {
                Data = Code.GetCarId(pagination, keyWord),
                Pagination = pagination
            };

            var result = ApiBackParameter<ApiPaginationData<ApiKeyValue<string, string>[]>>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        #endregion

        #region 获取作业班次

        /// <summary>
        /// 获取所有 作业班次
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetWorkShift")]
        public IHttpActionResult GetWorkShift()
        {

            var data = Code.GetWorkShift(null, null);

            var result = ApiBackParameter<string[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        /// <summary>
        /// 获取作业班次 带模糊查询
        /// 参数:keyWord 模糊查询作业班次
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetWorkShift")]
        public IHttpActionResult GetWorkShift(string keyWord)
        {

            var data = Code.GetWorkShift(null, keyWord);

            var result = ApiBackParameter<string[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        /// <summary>
        /// 获取作业班次 带分页模糊查询 
        /// 参数 :pageSize 页大小  pageIndex:当前页 keyWord:模糊查询作业班次
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/Car/GetWorkShift")]
        public IHttpActionResult GetWorkShift(int pageSize, int pageIndex, string keyWord = null)
        {
            Nfine.WebApi.Contracts.ApiPagination pagination = null;

            pagination = new Contracts.ApiPagination()
            {
                page = (int)pageIndex,
                rows = (int)pageSize
            };

            var data = new ApiPaginationData<string[]>()
            {
                Data = Code.GetWorkShift(pagination, keyWord),
                Pagination = pagination

            };

            var result = ApiBackParameter<ApiPaginationData<string[]>>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        #endregion

    }
}
