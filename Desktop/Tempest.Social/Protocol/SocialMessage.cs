//
// SocialMessage.cs
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

namespace Tempest.Social
{
	public enum SocialMessageType
		: ushort
	{
		/// <summary>
		/// Sent from the server to client to indicate it does not have a buddy list for this person.
		/// </summary>
		RequestBuddyList = 1,
		
		/// <summary>
		/// Sent from the server or client to update buddy lists.
		/// </summary>
		BuddyList = 2,
		
		/// <summary>
		/// Updates a person's attributes, including your own.
		/// </summary>
		Person = 3,
		
		EndPointRequest = 4,
		
		EndPointResult = 5
	}

	public abstract class SocialMessage
		: Message
	{
		protected SocialMessage (SocialMessageType type)
			: base (SocialProtocol.Instance, (ushort)type)
		{
		}
	}

	public static class SocialProtocol
	{
		public static readonly Protocol Instance = new Protocol (2, 1);
		public const int DefaultPort = 42920;

		static SocialProtocol()
		{
			Instance.DiscoverFromAssemblyOf<SocialMessage>();
		}
	}
}