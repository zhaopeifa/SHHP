using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    /// <summary>
    /// 道路
    /// </summary>
    public class ApiTaskDataEntryContracts
    {

        private bool _isFixedPoint = true;

        public string Title { get; set; }
        public string TaskId { get; set; }

        public string TaskEntryId { get; set; }

        /// <summary>
        /// 是否是定点任务
        /// </summary>
        public bool IsFixedPoint
        {
            get { return this._isFixedPoint; }
            set { this._isFixedPoint = value; }
        }

        public bool IsPerfect { get; set; }

        public int type { get; set; }

        public string StreetId { get; set; }
        public string StreetName { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 派发时间
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompletionTime { get; set; }

        public string DeliveryTimeFormat
        {
            get { return DeliveryTime.ToString(); }
        }
        public string CompletionTimeFormat
        {
            get
            {
                return this.CompletionTime.ToString();
            }
        }

        /// <summary>
        /// 是否完成
        /// </summary>
        public int IsComplete { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarId { get; set; }

        /// <summary>
        /// 非定点信息
        /// </summary>
        public string NotFiexdInfo { get; set; }

        /// <summary>
        /// 绿地起点
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 绿地终止点
        /// </summary>
        public string Destination { get; set; }

        public bool IsHaveCarWorkItemSelect { get; set; }
    }
}