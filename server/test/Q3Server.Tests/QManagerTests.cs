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
        private User user = new User("me;me myself and i;me@me.com");
        private User user2 = new User("him;he him;him@there.com");


        public QManagerTests()
        {
            var listener = new Mock<IQEventsListener>();
            manager = new QManager(listener.Object);            
        }

        [Fact]
        public void ICanCreateANewQueue()
        {
            Assert.NotNull(createQueue());
        }

        [Fact]
        public void ICannotCreateADuplicateQueue()
        {
            createQueue();
            Assert.Throws<DuplicateQueueException>(() => createQueue());
        }

        [Fact]
        public void CreatingAQueueFiresTheEvent()
        {
            bool eventFired = false;
            manager.queueCreated += (s, e) => eventFired = true;
            manager.CreateQueue("event", null, user);
            Assert.True(eventFired);
        }

        [Fact]
        public void CreatedQueueIsInTheListOfQueues()
        {
            var queue = createQueue();
            Assert.Contains(manager.ListQueues(), q => q.Name == queue.Name);
        }

        [Fact]
        public void ICannotGetAQueueThatDoesntExist()
        {
            Assert.Throws<QueueNotFoundException>(() => manager.GetQueue(0));
        }

        [Fact]
        public void ICanGetAQueueThatDoesExist()
        {
            var q = createQueue();
            Assert.Equal("q", q.Name);            
        }

        [Fact]
        public void NewQueueShouldContainTheStartingUser()
        {
            var q = createQueue();
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
            q.RemoveUser(user);

            Assert.Empty(q.Members);
            Assert.True(membershipChangedEventFired);
        }

        [Fact]
        public void ICanJoinAQueue()
        {
            
            var q = createQueue();
            q.AddUser(user2);

            Assert.Contains(user2, q.Members);
            Assert.True(membershipChangedEventFired);
        }

        [Fact]
        public void JoiningTwiceHasNoEffect()
        {
            var q = createQueue();
            q.AddUser(user);

            Assert.False(membershipChangedEventFired);
        }

        [Fact]
        public void LeavingTwiceHasNoEffect()
        {
            var q = createQueue();
            q.RemoveUser(user2);

            Assert.False(membershipChangedEventFired);
        }

        private Queue createQueue()
        {
            var q = manager.CreateQueue("q", null, user);
            q.QueueMembershipChanged += (s,e) => membershipChangedEventFired = true;
            q.QueueStatusChanged += (s, e) => statusChangedEventFired = true;
            return q;
        }
    }
}
