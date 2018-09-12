using Nfine.WebApi.Contracts;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nfine.WebApi.Unti;
using System.Drawing;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;

namespace Nfine.WebApi.Code.Deduct
{
    public class Deduct : IDeduct
    {
        private static string _imageFolderPath;
        private static string _imgDomainPathURL;

        private static string imageFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(_imageFolderPath))
                { _imageFolderPath = System.Configuration.ConfigurationManager.AppSettings["imagePath"]; }
                return _imageFolderPath;
            }
        }

        public static string imgDomainPathURL
        {
            get
            {
                if (string.IsNullOrEmpty(_imgDomainPathURL))
                { _imgDomainPathURL = System.Configuration.ConfigurationManager.AppSettings["ImgDomainPathURL"]; }
                return _imgDomainPathURL;
            }
        }

        private JavaScriptSerializer js = new JavaScriptSerializer();

        /// <summary>
        /// 获取扣分记录
        /// </summary>
        /// <param name="taskEntryId"></param>
        /// <returns></returns>
        public Contracts.ApiDeductAccordingContracts[] GetDeductDetails(string taskEntryId)
        {
            var result = GeDeductDetails2ApiDeductAccordingContracts(d => d.TaskEntry_Id == taskEntryId);
            return result;
        }

        /// <summary>
        /// 获取扣分明细
        /// </summary>
        /// <param name="taskEntryId">子任务id</param>
        /// <param name="SCNormId">扣分明细Id</param>
        /// <returns></returns>
        public ApiDeductAccordingContracts[] GetDeductDetails(string taskEntryId, string SCNormId)
        {
            var result = GeDeductDetails2ApiDeductAccordingContracts(d => d.TaskEntry_Id == taskEntryId && d.SCNorm_Id == SCNormId);


            return result;
        }

        /// <summary>
        /// 上传添加扣分记录
        /// </summary>
        /// <param name="deductsInsEntity"></param>
        /// <returns></returns>
        public string oudInsertDeductIns(ApiDeductUploadContracts deductsInsEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {

                var sNormEntity = db.IQueryable<ProfileScireCriteria_NormEntity>().FirstOrDefault(d => d.SNormId == deductsInsEntity.SCNorm_Id);
                var sClassifyEntiy = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().FirstOrDefault(d => d.SClassifyId == sNormEntity.SClassifyId);
                var sTypeEntiy = db.IQueryable<ProfileScoreCriteria_TypeEntity>().FirstOrDefault(d => d.STypeId == sClassifyEntiy.STypeId);
                var sEntiyEntity = db.IQueryable<ProfileScoreCriteria_EntryEntity>().FirstOrDefault(d => d.SEntryId == sTypeEntiy.SEntryId);
                var userEntity = db.IQueryable<UserEntity>().FirstOrDefault(d => d.F_Id == deductsInsEntity.CreatorUserId);

                if (sNormEntity == null)
                    throw new Exception("未找到对应扣分标准!");

                ProfileDeducInsEntity dbDeductIndesEntry = new ProfileDeducInsEntity()
                {
                    DeducIns_Id = Guid.NewGuid().ToString(),
                    TaskEntry_Id = deductsInsEntity.TaskEntry_Id,
                    SCNorm_Id = deductsInsEntity.SCNorm_Id,
                    SCEntryName = sEntiyEntity.Name,
                    SCTypeName = sTypeEntiy.Name,
                    SCClassifyName = sClassifyEntiy.SClassifyName,
                    SCNormProjectName = sNormEntity.SNormProjectName,
                    SCNormStandardName = sNormEntity.SNormStandardName,
                    DeductionSeveral = deductsInsEntity.DeductionSeveral,
                    DeductionScore = deductsInsEntity.DeductionScore,
                    DeductionDescribe = deductsInsEntity.DeductionDescribe,
                    CreateTime = DateTime.Now,
                    CreatorUserId = deductsInsEntity.CreatorUserId,
                    CreatorUserName = userEntity.F_RealName,
                    SCNormIsDeduct = sNormEntity.IsDeduct
                };

                db.Insert<ProfileDeducInsEntity>(dbDeductIndesEntry);


                try
                {
                    db.Commit();

                    return dbDeductIndesEntry.DeducIns_Id;
                }
                catch
                {
                    return "-1";
                }
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="deducIns_Id">要修改的Id</param>
        /// <param name="deductsInsEntity">model</param>
        /// <returns></returns>
        public string UpdateDeductIns(string deducIns_Id, ApiDeductUploadContracts deductsInsEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var UpdateDeductIns = db.FindEntity<ProfileDeducInsEntity>(deducIns_Id);

                var sNormEntity = db.IQueryable<ProfileScireCriteria_NormEntity>().FirstOrDefault(d => d.SNormId == deductsInsEntity.SCNorm_Id);
                var sClassifyEntiy = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().FirstOrDefault(d => d.SClassifyId == sNormEntity.SClassifyId);
                var sTypeEntiy = db.IQueryable<ProfileScoreCriteria_TypeEntity>().FirstOrDefault(d => d.STypeId == sClassifyEntiy.STypeId);
                var sEntiyEntity = db.IQueryable<ProfileScoreCriteria_EntryEntity>().FirstOrDefault(d => d.SEntryId == sTypeEntiy.SEntryId);
                var userEntity = db.IQueryable<UserEntity>().FirstOrDefault(d => d.F_Id == deductsInsEntity.CreatorUserId);

                if (sNormEntity == null)
                    throw new Exception("未找到对应扣分标准!");


                UpdateDeductIns.SCNorm_Id = deductsInsEntity.SCNorm_Id;
                UpdateDeductIns.SCEntryName = sEntiyEntity.Name;
                UpdateDeductIns.SCTypeName = sTypeEntiy.Name;
                UpdateDeductIns.SCClassifyName = sClassifyEntiy.SClassifyName;
                UpdateDeductIns.SCNormProjectName = sNormEntity.SNormProjectName;
                UpdateDeductIns.SCNormStandardName = sNormEntity.SNormStandardName;
                UpdateDeductIns.DeductionSeveral = deductsInsEntity.DeductionSeveral;
                UpdateDeductIns.DeductionScore = deductsInsEntity.DeductionScore;
                UpdateDeductIns.DeductionDescribe = deductsInsEntity.DeductionDescribe;
                UpdateDeductIns.LastModifyTime = DateTime.Now;
                UpdateDeductIns.LastModifyUserId = deductsInsEntity.CreatorUserId;
                UpdateDeductIns.LastModifyUserName = userEntity.F_RealName;
                UpdateDeductIns.SCNormIsDeduct = sNormEntity.IsDeduct;

                db.Update(UpdateDeductIns);
            }

            return deducIns_Id;
        }

        /// <summary>
        /// 删除评分明细
        /// </summary>
        /// <param name="DeducIns_Id"></param>
        /// <returns></returns>
        public bool DeleteDeductIns(string deducIns_Id)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var deleteInsEntity = db.FindEntity<ProfileDeducInsEntity>(deducIns_Id);

                db.Delete<ProfileDeducInsEntity>(deleteInsEntity);

                //删除关联的图片表
                var deleteImgs = db.IQueryable<ProfileDeducImgEntiy>().Where(d => d.DeducIns_Id == deducIns_Id).ToArray();

                foreach (var item in deleteImgs)
                {
                    //删除图片文件
                    db.Delete<ProfileDeducImgEntiy>(item);

                    //删除文件

                }

                db.Commit();
            }

            return true;
        }

        public static ApiDeductAccordingContracts[] GeDeductDetails2ApiDeductAccordingContracts(Func<ProfileDeducInsEntity, bool> where)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var dedyctInsDatas = db.IQueryable<ProfileDeducInsEntity>().Where(where).Select(d => new ApiDeductAccordingContracts()
                {
                    DeducIns_Id = d.DeducIns_Id,
                    TaskEntryId = d.TaskEntry_Id,
                    SCNorm_Id = d.SCNorm_Id,
                    SCEntryName = d.SCEntryName,
                    SCTypeName = d.SCTypeName,
                    SCClassifyName = d.SCClassifyName,
                    SCNormProjectName = d.SCNormProjectName,
                    SCNormStandardName = d.SCNormStandardName,
                    DeductionSeveral = d.DeductionSeveral,
                    DeductionScore = d.DeductionScore,
                    DeductionDescribe = d.DeductionDescribe,
                    CreatorUserName = d.CreatorUserName,
                    CreatorUserId = d.CreatorUserId,
                    CreateTime = d.CreateTime,
                    SCNormIsDeduct = d.SCNormIsDeduct
                }).ToArray();

                //获取图片地址
                foreach (var item in dedyctInsDatas)
                {
                    item.imgPaths = db.IQueryable<ProfileDeducImgEntiy>().Where(d => d.DeducIns_Id == item.DeducIns_Id).Select(d => imgDomainPathURL + d.DeducImgPath).ToArray();

                }

                return dedyctInsDatas;
            }
        }


        /// <summary>
        /// 完善非定点信息
        /// </summary>
        /// <param name="taskEntryId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool PerfectFixedPoint(string taskEntryId, string info)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {

                var query = db.IQueryable<ProfileTaskEntryEntity>().Where(d => d.F_Id == taskEntryId);

                if (query.Count() > 0)
                {
                    var taskEntryTask = query.FirstOrDefault();

                    taskEntryTask.BYMESS2 = info;
                    taskEntryTask.BYMESS3 = true;
                }


                db.Commit();
            }

            return true;

        }

        public string UploadDeductImage(string base64ImageCode, string DeductId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {


                if (string.IsNullOrEmpty(base64ImageCode))
                {
                    throw new Exception("图片编码为空!");
                }

                string imageId = string.Empty;
                string uploadDate = DateTime.Now.ToString("yyyyddMM");
                string filePath = string.Empty;

                filePath = string.Format(@"{0}\{1}{2}", uploadDate, Guid.NewGuid().ToString(), ".png");

                string path = imageFolderPath + filePath;

                string directoryPath = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directoryPath);

                if (!File.Exists(path))
                {

                    byte[] b = Convert.FromBase64String(base64ImageCode);
                    MemoryStream m = new MemoryStream(b);
                    using (FileStream fs = File.Open(path, System.IO.FileMode.Create))
                    {
                        m.WriteTo(fs);
                        m.Close();
                        fs.Close();
                    }
                }

                var deduimgEnity = new ProfileDeducImgEntiy()
                {
                    DeducImg_Id = Guid.NewGuid().ToString(),
                    DeducIns_Id = DeductId,
                    DeducImgPath = filePath
                };

                db.Insert<ProfileDeducImgEntiy>(deduimgEnity);

                return deduimgEnity.DeducImg_Id;
            }
        }


        public bool InsertDeductIns(ApiDeductUploadContracts deductsInsEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {

                var sNormEntity = db.IQueryable<ProfileScireCriteria_NormEntity>().FirstOrDefault(d => d.SNormId == deductsInsEntity.SCNorm_Id);
                var sClassifyEntiy = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().FirstOrDefault(d => d.SClassifyId == sNormEntity.SClassifyId);
                var sTypeEntiy = db.IQueryable<ProfileScoreCriteria_TypeEntity>().FirstOrDefault(d => d.STypeId == sClassifyEntiy.STypeId);
                var sEntiyEntity = db.IQueryable<ProfileScoreCriteria_EntryEntity>().FirstOrDefault(d => d.SEntryId == sTypeEntiy.SEntryId);
                var userEntity = db.IQueryable<UserEntity>().FirstOrDefault(d => d.F_Id == deductsInsEntity.CreatorUserId);

                if (sNormEntity == null)
                    throw new Exception("未找到对应扣分标准!");
                if (userEntity == null)
                    throw new Exception("未找到上传用户!");


                //验证判断还能不能扣分
                //查询当前所有的扣分
                


                ProfileDeducInsEntity dbDeductIndesEntry = new ProfileDeducInsEntity()
                {
                    DeducIns_Id = Guid.NewGuid().ToString(),
                    TaskEntry_Id = deductsInsEntity.TaskEntry_Id,
                    SCNorm_Id = deductsInsEntity.SCNorm_Id,
                    SCEntryName = sEntiyEntity.Name,
                    SCTypeName = sTypeEntiy.Name,
                    SCClassifyName = sClassifyEntiy.SClassifyName,
                    SCNormProjectName = sNormEntity.SNormProjectName,
                    SCNormStandardName = sNormEntity.SNormStandardName,
                    DeductionSeveral = deductsInsEntity.DeductionSeveral,
                    DeductionScore = deductsInsEntity.DeductionScore,
                    DeductionDescribe = deductsInsEntity.DeductionDescribe,
                    CreateTime = DateTime.Now,
                    CreatorUserId = deductsInsEntity.CreatorUserId,
                    CreatorUserName = userEntity.F_RealName,
                    SCNormIsDeduct = sNormEntity.IsDeduct
                };

                db.Insert<ProfileDeducInsEntity>(dbDeductIndesEntry);


                deductsInsEntity.images = js.Deserialize<string[]>(deductsInsEntity.imagesStr);

                //上传图片
                if (deductsInsEntity.images != null)
                {
                    string imageId = string.Empty;
                    string uploadDate = DateTime.Now.ToString("yyyyddMM");
                    string filePath = string.Empty;

                    foreach (var item in deductsInsEntity.images)
                    {


                        filePath = string.Format(@"{0}\{1}{2}", uploadDate, Guid.NewGuid().ToString(), ".png");

                        string path = imageFolderPath + filePath;

                        string directoryPath = Path.GetDirectoryName(path);
                        Directory.CreateDirectory(directoryPath);

                        if (!File.Exists(path))
                        {
                            byte[] b = Convert.FromBase64String(item);
                            MemoryStream m = new MemoryStream(b);
                            using (FileStream fs = File.Open(path, System.IO.FileMode.Create))
                            {
                                m.WriteTo(fs);
                                m.Close();
                                fs.Close();
                            }
                        }

                        var deduimgEnity = new ProfileDeducImgEntiy()
                        {
                            DeducImg_Id = Guid.NewGuid().ToString(),
                            DeducIns_Id = dbDeductIndesEntry.DeducIns_Id,
                            DeducImgPath = filePath
                        };

                        db.Insert<ProfileDeducImgEntiy>(deduimgEnity);
                    }
                }

                try
                {
                    db.Commit();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}