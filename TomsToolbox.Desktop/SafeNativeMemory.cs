namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Represents a wrapper class for a buffer allocated with <see cref="Marshal.AllocHGlobal(int)"/>
    /// </summary>
    public class SafeNativeMemory: SafeHandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeNativeMemory"/> class with no buffer allocated.
        /// </summary>
        public SafeNativeMemory()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeNativeMemory"/> class.
        /// </summary>
        /// <param name="size">The size of the buffer to allocate.</param>
        public SafeNativeMemory(int size)
            : base(IntPtr.Zero, true)
        {
            Allocate(size);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeNativeMemory" /> class.
        /// </summary>
        /// <param name="handle">The handle to the buffer for which this instance will take ownership.</param>
        /// <param name="size">The allocated size of the  allocated block.</param>
        public SafeNativeMemory(IntPtr handle, int size)
            : base(IntPtr.Zero, true)
        {
            Size = size;
            SetHandle(handle);
        }

        /// <summary>
        /// Allocates a buffer with the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public void Allocate(int size)
        {
            Size = size;
            SetHandle(Marshal.AllocHGlobal(size));
        }

        /// <summary>
        /// When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }

        /// <summary>
        /// Reads a processor native sized integer from unmanaged memory.
        /// </summary>
        /// <returns>
        /// The IntPtr read.
        /// </returns>
        public IntPtr ReadIntPtr()
        {
            return Marshal.ReadIntPtr(handle);
        }

        /// <summary>
        /// Reads a 32-bit signed integer from unmanaged memory.
        /// </summary>
        /// <returns>
        /// The 32-bit signed integer read.
        /// </returns>
        public int ReadInt32()
        {
            return Marshal.ReadInt32(handle);
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the handle value is invalid.
        /// </summary>
        /// <returns>
        /// true if the handle value is invalid; otherwise, false.
        /// </returns>
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <summary>
        /// Gets the size of the allocated buffer.
        /// </summary>
        public int Size
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Represents a wrapper class for a buffer allocated with <see cref="Marshal.AllocHGlobal(int)"/> and the size of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeNativeMemory<T> : SafeNativeMemory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeNativeMemory{T}"/> class.
        /// </summary>
        public SafeNativeMemory()
            : base(Marshal.SizeOf(typeof(T)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeNativeMemory{T}"/> class.
        /// </summary>
        /// <param name="size">The size of the buffer to allocate.</param>
        public SafeNativeMemory(int size)
            : base(size)
        {
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SafeNativeMemory"/> to <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="wrapper">The memory.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator T([NotNull] SafeNativeMemory<T> wrapper)
        {
            Contract.Requires(wrapper != null);

            return wrapper.ToStructure();
        }

        /// <summary>
        /// Marshal the memory from native to .NET.
        /// </summary>
        /// <returns>The .NET structure.</returns>
        [CanBeNull]
        public T ToStructure()
        {
            return Marshal.PtrToStructure(handle, typeof(T)).SafeCast<T>();
        }
    }
}