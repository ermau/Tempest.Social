using System;
using System.Net;

namespace Tempest.Social
{
	public class RequestConnectEventArgs
		: PersonEventArgs
	{
		public RequestConnectEventArgs (Person person)
			: base (person)
		{
		}

		/// <summary>
		/// Gets or sets whether the connection is accepted.
		/// </summary>
		public bool Accept
		{
			get;
			set;
		}
	}

	public class ConnectEventArgs
		: PersonEventArgs
	{
		public ConnectEventArgs (Person person, EndPoint endPoint, bool youreHosting)
			: base (person)
		{
			if (endPoint == null)
				throw new ArgumentNullException ("endPoint");

			EndPoint = endPoint;
			YoureHosting = youreHosting;
		}

		public EndPoint EndPoint
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets whether or not the server picked you as the host.
		/// </summary>
		public bool YoureHosting
		{
			get;
			private set;
		}
	}

	public class PersonEventArgs
		: EventArgs
	{
		public PersonEventArgs (Person person)
		{
			if (person == null)
				throw new ArgumentNullException ("person");

			Person = person;
		}

		public Person Person
		{
			get;
			private set;
		}
	}
}