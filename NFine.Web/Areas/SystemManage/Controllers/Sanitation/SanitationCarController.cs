using NFine.Application.SystemManage;
using NFine.Code;
using NFine.Domain.Contracts;
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
    /// <summary>
    /// 环评-环卫-机扫车
    /// </summary>
    public class SanitationCarController : ControllerBase
    {
        private ProfileSanitationCarApp App = new ProfileSanitationCarApp();
        private UserApp userApp = new UserApp();

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
        public ActionResult GetGridJson(Pagination pagination, string keyword, int carType)
        {
            var data = new
            {
                rows = App.GetList((ProfileCarTypeEnum)carType, pagination, keyword),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult FormWorkItem()
        {
            return View();
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ProfileSanitationCarEntity entity, int carType, string keyValue, string workItem)
        {
            entity.CarType = carType;

            ProfileCarWorkItemContracts[] works = workItem.ToObject<ProfileCarWorkItemContracts[]>();

            App.SubmitForm(entity, keyValue, works);
            return Success("操作成功!");
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
        public ActionResult GetWorkItemGridJson(string carId)
        {
            var carWorkItems = this.App.GetCarWorkItem(carId);

            return Content(carWorkItems.ToJson());
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="CityId"></param>
        /// <param name="CountyId"></param>
        /// <param name="ProjectId"></param>
        /// <param name="isRename"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SummitCarImport(string CityId, string CountyId, string projectId, int isRename = 1)
        {
            projectId = "dd1bbf6b-bcea-4cad-851b-1be4adf71860";

            string path = @"C:\Users\zps\Desktop\聚力环境测评系统样版\聚力环境测评系统样版\";
            string fileName = "1车辆信息(1).xlsx";


            ProfileOperationsCompanyApp companyApp = new ProfileOperationsCompanyApp();

            #region 导出

            if (FileHelper.IsExistFile(Path.Combine(path, fileName)))
            {


                using (ExcelHelper exHelp = new ExcelHelper(Path.Combine(path, fileName)))
                {
                    var datatable = exHelp.ExcelToDataTable(fileName, true);



                    ProfileSanitationCarEntity[] models = new ProfileSanitationCarEntity[datatable.Rows.Count];
                    ProfileSanitationCarEntity model;

                    for (int i = 0; i < datatable.Rows.Count; i++)
                    {
                        try
                        {

                            var carTypeStr = datatable.Rows[i]["车辆类型"].ToString();
                            var carId = datatable.Rows[i]["车牌号"].ToString();
                            var f_EnCode = datatable.Rows[i]["自编号"].ToString();
                            var companyName = datatable.Rows[i]["作业公司"].ToString();
                            var workShift = datatable.Rows[i]["作业班次"].ToString();

                            int carTypeInt = 0;
                            string companyId = "";


                            if (carTypeStr == "机扫车")
                            {
                                carTypeInt = ProfileCarTypeEnum.MachineCleanCar.GetIntValue();
                            }
                            else if (carTypeStr == "冲洗车")
                            {
                                carTypeInt = ProfileCarTypeEnum.WashTheCar.GetIntValue();
                            }

                            companyId = companyApp.GetDictionary(d => d.CompanyName == companyName).FirstOrDefault().Key;

                            model = new ProfileSanitationCarEntity()
                            {
                                CarId = carId,
                                CarType = carTypeInt,
                                WorkShift = workShift,
                                CompanyId = companyId,
                                F_EnCode = f_EnCode
                            };


                            models[i] = model;
                        }
                        catch
                        {

                        }
                    }
                    var thapp = new ProfileAmenitiesConstructionSiteApp();
                    foreach (var item in models)
                    {
                        try
                        {
                            if (item == null)
                                continue;
                            App.SubmitForm(item, string.Empty);
                        }
                        catch
                        {

                        }

                    }

                }
            }
            #endregion


            return Success("ss");
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="CityId"></param>
        /// <param name="CountyId"></param>
        /// <param name="ProjectId"></param>
        /// <param name="isRename"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SummitCarWorkItemImport(string CityId, string CountyId, string projectId, int isRename = 1)
        {
            projectId = "dd1bbf6b-bcea-4cad-851b-1be4adf71860";

            string path = @"C:\Users\zps\Desktop\聚力环境测评系统样版\聚力环境测评系统样版\";
            string fileName = "1班次作业明细.xls";



            #region 导出

            if (FileHelper.IsExistFile(Path.Combine(path, fileName)))
            {


                using (ExcelHelper exHelp = new ExcelHelper(Path.Combine(path, fileName)))
                {
                    var datatable = exHelp.ExcelToDataTable(fileName, true);



                    ProfileSanitationCarWorkItemEntity[] models = new ProfileSanitationCarWorkItemEntity[datatable.Rows.Count];
                    ProfileSanitationCarWorkItemEntity model;

                    for (int i = 0; i < datatable.Rows.Count; i++)
                    {
                        try
                        {

                            var workShift = datatable.Rows[i]["作业班次"].ToString();
                            var xh = datatable.Rows[i]["序号"].ToString();
                            var workTime = datatable.Rows[i]["作业时间"].ToString();
                            var workName = datatable.Rows[i]["作业点名称"].ToString();
                            var workAddress = datatable.Rows[i]["作业点地址"].ToString();
                            var workNode = datatable.Rows[i]["备注"].ToString();



                            model = new ProfileSanitationCarWorkItemEntity()
                            {
                                Note = workNode,
                                WorkName = workName,
                                WorkAddress = workAddress,
                                WorkTime = workTime,
                                WorkShift = workShift,
                                Subscript = int.Parse(xh)
                            };


                            models[i] = model;
                        }
                        catch
                        {

                        }
                    }
                    var thapp = new ProfileAmenitiesConstructionSiteApp();
                    foreach (var item in models)
                    {
                        try
                        {
                            if (item == null)
                                continue;
                            App.SubmitWorkItemForm(item, string.Empty);
                        }
                        catch
                        {

                        }

                    }

                }
            }
            #endregion


            return Success("ss");
        }
    }
}