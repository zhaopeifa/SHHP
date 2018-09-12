﻿using NFine.Code;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Contracts;
using NFine.Domain.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Repository.SystemManage
{
    /// <summary>
    /// 扣分记录表
    /// </summary>
    public class ProfileDeducInsRepository : RepositoryBase<ProfileDeducInsEntity>
    {

        public void SubmitForm(ProfileDeducInsSubMitContracts entity, string keyValue, string DeducIns_Id)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                //获取评分标准。
                var scNormEntity = db.IQueryable<ProfileScireCriteria_NormEntity>().Where(d => d.SNormId == entity.NormId).FirstOrDefault();
                var scClassifyEntity = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().Where(d => d.SClassifyId == scNormEntity.SClassifyId).FirstOrDefault();
                var scTypeEntit = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == scClassifyEntity.STypeId).FirstOrDefault();
                var scEntryEntity = db.IQueryable<ProfileScoreCriteria_EntryEntity>().Where(d => d.SEntryId == scTypeEntit.SEntryId).FirstOrDefault();


                //判断当前当中是存在数据，如果存在则是修改
                //两条具体使用哪一个？
                var deducInsQuery = db.IQueryable<ProfileDeducInsEntity>().Where(d => d.DeducIns_Id == DeducIns_Id);

                if (!string.IsNullOrEmpty(DeducIns_Id))
                {

                    var deducInsEntiy = deducInsQuery.FirstOrDefault();

                    deducInsEntiy.TaskEntry_Id = entity.TaskEntryId;
                    deducInsEntiy.SCNorm_Id = entity.NormId;
                    deducInsEntiy.SCNormProjectName = scNormEntity.SNormProjectName;
                    deducInsEntiy.SCNormStandardName = scNormEntity.SNormStandardName;
                    deducInsEntiy.SCNormIsDeduct = scNormEntity.IsDeduct;
                    deducInsEntiy.SCClassifyName = scClassifyEntity.SClassifyName;
                    deducInsEntiy.SCTypeName = scTypeEntit.Name;
                    deducInsEntiy.SCEntryName = scEntryEntity.Name;
                    deducInsEntiy.DeductionScore = entity.DeductionScore;
                    deducInsEntiy.DeductionSeveral = entity.DeductionSeveral;
                    deducInsEntiy.DeductionDescribe = entity.DeductionDescribe;
                    deducInsEntiy.LastModifyTime = DateTime.Now;
                    deducInsEntiy.LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                    deducInsEntiy.LastModifyUserName = OperatorProvider.Provider.GetCurrent().UserName;


                    db.Update<ProfileDeducInsEntity>(deducInsEntiy);
                }
                else
                {
                    ProfileDeducInsEntity deducInsEntity = new ProfileDeducInsEntity()
                    {
                        DeducIns_Id = keyValue,
                        TaskEntry_Id = entity.TaskEntryId,
                        SCNorm_Id = entity.NormId,
                        SCNormProjectName = scNormEntity.SNormProjectName,
                        SCNormStandardName = scNormEntity.SNormStandardName,
                        SCNormIsDeduct = entity.SNormIsDeduct,
                        SCClassifyName = scClassifyEntity.SClassifyName,
                        SCTypeName = scTypeEntit.Name,
                        SCEntryName = scEntryEntity.Name,
                        DeductionScore = entity.DeductionScore,
                        DeductionSeveral = entity.DeductionSeveral,
                        DeductionDescribe = entity.DeductionDescribe,
                        CreateTime = DateTime.Now,
                        CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId,
                        CreatorUserName = OperatorProvider.Provider.GetCurrent().UserName
                    };

                    db.Insert<ProfileDeducInsEntity>(deducInsEntity);
                }




                db.Commit();
            }
        }

        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var deleteEntity = db.IQueryable<ProfileDeducInsEntity>().Where(d => d.DeducIns_Id == keyValue).FirstOrDefault();

                db.Delete<ProfileDeducInsEntity>(deleteEntity);

                var delimgs = db.IQueryable<ProfileDeducImgEntiy>().Where(d => d.DeducIns_Id == keyValue).ToArray();

                foreach (var item in delimgs)
                {
                    db.Delete<ProfileDeducImgEntiy>(item);
                }

                db.Commit();
            }
        }
    }
}
