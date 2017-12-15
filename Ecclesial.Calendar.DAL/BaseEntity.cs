using Fosol.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesial.Calendar.DAL
{
    public abstract class BaseEntity
    {
        #region Properties
        [RowVersion]
        public byte[] RowVersion { get; set; }
        #endregion
    }
}
