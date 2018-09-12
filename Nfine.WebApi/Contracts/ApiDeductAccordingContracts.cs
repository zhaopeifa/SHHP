using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    /// <summary>
    /// 获取扣分明细
    /// </summary>
    public class ApiDeductAccordingContracts
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string DeducIns_Id { get; set; }

        /// <summary>
        /// 关联子任务Id
        /// </summary>
        public string TaskEntryId { get; set; }

        /// <summary>
        /// 扣分明细Id
        /// </summary>
        public string SCNorm_Id { get; set; }

        /// <summary>
        /// 评分标准大类
        /// </summary>
        public string SCEntryName { get; set; }

        /// <summary>
        /// 评分标准中类
        /// </summary>
        public string SCTypeName { get; set; }

        /// <summary>
        /// 评分标准小类
        /// </summary>
        public string SCClassifyName { get; set; }

        /// <summary>
        /// 评分标准检查项目
        /// </summary>
        public string SCNormProjectName { get; set; }

        /// <summary>
        /// 是否扣分项
        /// </summary>
        public bool SCNormIsDeduct { get; set; }

        /// <summary>
        /// 评分标准评分明细
        /// </summary>
        public string SCNormStandardName { get; set; }

        /// <summary>
        /// 扣分扣几处
        /// </summary>
        public int DeductionSeveral { get; set; }

        /// <summary>
        /// 扣分扣积分
        /// </summary>
        public int DeductionScore { get; set; }

        /// <summary>
        /// 扣分描述
        /// </summary>
        public string DeductionDescribe { get; set; }

        public string CreatorUserName { get; set; }

        public string CreatorUserId { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string[] imgPaths { get; set; }
    }
}