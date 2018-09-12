using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    public class ApiCarWorkItem
    {
        public string F_Id { get; set; }

        /// <summary>
        /// 作业班次
        /// </summary>
        public string WorkShift { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Subscript { get; set; }

        /// <summary>
        /// 工作点时间
        /// </summary>
        public string WorkTime { get; set; }

        /// <summary>
        /// 工作点名称
        /// </summary>
        public string WorkName { get; set; }

        /// <summary>
        /// 工作点地址
        /// </summary>
        public string WorkAddress { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
    }
}