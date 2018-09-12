using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Enums
{
    /// <summary>
    /// 垃圾箱房类型
    /// </summary>
    public enum ProfileSanitationGarbageBoxTypeEnum
    {
        小区压缩站 = 1,
        沿街压缩站 = 2
    }

    public static class ProfileSanitationGarbageBoxTypeEnumExtension
    {
        /// <summary>
        /// 获取枚举的对应的字符串值
        /// </summary>
        /// <returns></returns>
        public static int GetIntValue(this ProfileSanitationGarbageBoxTypeEnum DataViewType)
        {
            return ((int)DataViewType);
        }
    }
}
