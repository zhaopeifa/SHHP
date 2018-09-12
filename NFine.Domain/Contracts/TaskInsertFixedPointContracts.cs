using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Contracts
{
    public class TaskInsertFixedPointContracts
    {
        public string F_Id { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string CityId { get; set; }
        /// <summary>
        /// 区县
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
        /// 废纸箱
        /// </summary>
        public int WastebasketCount { get; set; }

        /// <summary>
        /// 沿途垃圾箱
        /// </summary>
        public int StreetTrashCount { get; set; }

        /// <summary>
        /// 机扫车
        /// </summary>
        public int MachineCleanCarCount { get; set; }

        /// <summary>
        /// 冲洗车
        /// </summary>
        public int WashTheCarCount { get; set; }

        /// <summary>
        /// 垃圾清运车
        /// </summary>
        public int GarbageTruckCarCount { get; set; }

        /// <summary>
        /// 飞行保洁车
        /// </summary>
        public int FlyingCarCount { get; set; }

        /// <summary>
        /// 四轮八桶车
        /// </summary>
        public int EightLadleCarCount { get; set; }



        public string PersonInChargeId { get; set; }

        public DateTime DeliveryTime { get; set; }

        public DateTime CompletionTime { get; set; }
    }
}
