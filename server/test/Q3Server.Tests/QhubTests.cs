using System;
using Q3;
using Xunit;

namespace Q3Tests
{
    public class Class1
    {
        private QHub hub = new QHub();

        [Fact]
        public void ICanStartAQueue()
        {
            hub.StartQueue("New Queue");
        }
    }
}
