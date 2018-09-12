﻿using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NFine.Web.Areas.SystemManage.Controllers
{
    public class AmenitiesResidentialController : ControllerBase
    {
        private ProfileAmenitiesResidentialApp App = new ProfileAmenitiesResidentialApp();
        private UserApp userApp = new UserApp();
        private ProfileAmenitiesMainWay_ResidentialApp ARApp = new ProfileAmenitiesMainWay_ResidentialApp();


        [HttpGet]
        [HandlerAuthorize(false)]
        public override System.Web.Mvc.ActionResult Index(string F_Id)
        {
            return base.Index(F_Id);
        }

        [HttpGet]
        [HandlerAuthorize(false)]
        public override ActionResult Form()
        {
            return base.Form();
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword, string cityId, string countyId, string projectId, string streetId)
        {
            var data = new
            {
                rows = App.GetList(pagination, keyword, projectId, streetId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ProfileAmenitiesResidentialEntity Entity, string keyValue, string mainWayIdsStr)
        {
            string[] mainWayIds = mainWayIdsStr.Split(',');
            App.SubmitForm(Entity, keyValue, mainWayIds);
            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAuthorize(false)]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            App.DeleteForm(keyValue);
            return Success("删除成功。");
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = App.GetForm(keyValue);
            if (data.F_LastModifyUserId != null)
            {
                var data1 = userApp.GetForm(data.F_LastModifyUserId);

                if (data1 != null)
                {
                    data.F_LastModifyUserId = data1.F_RealName;
                }
            }
            if (data.F_CreatorUserId != null)
            {
                var data2 = userApp.GetForm(data.F_CreatorUserId);
                if (data2 != null)
                {
                    data.F_CreatorUserId = data2.F_RealName;
                }

            }
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMainWayIds(string keyValue)
        {
            string sql = "SELECT * FROM ProfileAmenitiesMainWay_Residential WHERE ResidentialId='" + keyValue + "'";
            string[] ids = ARApp.FildSql(sql).Select(d => d.MainWayId).ToArray();
            return Content(ids.ToJson());
        }
    }
}