using NFine.Application.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Code;

namespace NFine.Web.Controllers
{
    [HandlerLogin]
    public class DefaultController : Controller
    {

        private ProfileTaskApp taskApp = new ProfileTaskApp();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取任务数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTaskCount()
        {
            //out int notToSendCount, out int toAuditCount, out int havePutAnEndToCount, out int theCancellationCounte
            int notToSendCount, toAuditCount, havePutAnEndToCount, theCancellationCounte;
            taskApp.GetTaskCount(out notToSendCount, out toAuditCount, out havePutAnEndToCount, out theCancellationCounte);


            var data = new
            {
                notToSendCount = notToSendCount,
                toAuditCount = toAuditCount,
                havePutAnEndToCount = havePutAnEndToCount,
                theCancellationCounte = theCancellationCounte
            };

            return Content(data.ToJson());
        }
    }
}
