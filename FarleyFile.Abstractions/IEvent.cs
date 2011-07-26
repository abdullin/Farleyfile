using System;
using System.Runtime.Serialization;

namespace FarleyFile
{
    public interface IEvent : IBaseMessage {}

    [DataContract]
    public class Identity : IComparable
    {
        [DataMember(Order = 1)]
        public Guid Id { get; protected set; }
        [DataMember(Order = 2)]
        public int Tag { get; protected set; }

        public bool IsEmpty
        {
            get { return Id == Guid.Empty; }
        }

        public static readonly Identity Empty = new Identity();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            var identity = obj as Identity;
            if (ReferenceEquals(identity, null))
            {
                return false;
            }

            return identity.Id.Equals(Id) && identity.Tag == Tag;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ Tag;
            }
        }

        public int CompareTo(object obj)
        {
            var id = obj as Identity;

            if (id == null)
            {
                throw new ArgumentException("These types are not comparable");
            }
            if (Tag != id.Tag)
                return Tag.CompareTo(id.Tag);
            return Id.CompareTo(id.Id);
        }


        public static implicit operator Guid(Identity id)
        {
            return id.Id;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})-{2}", GetType().Name.Replace("Id", ""), Tag, Id);
        }

        public static bool operator ==(Identity a, Identity b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(Identity a, Identity b)
        {
            if (ReferenceEquals(a, null))
                return !ReferenceEquals(b, null);
            return !a.Equals(b);
        }
    }

    [DataContract]
    public sealed class NoteId : Identity
    {
        public const int TagId = 1;
        public NoteId(Guid id)
        {
            Tag = TagId;
            Id = id;
        }
        
        NoteId() {}
    }
    [DataContract]
    public sealed class StoryId : Identity
    {
        public const int TagId = 2;
        public StoryId(Guid id)
        {
            Id = id;
            Tag = TagId;
        }
        
        StoryId() {}
    }
    [DataContract]
    public sealed class ActivityId : Identity
    {
        public const int TagId = 3;
        public ActivityId(Guid id)
        {
            Tag = TagId;
            Id = id;
        }

        
        ActivityId() {}
    }


    [DataContract]
    public sealed class TaskId : Identity
    {
        public const int TagId = 4;
        public TaskId(Guid id)
        {
            Tag = TagId;
            Id = id;
        }
        
        TaskId() {}
    }

    [DataContract]
    public sealed class TagId : Identity
    {
        public const int TagIdValue = 7;
        public TagId(Guid id)
        {
            Tag = TagIdValue;
            Id = id;
        }


        TagId() { }
    }
}