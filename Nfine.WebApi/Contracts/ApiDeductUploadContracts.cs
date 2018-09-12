using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    public class ApiDeductUploadContracts
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string  DeducIns_Id{ get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public string TaskEntry_Id { get; set; }

        /// <summary>
        /// 扣分明细Id
        /// </summary>
        public string SCNorm_Id { get; set; }

        public string SCEntryName { get; set; }

        public string SCTypeName { get; set; }

        public string SCClassifyName { get; set; }

        public string SCNormProjectName { get; set; }

        public string SCNormStandardName { get; set; }


        /// <summary>
        /// 扣几处  
        /// </summary>
        public int DeductionSeveral { get; set; }

        /// <summary>
        /// 扣几分
        /// </summary>
        public int DeductionScore { get; set; }

        /// <summary>
        /// 扣分描述
        /// </summary>
        public string DeductionDescribe { get; set; }

        public string CreatorUserId { get; set; }

        public string CreatorUserName { get; set; }

        public string[] images { get; set; }

        public string imagesStr { get; set; }
    }
}