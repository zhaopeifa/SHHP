using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Contracts
{
    public class ApiKeyValue<keyT,valueT>
    {
        public ApiKeyValue() { }

        public ApiKeyValue(keyT key,valueT value) {
            this.Key = key;
            this.Value = value;
        }

        public keyT Key { get; set; }

        public valueT Value { get; set; }
    }
}