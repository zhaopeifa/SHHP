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
    public class DeductController : ApiController
    {
        private Code.Deduct.IDeduct code = new Code.Deduct.Deduct();

        [HttpGet]
        [HttpPost]
        public IHttpActionResult Get(string taskEntryId)
        {
            var data = code.GetDeductDetails(taskEntryId);

            var result = ApiBackParameter<ApiDeductAccordingContracts[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult Get(string taskEntryId, string SCNormId)
        {
            var data = code.GetDeductDetails(taskEntryId, SCNormId);

            var result = ApiBackParameter<ApiDeductAccordingContracts[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        /// <summary>
        /// 完善点位信息
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [Route("api/PerfectFixedInfo")]
        public IHttpActionResult PostFixedPoint(string taskEntry, string noFiexInfo)
        {
            bool state = code.PerfectFixedPoint(taskEntry, noFiexInfo);

            var result = ApiBackParameter<bool>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = state;
            });

            return Ok(result);
        }


        [HttpGet]
        [HttpPost]
        public IHttpActionResult Post(ApiDeductUploadContracts entity)
        {
            bool state = this.code.InsertDeductIns(entity);
            var result = ApiBackParameter<bool>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = state;
            });

            return Ok(result);
        }

        [HttpGet]
        [HttpPost]
        [Route("api/outDeduct")]
        public IHttpActionResult oldPost(ApiDeductUploadContracts entity)
        {
            string data = code.oudInsertDeductIns(entity);

            var result = ApiBackParameter<string>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult Put(string deducInsId, ApiDeductUploadContracts entity)
        {
            string data = code.UpdateDeductIns(deducInsId, entity);

            var result = ApiBackParameter<string>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult Delete(string deducInsId)
        {
            bool state = code.DeleteDeductIns(deducInsId);

            var result = ApiBackParameter<bool>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = state;
            });

            return Ok(result);
        }

        //[HttpPost]
        //[HttpGet]
        //public IHttpActionResult GetDeductId()
        //{
        //    string data = Guid.NewGuid().ToString();

        //    var result = ApiBackParameter<string>.Get((api) =>
        //    {
        //        api.StatusCode = StatusCodeEnum.成功.GetIntValue();
        //        api.Data = data;
        //    });

        //    return Ok(result);

        //}

        
        [HttpPost]
        [Route("api/Deduct/UploadImage")]
        public IHttpActionResult UploadImage(UploadImageContr model)
        {
            var data = this.code.UploadDeductImage(model.base64ImageCode, model.DeductId);

            var result = ApiBackParameter<string>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        public class UploadImageContr
        {
            public string base64ImageCode { get; set; }
            public string DeductId { get; set; }
        }
    }
}
