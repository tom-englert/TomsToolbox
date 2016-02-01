namespace TomsToolbox.Desktop
{
    using System;
    using System.Runtime.InteropServices;

    using TomsToolbox.Core;

    /// <summary>
    /// Wrapper for a global memory handle. 
    /// The memory will be allocated using <see cref="Marshal.AllocHGlobal(int)"/>; the memory will be freed when the object is disposed.
    /// </summary>
    public sealed class HGlobal : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HGlobal"/> class.
        /// </summary>
        /// <param name="size">The size of the memory block to allocate.</param>
        public HGlobal(int size)
        {
            Size = size;
            Ptr = Marshal.AllocHGlobal(size);
        }

        /// <summary>
        /// Gets the size of the allocated memory block.
        /// </summary>
        public int Size
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the pointer to the allocated memory block.
        /// </summary>
        public IntPtr Ptr
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HGlobal"/> to <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="item">The item to convert.</param>
        /// <returns>
        /// The pointer to the allocated memory block.
        /// </returns>
        public static implicit operator IntPtr(HGlobal item)
        {
            return ToIntPtr(item);
        }

        /// <summary>
        /// Performs a conversion from <see cref="HGlobal"/> to <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="item">The item to convert.</param>
        /// <returns>
        /// The pointer to the allocated memory block.
        /// </returns>
        public static IntPtr ToIntPtr(HGlobal item)
        {
            return item == null ? IntPtr.Zero : item.Ptr;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (Ptr == IntPtr.Zero)
                return;

            Marshal.FreeHGlobal(Ptr);
            Ptr = IntPtr.Zero;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HGlobal"/> class.
        /// </summary>
        ~HGlobal()
        {
            this.ReportNotDisposedObject();
        }
    }
}
