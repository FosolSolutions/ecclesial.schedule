using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class Attribute : BaseEntity
    {
        #region Properties
        [PrimaryKey(Order = 0), MaxLength(50)]
        public string Key { get; set; }

        [PrimaryKey(Order = 1), MaxLength(250)]
        public string Value { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
        #endregion

        #region Constructors
        public Attribute()
        {

        }

        public Attribute(string key, string value)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(key));

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(value));

            this.Key = key;
            this.Value = value;
        }
        #endregion
    }
}
