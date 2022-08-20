// ReSharper disable UnusedMember.Local
namespace TomsToolbox.Wpf.Tests;

using System;
using System.Runtime.Serialization;

using Xunit;
using Xunit.Sdk;

public class DispatcherExtensionsTests
{
    [Fact]
    public void DispatcherExtensions_InvokePassesExceptionsTest()
    {
        try
        {
            using (var thread1 = new ForegroundThreadWithDispatcher("Test1"))
            {
                thread1.Dispatcher.Invoke(() => throw new TestException());
            }

            throw new XunitException("We should never get here");
        }
        catch (Exception ex)
        {
            Assert.Equal(typeof(TestException), ex.GetType());
        }
    }

    [Fact]
    public void DispatcherExtensions_InvokePassesExceptionsOnSameThreadTest()
    {
        try
        {
            using (var thread1 = new ForegroundThreadWithDispatcher("Test1"))
            {
                var t = thread1;
                thread1.Dispatcher.Invoke(() => t.Invoke(() => throw new TestException()));
            }

            throw new XunitException("We should never get here");
        }
        catch (Exception ex)
        {
            Assert.Equal(typeof(TestException), ex.GetType());
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
