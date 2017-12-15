using Fosol.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class Participant : BaseEntity
    {
        #region Properties
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Required]
        public Guid Key { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        public Gender Gender { get; set; }

        public List<ParticipantAttribute> Attributes { get; set; } = new List<ParticipantAttribute>();
        #endregion

        #region Constructors
        public Participant()
        {

        }

        public Participant(string name, string email, params dynamic[] attributes)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Cannot be null, empty or whitespace.", nameof(name));

            var names = name.Split(' ');
            this.FirstName = names[0];
            this.LastName = names[1];
            this.Email = email;
            this.Key = Guid.NewGuid();

            if (attributes != null && attributes.Count() > 0)
            {
                this.Attributes.AddRange(attributes.Select(a => new ParticipantAttribute(this, a.Key, a.Value)));
            }
        }

        public Participant(string name, string email, params ParticipantAttribute[] attributes)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Cannot be null, empty or whitespace.", nameof(name));

            var names = name.Split(' ');
            this.FirstName = names[0];
            this.LastName = names[1];
            this.Email = email;
            this.Key = Guid.NewGuid();

            if (attributes != null && attributes.Count() > 0)
            {
                this.Attributes.AddRange(attributes);
            }
        }

        public Participant(string firstName, string lastName, string email, params ParticipantAttribute[] attributes) : this($"{firstName} {lastName}", email, attributes)
        {
        }
        #endregion
    }
}
