using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Entity.SystemManage
{
    /// <summary>
    /// 环评-环卫-机扫车工作项
    /// </summary>
    public class ProfileSanitationCarWorkItemEntity : IEntity<ProfileSanitationCarWorkItemEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }

        /// <summary>
        /// 作业班次
        /// </summary>
        public string WorkShift { get; set; }

        /// <summary>
        /// 下标 序号
        /// </summary>
        public int Subscript { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string WorkTime { get; set; }

        /// <summary>
        /// 作业点名称
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// 作业点地址
        /// </summary>
        public string WorkAddress { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        public string F_CreatorUserId { get; set; }

        public DateTime? F_CreatorTime { get; set; }

        public bool? F_DeleteMark { get; set; }

        public string F_DeleteUserId { get; set; }

        public DateTime? F_DeleteTime { get; set; }

        public string F_LastModifyUserId { get; set; }

        public DateTime? F_LastModifyTime { get; set; }
    }
}
