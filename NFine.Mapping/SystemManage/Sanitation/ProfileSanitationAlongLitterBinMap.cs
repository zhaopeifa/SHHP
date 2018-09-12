﻿using NFine.Domain.Entity.SystemManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFine.Mapping.SystemManage
{
    //ProfileSanitationAlongLitterBinEntity
    public class ProfileSanitationAlongLitterBinMap : EntityTypeConfiguration<ProfileSanitationAlongLitterBinEntity>
    {
        public ProfileSanitationAlongLitterBinMap()
        {
            this.ToTable("ProfileSanitationAlongLitterBin");
            this.HasKey(t => t.F_Id);
        }
    }
}
