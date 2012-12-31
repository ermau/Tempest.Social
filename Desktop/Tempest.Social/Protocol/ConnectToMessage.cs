//
// ConnectToMessage.cs
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
	public class ConnectToMessage
		: SocialMessage
	{
		public ConnectToMessage()
			: base (SocialMessageType.ConnectTo)
		{
		}

		public ConnectToMessage (string id, bool youreHosting, Target target)
			: this()
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			if (target == null)
				throw new ArgumentNullException ("target");

			Id = id;
			YoureHosting = youreHosting;
			Target = target;
		}

		/// <summary>
		/// Gets or sets whether this connection client is to be the host.
		/// </summary>
		public bool YoureHosting
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ID of the person to connect to.
		/// </summary>
		public string Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the target to connect to or receive a connection from.
		/// </summary>
		public Target Target
		{
			get;
			set;
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteString (Id);
			writer.WriteBool (YoureHosting);
			writer.WriteString (Target.Hostname);
			writer.WriteInt32 (Target.Port);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			Id = reader.ReadString();
			YoureHosting = reader.ReadBool();
			Target = new Target (reader.ReadString(), reader.ReadInt32());
		}
	}
}
