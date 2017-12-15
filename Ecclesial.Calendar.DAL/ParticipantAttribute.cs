using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class ParticipantAttribute : BaseEntity
    {
        #region Properties
        [PrimaryKey, MaxLength(50)]
        public string Key { get; set; }

        [PrimaryKey, MaxLength(250)]
        public string Value { get; set; }

        [PrimaryKey, ForeignKey]
        public Participant Participant { get; set; }
        #endregion

        #region Constructors
        public ParticipantAttribute()
        {

        }

        public ParticipantAttribute(Participant participant, string key, string value)
        {
            if (participant == null)
                throw new ArgumentNullException(nameof(participant));

            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(key));

            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Cannot be null, emtpy or whitespate.", nameof(value));

            this.Participant = participant;
            this.Key = key;
            this.Value = value;
        }
        #endregion
    }
}
