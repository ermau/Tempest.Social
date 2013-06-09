//
// Person.cs
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tempest.Social
{
	public enum Status
		: byte
	{
		Offline = 0,
		Online = 1,
		Away = 2,
	}

	public sealed class Person
		: ISerializable, INotifyPropertyChanged, IEquatable<Person>
	{
		public Person (string identity)
		{
			if (identity == null)
				throw new ArgumentNullException ("identity");
			if (identity.Trim() == String.Empty)
				throw new ArgumentException ("Identity can not be empty", "identity");

			Identity = identity;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		internal Person (ISerializationContext context, IValueReader reader)
		{
			Deserialize (context, reader);
		}

		public string Nickname
		{
			get { return this.nickname; }
			set
			{
				if (this.nickname == value)
					return;

				this.nickname = value;
				OnPropertyChanged();
			}
		}

		public string Identity
		{
			get;
			private set;
		}

		public Status Status
		{
			get { return this.status; }
			set
			{
				if (this.status == value)
					return;

				this.status = value;
				OnPropertyChanged();
			}
		}

		public void Serialize (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteString (Identity);
			writer.WriteString (Nickname);
			writer.WriteByte ((byte)Status);
		}

		public void Deserialize (ISerializationContext context, IValueReader reader)
		{
			Identity = reader.ReadString();
			Nickname = reader.ReadString();
			Status = (Status)reader.ReadByte();
		}

		public override bool Equals (object obj)
		{
			if (ReferenceEquals (null, obj))
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			return obj is Person && Equals ((Person)obj);
		}

		public bool Equals (Person other)
		{
			if (ReferenceEquals (null, other))
				return false;
			if (ReferenceEquals (this, other))
				return true;
			return string.Equals (this.nickname, other.nickname) && this.status == other.status && string.Equals (this.Identity, other.Identity);
		}

		public override int GetHashCode()
		{
			unchecked {
				int hashCode = (this.nickname != null ? this.nickname.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (int)this.status;
				hashCode = (hashCode * 397) ^ (this.Identity != null ? this.Identity.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator == (Person left, Person right)
		{
			return Equals (left, right);
		}

		public static bool operator != (Person left, Person right)
		{
			return !Equals (left, right);
		}

		private string nickname;
		private Status status;

		private void OnPropertyChanged ([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler (this, new PropertyChangedEventArgs (propertyName));
		}
	}
}