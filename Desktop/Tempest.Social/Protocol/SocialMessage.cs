//
// SocialMessage.cs
//
// Author:
//   Eric Maupin <me@ermau.com>
//
// Copyright (c) 2012-2013 Eric Maupin
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
using TypePair = System.Collections.Generic.KeyValuePair<System.Type,System.Func<Tempest.Message>>;

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

		/// <summary>
		/// Request to be connected with another user.
		/// </summary>
		//ConnectRequest = 4,
		//ConnectResult = 5,
		//ConnectTo = 6,
		
		/// <summary>
		/// Search for a person
		/// </summary>
		Search = 7,

		/// <summary>
		/// Search results
		/// </summary>
		SearchResult = 8,

		/// <summary>
		/// Create a group
		/// </summary>
		CreateGroup = 9,

		/// <summary>
		/// Invite people to a group
		/// </summary>
		InviteToGroup = 10,
		
		/// <summary>
		/// A received invitation to a group
		/// </summary>
		GroupInvite = 11,

		/// <summary>
		/// A response to a received group invitation
		/// </summary>
		GroupInviteResponse = 12,
		
		/// <summary>
		/// Leave a group
		/// </summary>
		LeaveGroup = 13,

		/// <summary>
		/// The group was updated
		/// </summary>
		GroupUpdate = 14,
		
		GroupConnection = 15,

		/// <summary>
		/// Text message
		/// </summary>
		Text = 16,

		ConnectionInfo = 18,
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
		public static readonly Protocol Instance = new Protocol (2, 2);
		public const int DefaultPort = 42920;

		static SocialProtocol()
		{
			Instance.Register(new [] {
				new TypePair (typeof(BuddyListMessage), () => new BuddyListMessage()),
				new TypePair (typeof(CreateGroupMessage), () => new CreateGroupMessage()),
				new TypePair (typeof(GroupConnectionMessage), () => new GroupConnectionMessage()), 
				new TypePair (typeof(GroupInviteMessage), () => new GroupInviteMessage()),
				new TypePair (typeof(GroupInviteResponseMessage), () => new GroupInviteResponseMessage()),
				new TypePair (typeof(GroupUpdateMessage), () => new GroupUpdateMessage()),
				new TypePair (typeof(InviteToGroupMessage), () => new InviteToGroupMessage()),
				new TypePair (typeof(LeaveGroupMessage), () => new LeaveGroupMessage()),
				new TypePair (typeof(PersonMessage), () => new PersonMessage()),
				new TypePair (typeof(RequestBuddyListMessage), () => new RequestBuddyListMessage()),
				new TypePair (typeof(SearchMessage), () => new SearchMessage()),
				new TypePair (typeof(SearchResultMessage), () => new SearchResultMessage()),
				new TypePair (typeof(TextMessage), () => new TextMessage()),
				new TypePair (typeof(ConnectionInfoMessage), () => new ConnectionInfoMessage()),
			});
		}
	}
}