using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Domain.Enums
{
    /// <summary>
    /// 压缩站类型
    /// </summary>
    public enum ProfileCompressionStationType
    {
        沿街压缩站=1,
        小区压缩站=2
    }
    public static class ProfileCompressionStationTypeExtension
    {
        private static int GetIntValue(this ProfileCompressionStationType type)
        {
            return (int)type;
        }
    }
}
