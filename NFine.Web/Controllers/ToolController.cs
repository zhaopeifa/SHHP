using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NFine.Web.Controllers
{
    /// <summary>
    /// 公共工具接口
    /// </summary>
    public class ToolController : Controller
    {
        //
        // GET: /Tool/

        [HttpPost]
        public ActionResult GetExcel2DataTable()
        {
            var file = Request.Files[0];

            return Content("yes");
        }

        [HttpGet]
        [HttpPost]
        public ActionResult GetNewGuid()
        {
            return Content(Guid.NewGuid().ToString());
        }

    }
}
