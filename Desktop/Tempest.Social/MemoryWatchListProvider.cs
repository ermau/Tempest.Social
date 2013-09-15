//
// MemoryWatchListProvider.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadenza.Collections;

namespace Tempest.Social
{
	public class MemoryWatchListProvider
		: IWatchListProvider
	{
		public Task AddAsync (Person listOwner, Person target)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");
			if (target == null)
				throw new ArgumentNullException ("target");

			lock (this.watchLists)
			{
				this.people[listOwner.Identity] = listOwner;
				this.people[target.Identity] = target;
				this.watchLists.Add (listOwner.Identity, target.Identity);
			}

			return true.CreateTask();
		}

		public Task AddRangeAsync (Person listOwner, IEnumerable<Person> targets)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");
			if (targets == null)
				throw new ArgumentNullException ("targets");

			lock (this.watchLists)
			{
				this.people[listOwner.Identity] = listOwner;

				foreach (Person target in targets)
				{
					this.people[target.Identity] = target;
					this.watchLists.Add (listOwner.Identity, target.Identity);
				}
			}

			return true.CreateTask();
		}

		public Task RemoveAsync (string listOwner, string target)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");
			if (target == null)
				throw new ArgumentNullException ("target");

			lock (this.watchLists)
				this.watchLists.Remove (listOwner, target);

			return true.CreateTask();
		}

		public Task ClearAsync (string listOwner)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");

			lock (this.watchLists)
				this.watchLists.Remove (listOwner);

			return true.CreateTask();
		}

		public Task<bool> GetIsWatcherAsync (string watcher, string watchee)
		{
			if (watcher == null)
				throw new ArgumentNullException ("watcher");
			if (watchee == null)
				throw new ArgumentNullException ("watchee");

			bool isWatcher;
			lock (this.watchLists)
				isWatcher = this.watchLists[watcher].Contains (watchee);

			return isWatcher.CreateTask();
		}

		public Task<IEnumerable<Person>> GetWatchedAsync (string listOwner)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");

			Person[] buddies;
			lock (this.watchLists)
				buddies = this.watchLists[listOwner].Select (s => people[s]).ToArray();

			return buddies.CreateTask<IEnumerable<Person>>();
		}

		public Task<IEnumerable<Person>> GetWatchersAsync (string target)
		{
			if (target == null)
				throw new ArgumentNullException ("target");

			Person[] owners;
			lock (this.watchLists)
				owners = this.watchLists.Inverse[target].Select (s => people[s]).ToArray();

			return owners.CreateTask<IEnumerable<Person>>();
		}

		private readonly Dictionary<string, Person> people = new Dictionary<string, Person>();
		private readonly BidirectionalLookup<string, string> watchLists = new BidirectionalLookup<string, string>();
	}
}