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
    public class ScoringCriteriaController : ApiController
    {
        Code.ScoringCriteria.IScoringCriteria code = new Code.ScoringCriteria.ScoringCriteriaCode();
        public IHttpActionResult Get(string typeId)
        {
            var data = code.GetScoringCriteria(typeId);

            var result = ApiBackParameter<ApiScoringCriteriaClassifyContracts[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }

        public IHttpActionResult Get(string taskEntryId, string typeId)
        {
            var data = code.GetScoringCriteriaAndRecord(taskEntryId, typeId);

            var result = ApiBackParameter<ApiScoringCriteriaClassifyContracts[]>.Get((api) =>
            {
                api.StatusCode = StatusCodeEnum.成功.GetIntValue();
                api.Data = data;
            });

            return Ok(result);
        }
    }
}
