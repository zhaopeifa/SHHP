using Nfine.WebApi.Data.Enums;
using NFine.Data;
using NFine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfine.WebApi.Code.Task
{
    public interface ITask
    {
        Contracts.ApiTaskDataEntryContracts[] GetTask(string userId, string EntryId, string typeId, ProfileTaskStateEnum taskState, out int ProfileTaskEntryType);

        Contracts.ApiTaskDataEntryContracts[] GetWayTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);


        Contracts.ApiTaskDataEntryContracts[] GetTandasTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetGarbageBoxTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetcompressionStationTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetGreeningTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetGreenResidentialTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetCesspoolTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetWastebasketTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetStreetTrashTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetMachineCleanCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetWashTheCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetGarbageTruckCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetFlyingCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        Contracts.ApiTaskDataEntryContracts[] GetEightLadleCarTask(IRepositoryBase db, string userId, string typeId, ProfileTaskStateEnum taskState);

        int GetUndoneTaskCount(string userId, string entryId);
    }
}
