using Nfine.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfine.WebApi.Code.Deduct
{
    public interface IDeduct
    {
        ApiDeductAccordingContracts[] GetDeductDetails(string taskEntryId);

        ApiDeductAccordingContracts[] GetDeductDetails(string taskEntryId, string SCNormId);

        bool PerfectFixedPoint(string taskEntryId, string info);

        bool InsertDeductIns(ApiDeductUploadContracts deductsInsEntity);

        string oudInsertDeductIns(ApiDeductUploadContracts deductsInsEntity);

        string UpdateDeductIns(string deducIns_Id, ApiDeductUploadContracts deductsInsEntity);

        bool DeleteDeductIns(string deducIns_Id);

        string UploadDeductImage(string base64ImageCode, string DeductId);
    }
}
