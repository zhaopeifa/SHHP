﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.SystemManage
{
    /// <summary>
    /// 环评-环卫-任务条目
    /// </summary>
    public class ProfileTaskEntryEntity : IEntity<ProfileTaskEntryEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }

        /// <summary>
        /// 是否定点任务
        /// </summary>
        public bool IsFixedPoint { get; set; }

        /// <summary>
        /// 项目类型
        /// </summary>
        public int ProjectType { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string F_EnCode { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 区县Id
        /// </summary>
        public string CountyId { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 街道Id
        /// </summary>
        public string StreetId { get; set; }

        /// <summary>
        /// 被派发人 责任人
        /// </summary>
        public string PersonInChargeId { get; set; }

        /// <summary>
        /// 任务条目 关联类型
        /// </summary>
        public int TaskEntryType { get; set; }

        /// <summary>
        /// 关联数据Id
        /// </summary>
        public string EntryDataId { get; set; }

        public string F_CreatorUserId { get; set; }

        public DateTime? F_CreatorTime { get; set; }

        public bool? F_DeleteMark { get; set; }

        public string F_DeleteUserId { get; set; }

        public DateTime? F_DeleteTime { get; set; }


        public string F_LastModifyUserId { get; set; }

        public DateTime? F_LastModifyTime { get; set; }

        public int? BYMESS1 { get; set; }

        /// <summary>
        /// 备用  非定点地址 车牌号
        /// </summary>
        public string BYMESS2 { get; set; }

        /// <summary>
        /// 备用 非定点是否上传地点信息
        /// </summary>
        public bool? BYMESS3 { get; set; }
    }
}
