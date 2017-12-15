using Ecclesial.Calendar.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace Ecclesial.Calendar.Helpers
{
    public static class Authentication
    {
        public static bool IsAllowed(this ClaimsPrincipal user, IEnumerable<TaskAttribute> attributes)
        {
            foreach (var attr in attributes)
            {
                if (!user.Claims.Any(c => String.Compare(c.Type, attr.Key, true) == 0 && String.Compare(c.Value, attr.Value, true) == 0 && c.Issuer == "Ecclesial.Calendar"))
                    return false;
            }

            return true;
        }

        public static bool IsAllowed(this ClaimsPrincipal user, DataSource context, int taskId)
        {
            // Check if user has claims that match the required task attributes.
            var task_attrs = context.TaskAttributes.Where(new { EventTask_Id = taskId }).ToList();

            return user.IsAllowed(task_attrs);
        }

        public static Participant GetParticipant(this ClaimsPrincipal user, DataSource context)
        {
            var impersonate = user.GetImpersonate();
            if (impersonate == null)
            {
                var userId = int.Parse(user.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
                return context.Participants.Find(userId);
            }
            else
            {
                return context.Participants.Find(int.Parse(impersonate.Value));
            }
        }

        public static ClaimsIdentity CreateIdentity(this DataSource context, Participant participant)
        {
            if (participant == null)
                return null;

            participant.Attributes = context.ParticipantAttributes.Where(new { Participant_Id = participant.Id }).ToList();

            var claims = new List<Claim>(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, $"{participant.Id}"),
                new Claim(ClaimTypes.Email, participant.Email),
                new Claim(ClaimTypes.Name, $"{participant.FirstName} {participant.LastName}"),
                new Claim(ClaimTypes.Gender, $"{participant.Gender}"),
                new Claim("Key", $"{participant.Key}", "string", "Ecclesial.Calendar")
            });

            foreach (var attr in participant.Attributes)
            {
                claims.Add(new Claim(attr.Key, attr.Value, "string", "Ecclesial.Calendar"));
            }

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public static ClaimsIdentity CreateIdentity(this DataSource context, Guid key)
        {
            if (key == Guid.Empty)
                return null;

            var participant = context.Participants.Where(new { Key = key }).FirstOrDefault();

            return context.CreateIdentity(participant);
        }

        public static ClaimsIdentity CreateIdentity(this DataSource context, int participantId)
        {
            var participant = context.Participants.Find(participantId);

            return context.CreateIdentity(participant);
        }

        public static Claim GetNameIdentifier(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        }

        public static Claim GetName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        }

        public static Claim GetEmail(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        }

        public static Claim GetImpersonate(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "Impersonate");
        }

        public static Claim GetNameIdentifier(this ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        }

        public static Claim GetName(this ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        }

        public static Claim GetEmail(this ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        }

        public static Claim GetImpersonate(this ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == "Impersonate");
        }
    }
}
