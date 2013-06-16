//
// GroupManager.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tempest.Social
{
	public sealed class GroupManager
	{
		public event Action<Group> GroupUpdated;

		public Group CreateGroup (Person person)
		{
			if (person == null)
				throw new ArgumentNullException ("person");

			while (this.groups.ContainsKey (this.nextId))
				Interlocked.Increment (ref this.nextId);

			var group = new Group (this.nextId, new[] { person.Identity });
			this.groups.Add (group.Id, group);

			return group;
		}

		public void JoinGroup (Group group, Person person)
		{
			if (group == null)
				throw new ArgumentNullException ("group");
			if (person == null)
				throw new ArgumentNullException ("person");

			if (!this.groups.TryGetValue (group.Id, out group))
				return;

			group.Participants.Add (person.Identity);
			OnGroupUpdated (group);
		}

		public void LeaveGroup (Group group, Person person)
		{
			if (person == null)
				throw new ArgumentNullException ("person");
			if (group == null)
				throw new ArgumentNullException ("group");

			if (!this.groups.TryGetValue (group.Id, out group))
				return;

			if (!group.Participants.Remove (person.Identity))
				return;

			OnGroupUpdated (group);
			if (group.Participants.Count == 0)
				this.groups.Remove (group.Id);
		}

		public bool TryGetGroup (int groupId, out Group group)
		{
			return this.groups.TryGetValue (groupId, out group);
		}

		private int nextId;
		private readonly Dictionary<int, Group> groups = new Dictionary<int, Group>();

		private void OnGroupUpdated (Group group)
		{
			var updated = GroupUpdated;
			if (updated != null)
				updated (group);
		}
	}
}