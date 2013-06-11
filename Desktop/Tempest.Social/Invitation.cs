using System;
using System.Linq;

namespace Tempest.Social
{
	public enum InvitationResponse
		: byte
	{
		Error = 0,
		Accepted = 1,
		Rejected = 2,
		Offline = 3
	}

	public sealed class Invitation
	{
		public Invitation (Group group, Person person, InvitationResponse response)
		{
			if (group == null)
				throw new ArgumentNullException ("group");
			if (person == null)
				throw new ArgumentNullException ("person");
			if (!Enum.IsDefined (typeof (InvitationResponse), response))
				throw new ArgumentException ("Invalid value for response", "response");

			Group = group;
			Person = person;
			Response = response;
		}

		public Group Group
		{
			get;
			private set;
		}

		public Person Person
		{
			get;
			private set;
		}

		public InvitationResponse Response
		{
			get;
			private set;
		}
	}
}