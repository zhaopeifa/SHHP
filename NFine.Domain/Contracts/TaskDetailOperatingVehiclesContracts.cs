using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Contracts
{
    /// <summary>
    /// 作业车辆
    /// </summary>
    public class TaskDetailOperatingVehiclesContracts : ITaskDetail
    {
        public string F_Id { get; set; }

        public bool? IsPerfect { get; set; }

        public string CarId { get; set; }

        public bool CompleteState { get; set; }

        public string DataId { get; set; }

        public string PersonInChargeId { get; set; }

        public string PersonInChargeName { get; set; }

        public string StreetId { get; set; }

        public string StreetName { get; set; }

        public string OrdeNo { get; set; }

        public DateTime CompletionTime { get; set; }

        public DateTime? DeliveryTime { get; set; }
    }
}
