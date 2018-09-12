﻿using Nfine.WebApi.Contracts;
using Nfine.WebApi.Data.Enums;
using Nfine.WebApi.Enums;
using NFine.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nfine.WebApi.Controllers
{
    public class CheckingPointController : ApiController
    {
        Code.CheckingPoint.ICheckingPoint code = new Code.CheckingPoint.CheckingPointCode();

        [HttpGet]
        [Route("api/CheckingPoint/GetEntry")]
        public IHttpActionResult GetEntry(string ProjectId)
        {
            var checkPoints = code.GetCheckingPoint(ProjectId);

            var result = ApiBackParameter<List<ApiCheckingPointContracts>>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = checkPoints;
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("api/CheckingPoint/GetEntry")]
        public IHttpActionResult GetEntry(string userId,string ProjectId)
        {
            var checkPoints = code.GetCheckingPointHavTaskCount(userId, ProjectId);

            var result = ApiBackParameter<List<ApiCheckingPointContracts>>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = checkPoints;
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("api/CheckingPoint/GetType")]
        public IHttpActionResult GetClassify(string entryId)
        {

            var data = code.GetCheckingPointClassification(entryId);

            var result = ApiBackParameter<List<ApiCheckingPointTypeContracts>>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }
    }
}
