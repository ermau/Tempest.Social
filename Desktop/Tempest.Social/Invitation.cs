﻿//
// Invitation.cs
//
// Author:
//   Eric Maupin <me@ermau.com>
//
// Copyright (c) 2013 Eric Maupin
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