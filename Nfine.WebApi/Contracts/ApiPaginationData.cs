using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    public class ApiPaginationData<dataT>
    {
        public ApiPaginationData() { }

        public ApiPaginationData(ApiPagination pagination, dataT data)
        {
            this.Pagination = pagination;
            this.Data = data;
        }

        /// <summary>
        /// 分页参数
        /// </summary>
        public ApiPagination Pagination { get; set; }

        /// <summary>
        /// 数据data
        /// </summary>
        public dataT Data { get; set; }
    }
}