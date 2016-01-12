using System;

namespace hacks.modelling.value_objects
{
    public sealed class AccountId : IEquatable<AccountId>
    {
        private readonly Guid _id;

        public static AccountId NewAccount()
        {
            return new AccountId(SeqGuid.NewGuid());
        }

        private AccountId(Guid id)
        {
            if(Guid.Empty == id)throw new ArgumentOutOfRangeException("Id cannot be empty");
            _id = id;
        }

        public static explicit operator Guid(AccountId accountId)
        {
            return accountId._id;
        }

        //alt-ins R# to code gen equality override
        public bool Equals(AccountId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AccountId && Equals((AccountId) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(AccountId left, AccountId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AccountId left, AccountId right)
        {
            return !Equals(left, right);
        }


    }
}