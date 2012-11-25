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
		public Task AddAsync (string listOwner, string buddy)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");
			if (buddy == null)
				throw new ArgumentNullException ("buddy");

			lock (this.buddyLists)
				this.buddyLists.Add (listOwner, buddy);

			return Task.FromResult (true);
		}

		public Task AddRangeAsync (string listOwner, IEnumerable<string> buddies)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");
			if (buddies == null)
				throw new ArgumentNullException ("buddies");

			lock (this.buddyLists)
				this.buddyLists.Add (listOwner, buddies);

			return Task.FromResult (true);
		}

		public Task RemoveAsync (string listOwner, string buddyId)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");
			if (buddyId == null)
				throw new ArgumentNullException ("buddyId");

			lock (this.buddyLists)
				this.buddyLists.Remove (listOwner, buddyId);

			return Task.FromResult (true);
		}

		public Task<IEnumerable<string>> GetWatchedAsync (string listOwner)
		{
			if (listOwner == null)
				throw new ArgumentNullException ("listOwner");

			string[] buddies;
			lock (this.buddyLists)
				buddies = this.buddyLists[listOwner].ToArray();

			return Task.FromResult<IEnumerable<string>> (buddies);
		}

		public Task<IEnumerable<string>> GetWatchersAsync (string target)
		{
			if (target == null)
				throw new ArgumentNullException ("target");

			string[] owners;
			lock (this.buddyLists)
				owners = this.buddyLists.Inverse[target].ToArray();

			return Task.FromResult<IEnumerable<string>> (owners);
		}

		private readonly BidirectionalLookup<string, string> buddyLists = new BidirectionalLookup<string, string>();
	}
}