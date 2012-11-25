//
// WatchListProviderTests.cs
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
using NUnit.Framework;

namespace Tempest.Social.Tests
{
	public abstract class WatchListProviderTests
	{
		protected abstract IWatchListProvider GetProvider();

		[Test]
		public void AddAsyncInvalid()
		{
			IWatchListProvider provider = GetProvider();

			Assert.Throws<ArgumentNullException> (() => provider.AddAsync (null, "buddyId"));
			Assert.Throws<ArgumentNullException> (() => provider.AddAsync ("ownerId", null));
		}

		[Test]
		public void AddAsync()
		{
			IWatchListProvider provider = GetProvider();

			const string owner = "identity1";
			const string buddy1 = "buddy1";
			const string buddy2 = "buddy2";
			
			provider.AddAsync (owner, buddy1).Wait();
			provider.AddAsync (owner, buddy2).Wait();

			IEnumerable<string> buddies = provider.GetWatchedAsync (owner).Result;
			CollectionAssert.Contains (buddies, buddy1);
			CollectionAssert.Contains (buddies, buddy2);
		}

		[Test]
		public void AddRangeAsyncInvalid()
		{
			IWatchListProvider provider = GetProvider();

			Assert.Throws<ArgumentNullException> (() => provider.AddRangeAsync (null, Enumerable.Empty<string>()));
			Assert.Throws<ArgumentNullException> (() => provider.AddRangeAsync ("ownerId", null));
		}

		[Test]
		public void AddRangeAsync()
		{
			IWatchListProvider provider = GetProvider();

			const string owner = "identity1";
			const string buddy1 = "buddy1";
			const string buddy2 = "buddy2";
			
			provider.AddRangeAsync (owner, new[] { buddy1, buddy2 }).Wait();

			IEnumerable<string> buddies = provider.GetWatchedAsync (owner).Result;
			CollectionAssert.Contains (buddies, buddy1);
			CollectionAssert.Contains (buddies, buddy2);
		}

		[Test]
		public void RemoveAsyncInvalid()
		{
			IWatchListProvider provider = GetProvider();

			Assert.Throws<ArgumentNullException> (() => provider.RemoveAsync (null, "buddyId"));
			Assert.Throws<ArgumentNullException> (() => provider.RemoveAsync ("ownerId", null));
		}

		[Test]
		public void RemoveAsync()
		{
			IWatchListProvider provider = GetProvider();

			const string owner = "identity1";
			const string buddy1 = "buddy1";
			const string buddy2 = "buddy2";
			
			provider.AddAsync (owner, buddy1).Wait();
			provider.AddAsync (owner, buddy2).Wait();
			
			provider.RemoveAsync (owner, buddy1).Wait();
			
			IEnumerable<string> buddies = provider.GetWatchedAsync (owner).Result;
			CollectionAssert.DoesNotContain (buddies, buddy1);
			CollectionAssert.Contains (buddies, buddy2);
		}

		[Test]
		public void GetWatchedAsyncInvalid()
		{
			IWatchListProvider provider = GetProvider();

			Assert.Throws<ArgumentNullException> (() => provider.GetWatchedAsync (null));
		}

		[Test]
		public void GetWatchedAsyncEmpty()
		{
			IWatchListProvider provider = GetProvider();

			IEnumerable<string> buddies = provider.GetWatchedAsync ("identity").Result;
			Assert.IsNotNull (buddies);
			CollectionAssert.IsEmpty (buddies);
		}

		[Test]
		public void GetWatchersAsyncInvalid()
		{
			IWatchListProvider provider = GetProvider();

			Assert.Throws<ArgumentNullException> (() => provider.GetWatchersAsync (null));
		}

		[Test]
		public void GetWatchersAsyncEmpty()
		{
			IWatchListProvider provider = GetProvider();

			IEnumerable<string> buddies = provider.GetWatchersAsync ("identity").Result;
			Assert.IsNotNull (buddies);
			CollectionAssert.IsEmpty (buddies);
		}

		[Test]
		public void GetWatchers()
		{
			IWatchListProvider provider = GetProvider();

			const string A = "identity1";
			const string B = "buddy1";
			const string C = "buddy2";
			
			provider.AddRangeAsync (A, new[] { B, C }).Wait();
			provider.AddRangeAsync (B, new[] { A, C }).Wait();

			IEnumerable<string> watchers = provider.GetWatchersAsync (C).Result;
			CollectionAssert.Contains (watchers, A);
			CollectionAssert.Contains (watchers, B);
		}
	}
}