//
// ConnectEventArgs.cs
//
// Author:
//   Eric Maupin <me@ermau.com>
//
// Copyright (c) 2012 Eric Maupin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

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

		/// <summary>
		/// Gets or sets the targe the other party should connect to.
		/// </summary>
		public Target Target
		{
			get;
			set;
		}
	}

	public class ConnectEventArgs
		: PersonEventArgs
	{
		public ConnectEventArgs (Person person, Target target, bool youreHosting)
			: base (person)
		{
			if (target == null)
				throw new ArgumentNullException ("target");

			Target = target;
			YoureHosting = youreHosting;
		}

		public Target Target
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