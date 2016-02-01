namespace TomsToolbox.Desktop.Tests
{
    using System;
    using System.Runtime.Serialization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DispatcherExtensionsTests
    {
        [TestMethod]
        public void DispatcherExtensions_InvokePassesExceptionsTest()
        {
            try
            {
                using (var thread1 = new ForegroundThreadWithDispatcher("Test1", System.Threading.ApartmentState.MTA))
                {
                    thread1.Dispatcher.Invoke(() => { throw new TestException(); });
                }

                Assert.Fail("We should never get here");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(TestException));
            }
        }

        [TestMethod]
        public void DispatcherExtensions_InvokePassesExceptionsOnSameThreadTest()
        {
            try
            {
                using (var thread1 = new ForegroundThreadWithDispatcher("Test1", System.Threading.ApartmentState.MTA))
                {
                    var t = thread1;
                    thread1.Dispatcher.Invoke(() => t.Invoke(() => { throw new TestException(); }));
                }

                Assert.Fail("We should never get here");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(TestException));
            }
        }

        [Serializable]
        class TestException : Exception
        {
            public TestException()
            {
            }

            public TestException(string message)
                : base(message)
            {
            }

            public TestException(string message, Exception inner)
                : base(message, inner)
            {
            }

            protected TestException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
