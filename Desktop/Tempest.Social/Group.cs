//
// Group.cs
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tempest.Social
{
	public class Group
		: INotifyPropertyChanged
	{
		public Group (int id)
		{
			Id = id;
		}

		public Group (int id, string ownerId)
			: this (id)
		{
			if (ownerId == null)
				throw new ArgumentNullException ("ownerId");

			OwnerId = ownerId;
			this.participants.Add (ownerId);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public int Id
		{
			get;
			private set;
		}

		public string OwnerId
		{
			get { return this.ownerId; }
			set
			{
				if (this.ownerId == value)
					return;

				this.ownerId = value;
				OnPropertyChanged();
			}
		}

		public ICollection<string> Participants
		{
			get { return this.participants; }
		}

		private readonly ObservableCollection<string> participants = new ObservableCollection<string>();
		private string ownerId;	

		protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler (this, new PropertyChangedEventArgs (propertyName));
		}
	}

	public class GroupSerializer
		: ISerializer<Group>
	{
		public static readonly GroupSerializer Instance = new GroupSerializer();

		public void Serialize (ISerializationContext context, IValueWriter writer, Group element)
		{
			writer.WriteInt32 (element.Id);
			writer.WriteString (element.OwnerId);
			writer.WriteEnumerable (context, Serializer<string>.Default, element.Participants);
		}

		public Group Deserialize (ISerializationContext context, IValueReader reader)
		{
			var g = new Group (reader.ReadInt32(), reader.ReadString());
			foreach (string participant in reader.ReadEnumerable (context, Serializer<string>.Default))
				g.Participants.Add (participant);

			return g;
		}
	}
}