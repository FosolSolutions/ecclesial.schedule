using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class Tag : BaseEntity
    {
        #region Properties
        [PrimaryKey, MaxLength(100)]
        public string Id { get; set; }
        #endregion

        #region Constructors
        public Tag()
        {

        }

        public Tag(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(id));

            this.Id = id;
        }
        #endregion
    }
}
