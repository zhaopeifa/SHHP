using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Enums
{
    /// <summary>
    /// 任务项类型
    /// </summary>
    public enum ProfileTaskEntryTypeEnum
    {
        /// <summary>
        /// 道路
        /// </summary>
        Way = 1,
        /// <summary>
        /// 公厕
        /// </summary>
        Tandas,
        /// <summary>
        /// 垃圾厢房
        /// </summary>
        GarbageBox,
        /// <summary>
        /// 压缩站
        /// </summary>
        compressionStation,
        /// <summary>
        /// 沿途绿化
        /// </summary>
        Greening,
        /// <summary>
        /// 绿色账户小区
        /// </summary>
        GreenResidential,
        /// <summary>
        /// 倒粪池小便池
        /// </summary>
        cesspool,
        /// <summary>
        /// 废纸箱
        /// </summary>
        Wastebasket,
        /// <summary>
        /// 沿街垃圾桶
        /// </summary>
        StreetTrash,
        /// <summary>
        /// 机扫车
        /// </summary>
        MachineCleanCar,
        /// <summary>
        /// 冲洗车
        /// </summary>
        WashTheCar,
        /// <summary>
        /// 垃圾清运车
        /// </summary>
        GarbageTruckCar,
        /// <summary>
        /// 飞行保洁车
        /// </summary>
        FlyingCar,
        /// <summary>
        /// 四轮八桶车
        /// </summary>
        EightLadleCar,
        /// <summary>
        /// 车辆
        /// </summary>
        Car
    }


    public static class ProfileTaskEntryTypeEnumExtension
    {
        public static int GetIntValue(this ProfileTaskEntryTypeEnum type)
        {
            return (int)type;
        }

        public static string GetComments(this ProfileTaskEntryTypeEnum type)
        {
            string result = string.Empty;

            switch (type)
            {
                case ProfileTaskEntryTypeEnum.Way:
                    result = "道路清扫";
                    break;
                case ProfileTaskEntryTypeEnum.Tandas:
                    result = "环卫公厕";
                    break;
                case ProfileTaskEntryTypeEnum.GarbageBox:
                    result = "垃圾箱房";
                    break;
                case ProfileTaskEntryTypeEnum.compressionStation:
                    result = "垃圾箱房";
                    break;
                case ProfileTaskEntryTypeEnum.Greening:
                    result = "压缩站";
                    break;
                case ProfileTaskEntryTypeEnum.GreenResidential:
                    result = "绿色账户小区";
                    break;
                case ProfileTaskEntryTypeEnum.cesspool:
                    result = "倒粪站小便池";
                    break;
                case ProfileTaskEntryTypeEnum.Wastebasket:
                    break;
                case ProfileTaskEntryTypeEnum.StreetTrash:
                    break;
                case ProfileTaskEntryTypeEnum.MachineCleanCar:
                    break;
                case ProfileTaskEntryTypeEnum.WashTheCar:
                    break;
                case ProfileTaskEntryTypeEnum.GarbageTruckCar:
                    break;
                case ProfileTaskEntryTypeEnum.FlyingCar:
                    break;
                case ProfileTaskEntryTypeEnum.EightLadleCar:
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取环卫子项
        /// </summary>
        /// <returns></returns>
        public static ProfileTaskEntryTypeEnum[] GetSantationEntrys()
        {
            ProfileTaskEntryTypeEnum[] result = new ProfileTaskEntryTypeEnum[7];

            result[0] = ProfileTaskEntryTypeEnum.Way;
            result[1] = ProfileTaskEntryTypeEnum.Tandas;
            result[2] = ProfileTaskEntryTypeEnum.GarbageBox;
            result[3] = ProfileTaskEntryTypeEnum.compressionStation;
            result[4] = ProfileTaskEntryTypeEnum.Greening;
            result[5] = ProfileTaskEntryTypeEnum.GreenResidential;
            result[6] = ProfileTaskEntryTypeEnum.cesspool;

            return result;
        }
    }
}
