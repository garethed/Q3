using System;
using Q3Server;
using Xunit;
using Moq;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace Q3Tests
{
    public class QHubTests
    {
        private QHub hub;

        public QHubTests()
        {
            var manager = new Mock<IQManager>();
            var userAccessor = new Mock<IUserAccessor>();
            userAccessor.Setup(c => c.User).Returns(new User("test;testy test;test@test.test"));
            hub = new QHub(manager.Object, userAccessor.Object);
        }

        [Fact]
        public void ICanStartAQueue()
        {
            hub.StartQueue("New Queue", null);
        }
    }
}
