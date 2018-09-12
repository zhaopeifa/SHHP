﻿using NFine.Code;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Contracts;
using NFine.Domain.Entity.SystemManage;
using NFine.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Application.SystemManage
{
    public class ProfileDeducInsApp
    {
        private static string _imgDomainPathURL;

        private static string ImgDomainPathURL
        {
            get
            {
                if (string.IsNullOrEmpty(_imgDomainPathURL))
                { _imgDomainPathURL = System.Configuration.ConfigurationManager.AppSettings["ImgDomainPathURL"]; }
                return _imgDomainPathURL;
            }
        }

        private ProfileDeducInsRepository service = new ProfileDeducInsRepository();
        private ProfileDeducImgRepository imgService = new ProfileDeducImgRepository();

        public DataTable FildSql(string sqlStr)
        {
            DataTable table = DbHelper.ExecuteDataTable(sqlStr.ToString(), null);
            return table;
        }

        /// <summary>
        /// 获取扣分标准
        /// </summary>
        /// <param name="taskEntryId"></param>
        /// <returns></returns>
        public DataTable GetDeducInsTable(Pagination pagination, string keyword, string taskEntryId)
        {
            StringBuilder sqlStr = new StringBuilder();

            sqlStr.Append("SELECT ");
            sqlStr.Append(" * ");
            sqlStr.Append("FROM ProfileDeducIns WHERE TaskEntry_Id='" + taskEntryId + "'  ");

            if (!string.IsNullOrEmpty(keyword))
            {
                sqlStr.Append("AND SCEntryName='%" + keyword + "%' ");
                sqlStr.Append("OR SCTypeName='%" + keyword + "%' ");
                sqlStr.Append("OR SCClassifyName='%" + keyword + "%' ");
                sqlStr.Append("OR SCNormProjectName='%" + keyword + "%' ");
                sqlStr.Append("OR SCNormStandardName='%" + keyword + "%' ");
                sqlStr.Append("OR DeductionDescribe='%" + keyword + "%' ");
            }

            sqlStr.Append(" ORDER BY CreateTime DESC");
            sqlStr.Append(" offset " + pagination.rows * (pagination.page - 1) + " rows fetch next " + pagination.rows * (pagination.page - 1) + pagination.rows + " rows ONLY");

            DataTable dataTable = DbHelper.ExecuteDataTable(sqlStr.ToString(), null);

            return dataTable;

        }

        public List<ProfileDeducInsEntity> GetDeducIns(Pagination pagination, string keyword, string taskEntryId)
        {
            var expression = ExtLinq.True<ProfileDeducInsEntity>();

            expression = expression.And(t => t.TaskEntry_Id == taskEntryId);

            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.SCTypeName.Contains(keyword));
                expression = expression.Or(t => t.SCTypeName.Contains(keyword));
                expression = expression.Or(t => t.SCClassifyName.Contains(keyword));
                expression = expression.Or(t => t.SCNormProjectName.Contains(keyword));
                expression = expression.Or(t => t.SCNormStandardName.Contains(keyword));
                expression = expression.Or(t => t.DeductionDescribe.Contains(keyword));
            }

            return service.FindList(expression, pagination);
        }

        public void SubmitForm(ProfileDeducInsSubMitContracts entity, string keyValue, string DeducIns_Id)
        {
            service.SubmitForm(entity, keyValue, DeducIns_Id);
        }

        public ProfileDeducInsSubMitContracts GetForm(string normId, string taskEntryId)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var deducInsQuery = db.IQueryable<ProfileDeducInsEntity>().Where(d => d.SCNorm_Id == normId && d.TaskEntry_Id == taskEntryId);


                if (deducInsQuery.Count() > 0)
                {
                    var deduInsEntiy = deducInsQuery.FirstOrDefault();
                    return new ProfileDeducInsSubMitContracts()
                    {
                        F_Id = deduInsEntiy.DeducIns_Id,
                        TaskEntryId = deduInsEntiy.TaskEntry_Id,
                        DeductionDescribe = deduInsEntiy.DeductionDescribe,
                        DeductionSeveral = deduInsEntiy.DeductionSeveral,
                        DeductionScore = deduInsEntiy.DeductionScore,
                        NormId = deduInsEntiy.SCNorm_Id
                    };
                }

                return null;
            }
        }

        public ProfileDeducInsSubMitContracts GetFormByKeyValue(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var deduInsEntiy = service.FindEntity(keyValue);

                var scNormEntity = db.IQueryable<ProfileScireCriteria_NormEntity>().Where(d => d.SNormId == deduInsEntiy.SCNorm_Id).FirstOrDefault();
                var scClassifyEntity = db.IQueryable<ProfileScoreCriteria_ClassifyEntity>().Where(d => d.SClassifyId == scNormEntity.SClassifyId).FirstOrDefault();
                var scTypeEntit = db.IQueryable<ProfileScoreCriteria_TypeEntity>().Where(d => d.STypeId == scClassifyEntity.STypeId).FirstOrDefault();


                return new ProfileDeducInsSubMitContracts()
                {
                    SClassifyName = deduInsEntiy.SCClassifyName,
                    SClassifyScore = scClassifyEntity.Score,
                    SEntryName = deduInsEntiy.SCEntryName,
                    SNormCondition = scNormEntity.Condition,
                    SNormIsDeduct = scNormEntity.IsDeduct,
                    SNormProjectName = deduInsEntiy.SCNormProjectName,
                    SNormStandardName = deduInsEntiy.SCNormStandardName,
                    STypeName = deduInsEntiy.SCTypeName,
                    DeducIns_Id = deduInsEntiy.DeducIns_Id,
                    F_Id = deduInsEntiy.DeducIns_Id,
                    TaskEntryId = deduInsEntiy.TaskEntry_Id,
                    DeductionDescribe = deduInsEntiy.DeductionDescribe,
                    DeductionSeveral = deduInsEntiy.DeductionSeveral,
                    DeductionScore = deduInsEntiy.DeductionScore,
                    NormId = deduInsEntiy.SCNorm_Id
                };
            }
        }

        public List<KeyValuePair<string, string>> GetImgFormByKeyValue(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var result = db.IQueryable<ProfileDeducImgEntiy>().Where(d => d.DeducIns_Id == keyValue).ToList();

                return result.Select(d => new KeyValuePair<string, string>(d.DeducImg_Id, ImgDomainPathURL + d.DeducImgPath)).ToList();
            }
        }

        public void DeleteImageForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                var deleteEntiy = db.IQueryable<ProfileDeducImgEntiy>().FirstOrDefault(d => d.DeducImg_Id == keyValue);

                imgService.Delete(deleteEntiy);
            }
        }

        public string DuImagePath(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                return db.IQueryable<ProfileDeducImgEntiy>().FirstOrDefault(d => d.DeducImg_Id == keyValue).DeducImgPath;
            }
        }

        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }

        public void SubmitImageForm(string filePath, string deducInsId)
        {
            imgService.SubmitForm(filePath, deducInsId);
        }
    }
}
