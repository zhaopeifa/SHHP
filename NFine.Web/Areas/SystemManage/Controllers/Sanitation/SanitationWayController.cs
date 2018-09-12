using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Code.Web;
using NFine.Domain.Entity.SystemManage;
using NFine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NFine.Web.Areas.SystemManage.Controllers
{
    public class SanitationWayController : ControllerBase
    {
        private ProfileSanitationWayApp wayApp = new ProfileSanitationWayApp();
        private UserApp userApp = new UserApp();
        private ProfileStreetApp StreetApp = new ProfileStreetApp();
        private ProfileMainWayApp MainWayApp = new ProfileMainWayApp();

        private static string _temporaryFilesDiskPath;
        private static string TemporaryFilesDiskPath
        {
            get
            {
                if (string.IsNullOrEmpty(_temporaryFilesDiskPath))
                { _temporaryFilesDiskPath = System.Configuration.ConfigurationManager.AppSettings["TemporaryFilesDiskPath"]; }
                return _temporaryFilesDiskPath;
            }
        }

        [HttpGet]
        [HandlerAuthorize(false)]
        public override ActionResult Index(string F_Id)
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
        [HandlerAuthorize(false)]
        public ActionResult Import()
        {
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword, string cityId, string countyId, string projectId, string streetId)
        {
            var data = new
            {
                rows = wayApp.GetList(pagination, keyword, projectId, streetId),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ProfileSanitationWayEntity WayEntity, string keyValue)
        {
            //SummitImport(WayEntity.CityId, WayEntity.CountyId, WayEntity.ProjectId);

            wayApp.SubmitForm(WayEntity, keyValue);
            return Success("操作成功。");
        }

        ///// <summary>
        ///// 数据导入
        ///// </summary>
        ///// <param name="CityId"></param>
        ///// <param name="CountyId"></param>
        ///// <param name="ProjectId"></param>
        ///// <param name="isRename"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[HandlerAjaxOnly]
        //[ValidateAntiForgeryToken]
        //public ActionResult SummitImport(string CityId, string CountyId, string projectId, int isRename = 1)
        //{
        //    projectId = "dd1bbf6b-bcea-4cad-851b-1be4adf71860";

        //    string path = @"C:\Users\zps\Desktop\聚力环境测评系统样版\聚力环境测评系统样版\附件6、虹口市容点位\";
        //    string fileName = "Book1.xlsx";
        //    #region 导出

        //    if (FileHelper.IsExistFile(Path.Combine(path, fileName)))
        //    {


        //        using (ExcelHelper exHelp = new ExcelHelper(Path.Combine(path, fileName)))
        //        {
        //            var datatable = exHelp.ExcelToDataTable(fileName, true);

        //            ProfileAmenitiesConstructionSiteEntity[] models = new ProfileAmenitiesConstructionSiteEntity[datatable.Rows.Count];
        //            ProfileAmenitiesConstructionSiteEntity model;

        //            for (int i = 0; i < datatable.Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    var jdName = datatable.Rows[i]["街道"].ToString();
        //                    var fCode = datatable.Rows[i]["序号"].ToString();
        //                    var address = datatable.Rows[i]["地址"].ToString();
        //                    var name = datatable.Rows[i]["工地名称"].ToString();



        //                    var StreetNamekey = StreetApp.GetDictionary(d => d.StreetName == jdName)[0].Key;

        //                    var streetModel = StreetApp.GetForm(StreetNamekey);

        //                    model = new ProfileAmenitiesConstructionSiteEntity()
        //                    {
        //                        CityId = streetModel.CityId,
        //                        CountyId = streetModel.CountyId,
        //                        StreetId = streetModel.F_Id,
        //                        Address = address,
        //                        F_EnCode = fCode,
        //                        SiteName = name,
        //                        ProjectId = projectId
        //                    };

        //                    models[i] = model;
        //                }
        //                catch
        //                {

        //                }
        //            }
        //            var thapp = new ProfileAmenitiesConstructionSiteApp();
        //            foreach (var item in models)
        //            {
        //                try
        //                {
        //                    if (item == null)
        //                        continue;
        //                    thapp.SubmitForm(item, string.Empty);
        //                }
        //                catch
        //                {

        //                }

        //            }

        //        }
        //    }
        //    #endregion


        //    return Success("ss");
        //}

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="countyId"></param>
        /// <param name="ProjectId"></param>
        /// <param name="isReName"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitImprot(string cityId, string countyId, string ProjectId, int isReName = 1)
        {
            var file = Request.Files[0];

            string path = TemporaryFilesDiskPath;
            string fileName = Guid.NewGuid().ToString() + FileHelper.GetExtension(file.FileName);

            string filePah = Path.Combine(path, fileName);

            if (!FileHelper.IsExistDirectory(path))
            {
                FileHelper.CreateDirectory(path);
            }

            try
            {
                //将上传文件保存
                file.SaveAs(filePah);

                if (FileHelper.IsExistFile(filePah))
                {
                    using (ExcelHelper exHelp = new ExcelHelper(filePah))
                    {
                        var datatable = exHelp.ExcelToDataTable(fileName, true);

                        ProfileSanitationWayEntity wayEntity = null;

                        string streetName = string.Empty;
                        string wayName = string.Empty;
                        string mainWayName = string.Empty;
                        string wayOrigin = string.Empty;
                        string wayDestination = string.Empty;
                        string wayGrade = string.Empty;

                        #region 每行填充数据

                        for (int i = 0; i < datatable.Rows.Count; i++)
                        {
                            wayEntity = new ProfileSanitationWayEntity();


                            try
                            {
                                streetName = datatable.Rows[i]["街道"].ToString();
                                //"大连西路(四平路-密云路)" 格式
                                wayName = datatable.Rows[i]["路名"].ToString();

                                mainWayName = wayName.Split('(')[0];
                                wayOrigin = wayName.Split('(')[1].Split(')')[0].Split('-')[0];
                                wayDestination = wayName.Split('(')[1].Split(')')[0].Split('-')[1];

                                wayGrade = datatable.Rows[i]["类型"].ToString();

                                wayEntity.CityId = cityId;
                                wayEntity.CountyId = countyId;
                                wayEntity.ProjectId = ProjectId;
                                wayEntity.F_EnCode = int.Parse(datatable.Rows[i]["序号"].ToString());
                                wayEntity.StreetId = StreetApp.GetDictionary(d => d.StreetName == streetName)[0].Key;
                                wayEntity.MainWayId = MainWayApp.GetDictionary(d => d.MainWayName == mainWayName)[0].Key;
                                wayEntity.WayName = wayName;
                                wayEntity.Origin = wayOrigin;
                                wayEntity.Destination = wayDestination;
                                wayEntity.IsJointPartWay = false;
                            }
                            catch
                            {
                                continue;
                            }

                            switch (wayGrade)
                            {
                                case "一级":
                                    wayEntity.WayGrade = (int)ProfileWayGradeEnum.一级道路;
                                    break;
                                case "二级":
                                    wayEntity.WayGrade = (int)ProfileWayGradeEnum.二级道路;
                                    break;
                                case "三级及其它":
                                    wayEntity.WayGrade = (int)ProfileWayGradeEnum.三级及其它;
                                    break;
                                default:
                                    break;
                            }

                            wayApp.BatchSubmitFrom(wayEntity, null, (db, c) => { return db.F_EnCode == c.F_EnCode; });
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (FileHelper.IsExistFile(filePah))
                {
                    FileHelper.DeleteFile(filePah);
                }
            }

            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAuthorize(false)]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            wayApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }

        /// <summary>
        /// 获取道路等级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetEnableWayGradeGridJson()
        {

            List<object> list = new List<object>();

            foreach (int myCode in Enum.GetValues(typeof(NFine.Domain.Enums.ProfileWayGradeEnum)))
            {
                string strName = Enum.GetName(typeof(NFine.Domain.Enums.ProfileWayGradeEnum), myCode);//获取名称

                list.Add(new { id = myCode, text = strName });
            }
            return Content(list.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = wayApp.GetForm(keyValue);
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


    }
}