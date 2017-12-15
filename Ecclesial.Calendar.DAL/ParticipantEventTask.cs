using Fosol.Data;
using Fosol.Data.Extensions.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesial.Calendar.DAL
{
    public class ParticipantEventTask : BaseEntity
    {
        #region Properties
        //[PrimaryKey, ForeignKey]
        //public int ParticipantId { get; set; }

        //[PrimaryKey, ForeignKey]
        //public int EventTaskId { get; set; }

        [PrimaryKey, ForeignKey]
        public Participant Participant { get; set; }

        [PrimaryKey, ForeignKey]
        public EventTask EventTask { get; set; }
        #endregion

        #region Constructors
        public ParticipantEventTask()
        {

        }

        //public ParticipantEventTask(int participantId, int eventTaskId)
        //{
        //    this.ParticipantId = participantId;
        //    this.EventTaskId = eventTaskId;
        //}

        //public ParticipantEventTask(Participant participant, EventTask eventTask)
        //{
        //    this.ParticipantId = participant.Id;
        //    this.EventTaskId = eventTask.Id;
        //}

        public ParticipantEventTask(Participant participant, EventTask eventTask)
        {
            this.Participant = participant;
            this.EventTask = eventTask;
        }
        #endregion

        #region Methods
        public static string InsertSql(Participant participant, EventTask eventTask)
        {
            return $"INSERT [{typeof(ParticipantEventTask).GetScheme()}].[{typeof(ParticipantEventTask).GetTableName()}] ([Participant_Id], [EventTask_Id]) VALUES ( {participant.Id}, {eventTask.Id} );";
        }

        public static string DeleteSql(Participant participant, EventTask eventTask)
        {
            return $"DELETE FROM [{typeof(ParticipantEventTask).GetScheme()}].[{typeof(ParticipantEventTask).GetTableName()}] WHERE [Participant_Id]={participant.Id} AND [EventTask_Id]={eventTask.Id};";
        }

        public static string SelectSql(Participant participant)
        {
            return $"SELECT * FROM [{typeof(ParticipantEventTask).GetScheme()}].[{typeof(ParticipantEventTask).GetTableName()}] WHERE [Participant_Id]={participant.Id}";
        }

        public static string SelectSql(EventTask eventTask)
        {
            return $"SELECT * FROM [{typeof(ParticipantEventTask).GetScheme()}].[{typeof(ParticipantEventTask).GetTableName()}] WHERE [EventTask_Id]={eventTask.Id}";
        }

        public static string SelectParticipantsSql()
        {
            return $"SELECT p.* FROM [{typeof(Participant).GetScheme()}].[{typeof(Participant).GetTableName()}] p INNER JOIN [{typeof(ParticipantEventTask).GetScheme()}].[{typeof(ParticipantEventTask).GetTableName()}] pet ON p.[Id] = pet.[Participant_Id] WHERE pet.[EventTask_Id]=@Id";
        }
        #endregion
    }
}
