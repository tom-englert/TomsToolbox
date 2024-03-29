﻿namespace TomsToolbox.Essentials.Tests;

using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

using Xunit;

public class TaskSchedulerTests
{
    [Fact]
    public void ThreadBoundTaskSchedulerTest()
    {
        var stack = new ConcurrentStack<string>();
        var thisId = Thread.CurrentThread.ManagedThreadId;
        int schedulerId;

        using (var taskScheduler = new ThreadBoundTaskScheduler())
        {
            var factory = taskScheduler.TaskFactory;
            schedulerId = taskScheduler.ThreadId;

            factory.StartNew(() => { stack.Push("1: " + Thread.CurrentThread.ManagedThreadId); Thread.Sleep(100); });
            factory.StartNew(() => { stack.Push("2: " + Thread.CurrentThread.ManagedThreadId); Thread.Sleep(100); });
            factory.StartNew(() => { stack.Push("3: " + Thread.CurrentThread.ManagedThreadId); Thread.Sleep(100); });
            factory.StartNew(() => { stack.Push("4: " + Thread.CurrentThread.ManagedThreadId); Thread.Sleep(100); });
        }

        Assert.Equal(4, stack.Count);
        Assert.NotEqual(thisId, schedulerId);
        Assert.True(stack.Reverse().SequenceEqual(Enumerable.Range(1, 4).Select(i => i + ": " + schedulerId)));
    }
}