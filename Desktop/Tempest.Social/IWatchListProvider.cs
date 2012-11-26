//
// IWatchListProvider.cs
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
using System.Threading.Tasks;

namespace Tempest.Social
{
	public interface IWatchListProvider
	{
		/// <summary>
		/// Asynchronously adds an identifier to <paramref name="listOwner"/>'s buddy list.
		/// </summary>
		/// <param name="listOwner">The list owner.</param>
		/// <param name="target">The person to add.</param>
		/// <exception cref="ArgumentNullException"><paramref name="listOwner"/> or <paramref name="target"/> are <c>null</c>.</exception>
		Task AddAsync (Person listOwner, Person target);
		
		/// <summary>
		/// Asynchronously adds a set of identifiers to <paramref name="listOwner"/>'s buddy list.
		/// </summary>
		/// <param name="listOwner">The list owner.</param>
		/// <param name="targets">The people to add.</param>
		/// <exception cref="ArgumentNullException"><paramref name="listOwner"/> or <paramref name="targets"/> are <c>null</c>.</exception>
		Task AddRangeAsync (Person listOwner, IEnumerable<Person> targets);

		/// <summary>
		/// Asynchronously removes an identifier to <paramref name="listOwner"/>'s buddy list.
		/// </summary>
		/// <param name="listOwner">The <see cref="Person.Identity"/> of the list owner.</param>
		/// <param name="target">The <see cref="Person.Identity"/> of the person to remove.</param>
		/// <exception cref="ArgumentNullException"><paramref name="listOwner"/> or <paramref name="target"/> are <c>null</c>.</exception>
		Task RemoveAsync (string listOwner, string target);

		/// <summary>
		/// Asynchronously retrieves the identifiers watched by <paramref name="listOwner"/>'s.
		/// </summary>
		/// <param name="listOwner">The <see cref="Person.Identity"/> of the person to retrieve the list for.</param>
		/// <returns>A future to an enumerable of buddies.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="listOwner"/> is <c>null</c>.</exception>
		Task<IEnumerable<Person>> GetWatchedAsync (string listOwner);


		/// <summary>
		/// Asynchronously retrieves the identifiers watching <paramref name="target"/>.
		/// </summary>
		/// <param name="target">The <see cref="Person.Identity"/> of the person being watched.</param>
		/// <returns>A future to an enumerable of identities who have <paramref name="target"/> on their watch list.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
		Task<IEnumerable<Person>> GetWatchersAsync (string target);
	}
}