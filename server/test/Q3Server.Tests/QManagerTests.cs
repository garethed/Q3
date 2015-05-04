using System;
using Q3Server;
using Xunit;
using Moq;
using System.Linq;

namespace Q3Tests
{
    public class QManagerTests
    {
        private QManager manager;
        private bool membershipChangedEventFired = false;
        private bool statusChangedEventFired = false;

        public QManagerTests()
        {
            var listener = new Mock<IQEventsListener>();
            manager = new QManager(listener.Object);
        }

        [Fact]
        public void ICanCreateANewQueue()
        {
            Assert.NotNull(manager.CreateQueue("newQueue", "me"));
        }

        [Fact]
        public void ICannotCreateADuplicateQueue()
        {
            manager.CreateQueue("dupe", "me");
            Assert.Throws<DuplicateQueueException>(() => manager.CreateQueue("dupe", "me"));
        }

        [Fact]
        public void CreatingAQueueFiresTheEvent()
        {
            bool eventFired = false;
            manager.queueCreated += (s, e) => eventFired = true;
            manager.CreateQueue("event", "me");
            Assert.True(eventFired);
        }

        [Fact]
        public void CreatedQueueIsInTheListOfQueues()
        {
            manager.CreateQueue("new", "me");
            Assert.Contains(manager.ListQueues(), q => q.Name == "new");
        }

        [Fact]
        public void ICannotGetAQueueThatDoesntExist()
        {
            Assert.Throws<QueueNotFoundException>(() => manager.GetQueue(0));
        }

        [Fact]
        public void ICanGetAQueueThatDoesExist()
        {
            var q = manager.CreateQueue("queue", "me");
            Assert.Equal("queue", q.Name);            
        }

        [Fact]
        public void NewQueueShouldContainTheStartingUser()
        {
            var user = "me";
            var q = manager.CreateQueue("queue", user);
            Assert.Equal(user, q.Members.First());

        }

        [Fact]
        public void ICanCloseAQueue()
        {
            var q = createQueue();

            manager.CloseQueue(q.Id);

            Assert.Equal(QueueStatus.Closed, q.Status);
            Assert.True(statusChangedEventFired);
        }

        [Fact]
        public void ICanLeaveAQueue()
        {
            var q = createQueue();
            q.RemoveUser("me");

            Assert.Empty(q.Members);
            Assert.True(membershipChangedEventFired);
        }

        [Fact]
        public void ICanJoinAQueue()
        {
            var q = createQueue();
            q.AddUser("him");

            Assert.Contains("him", q.Members);
            Assert.True(membershipChangedEventFired);
        }

        [Fact]
        public void JoiningTwiceHasNoEffect()
        {
            var q = createQueue();
            q.AddUser("me");

            Assert.False(membershipChangedEventFired);
        }

        [Fact]
        public void LeavingTwiceHasNoEffect()
        {
            var q = createQueue();
            q.RemoveUser("her");

            Assert.False(membershipChangedEventFired);
        }

        private Queue createQueue()
        {
            var q = manager.CreateQueue("q", "me");
            q.QueueMembershipChanged += (s,e) => membershipChangedEventFired = true;
            q.QueueStatusChanged += (s, e) => statusChangedEventFired = true;
            return q;
        }
    }
}
