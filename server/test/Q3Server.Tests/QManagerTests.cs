using System;
using Q3;
using Xunit;

namespace Q3Tests
{
    public class QManagerTests
    {
        private QManager manager = new QManager();

        [Fact]
        public void ICanCreateANewQueue()
        {
            Assert.NotNull(manager.CreateQueue("newQueue"));
        }

        [Fact]
        public void ICannotCreateADuplicateQueue()
        {
            manager.CreateQueue("dupe");
            Assert.Throws<DuplicateQueueException>(() => manager.CreateQueue("dupe"));
        }

        [Fact]
        public void CreatingAQueueFiresTheEvent()
        {
            bool eventFired = false;
            manager.queueCreated += (s, e) => eventFired = true;
            manager.CreateQueue("event");
            Assert.True(eventFired);
        }
    }
}
