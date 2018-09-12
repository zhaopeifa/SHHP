using Nfine.WebApi.Contracts;
using Nfine.WebApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfine.WebApi.Code.Car
{
    public interface ICar
    {
        ApiCarWorkItem[] GetWorkItem(CarWhereType keyWorkType, string keyWork);

        ApiKeyValue<string, string>[] GetCarId(ApiPagination pagination, string keyWord);

        string[] GetWorkShift(ApiPagination pagination, string keyWord);
    }
}
