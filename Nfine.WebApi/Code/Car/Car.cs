using Nfine.WebApi.Contracts;
using Nfine.WebApi.Enums;
using NFine.Data;
using NFine.Data.Extensions;
using NFine.Domain.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nfine.WebApi.Code.Car
{
    public class Car : ICar
    {
        public Contracts.ApiCarWorkItem[] GetWorkItem(CarWhereType keyWorkType, string keyWork)
        {
            IQueryable<ProfileSanitationCarWorkItemEntity> query = null;

            using (var db = new RepositoryBase().BeginTrans())
            {
                switch (keyWorkType)
                {
                    case CarWhereType.CarId:
                        var carQuery = db.IQueryable<ProfileSanitationCarEntity>().Where(d => d.CarId == keyWork);
                        if (carQuery.Count() > 0)
                        {
                            var carEntity = carQuery.FirstOrDefault();

                            query = db.IQueryable<ProfileSanitationCarWorkItemEntity>().Where(d => d.WorkShift == carEntity.WorkShift);
                        }

                        break;
                    case CarWhereType.WorkShift:

                        query = db.IQueryable<ProfileSanitationCarWorkItemEntity>().Where(d => d.WorkShift == keyWork);

                        break;
                    default:
                        break;
                }


                if (query != null)
                {
                    return query.Select(d => new Nfine.WebApi.Contracts.ApiCarWorkItem()
                    {
                        F_Id = d.F_Id,
                        WorkShift = d.WorkShift,
                        Subscript = d.Subscript,
                        WorkTime = d.WorkTime,
                        WorkName = d.WorkName,
                        WorkAddress = d.WorkAddress,
                        Note = d.Note
                    }).OrderBy(d => d.Subscript).ToArray();
                }

                return null;
            }

        }

        public ApiKeyValue<string, string>[] GetCarId(Contracts.ApiPagination pagination, string keyWord)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                IQueryable<ProfileSanitationCarEntity> query = db.IQueryable<ProfileSanitationCarEntity>();

                //模糊查询
                if (!string.IsNullOrEmpty(keyWord))
                {
                    query = query.Where(d => d.CarId.Contains(keyWord));
                }

                //分页
                if (pagination != null)
                {
                    pagination.records = query.Count();

                    query = query.OrderBy(d => d.CarId).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
                }

                return query.Select(d => new ApiKeyValue<string, string>()
                {
                    Key = d.CarId,
                    Value = d.WorkShift
                }).ToArray();
            }
        }

        public string[] GetWorkShift(Contracts.ApiPagination pagination, string keyWord)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                IQueryable<ProfileSanitationCarEntity> query = db.IQueryable<ProfileSanitationCarEntity>();

                //模糊查询
                if (!string.IsNullOrEmpty(keyWord))
                {
                    query = query.Where(d => d.WorkShift.Contains(keyWord));
                }

                //分页
                if (pagination != null)
                {
                    pagination.records = query.Count();

                    query = query.OrderBy(d => d.WorkShift).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
                }

                return query.Select(d => d.WorkShift).ToArray();
            }
        }
    }
}