//
// ConnectResultMessage.cs
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
	public enum ConnectResult
		: byte
	{
		FailedUnknown = 0,
		Success = 1,

		/// <summary>
		/// The person you tried to connec to was not found.
		/// </summary>
		FailedNotFound = 2,

		/// <summary>
		/// You are not following the person you tried to connect to.
		/// </summary>
		FailedNotFollowing = 3,

		/// <summary>
		/// The other party rejected the connection.
		/// </summary>
		FailedRejected = 4
	}

	public class ConnectResultMessage
		: SocialMessage
	{
		public ConnectResultMessage()
			: base (SocialMessageType.ConnectResult)
		{
		}

		public ConnectResultMessage (ConnectResult result, Target target)
			: this()
		{
			if (!Enum.IsDefined (typeof (ConnectResult), result))
				throw new ArgumentException ("result");
			if (result == ConnectResult.Success && target == null)
				throw new ArgumentNullException ("target");

			Result = result;
			Target = target;
		}

		public ConnectResult Result
		{
			get;
			set;
		}

		public Target Target
		{
			get;
			set;
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteByte ((byte)Result);
			if (writer.WriteBool (Target != null))
				Target.Serialize (context, writer);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			Result = (ConnectResult)reader.ReadByte();
			if (reader.ReadBool())
				Target = new Target (context, reader);
		}
	}
}