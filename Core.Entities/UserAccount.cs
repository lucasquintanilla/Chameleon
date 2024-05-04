using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class UserAccount
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public string Username { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Follow> Followers { get; set; }
        public virtual ICollection<Follow> Following { get; set; }
    }

    public class Like
    {
        public int Id { get; set; }
        public Guid UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class Follow
    {
        public int Id { get; set; }
        public Guid FollowerId { get; set; }
        public UserAccount Follower { get; set; }
        public Guid FollowingId { get; set; }
        public UserAccount Following { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Currency { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }

    public class Subscription
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int PlanId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }        
        public UserAccount UserAccount { get; set; }
        public SubscriptionPlan Plan { get; set; }
    }

    public class AccountSettings
    {
        public int Id { get; set; }
        public int MyProperty { get; set; }
    }

    public class SearchHistory
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
