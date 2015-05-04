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
            var q = manager.CreateQueue("shorLived", "me");
            var eventFired = false;
            q.QueueStatusChanged += (s,e) => eventFired = true;

            manager.CloseQueue(q.Id);

            Assert.Equal(QueueStatus.Closed, q.Status);
            Assert.True(eventFired);
        }
    }
}
