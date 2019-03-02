using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;

#if NET40

namespace System.Threading
{
    /// <summary>
    /// <para>
    /// Provides methods for performing volatile memory operations.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public static class Volatile
    {
        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool Read(ref bool location)
        {
            bool flag = location;
            Thread.MemoryBarrier();
            return flag;
        }

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref bool location, bool value)
        {
            Thread.MemoryBarrier();
            location = value;
        }

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static double Read(ref double location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref double location, double value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static float Read(ref float location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref float location, float value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static UIntPtr Read(ref UIntPtr location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref UIntPtr location, UIntPtr value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static IntPtr Read(ref IntPtr location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref IntPtr location, IntPtr value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static ulong Read(ref ulong location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref ulong location, ulong value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static long Read(ref long location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref long location, long value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static int Read(ref int location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref int location, int value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static ushort Read(ref ushort location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref ushort location, ushort value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static short Read(ref short location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref short location, short value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static byte Read(ref byte location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref byte location, byte value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static sbyte Read(ref sbyte location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref sbyte location, sbyte value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the value of the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The value that was read.
        /// This value is the latest written by any processor in the computer, regardless of the number of processors or the state of processor cache.
        /// </returns>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static uint Read(ref uint location) => Thread.VolatileRead(ref location);

        /// <summary>
        /// Writes the specified value to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <param name="location">The field where the value is written.</param>
        /// <param name="value">
        /// The value to write.
        /// The value is written immediately so that it is visible to all processors in the computer.
        /// </param>
        [CLSCompliant(false)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void Write(ref uint location, uint value) => Thread.VolatileWrite(ref location, value);

        /// <summary>
        /// Reads the object reference from the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears after this method in the code, the processor cannot move it before this method.
        /// </summary>
        /// <typeparam name="T">The type of field to read. This must be a reference type, not a value type.</typeparam>
        /// <param name="location">The field to read.</param>
        /// <returns>
        /// The reference to <typeparamref name="T"/> that was read.
        /// This reference is the latest written by any processor in the computer,
        /// regardless of the number of processors or the state of processor cache.        
        /// </returns>
        public static T Read<T>(ref T location) where T : class
        {
            T value = location;
            Thread.MemoryBarrier();
            return value;
        }

        /// <summary>
        /// Writes the specified object reference to the specified field.
        /// On systems that require it,
        /// inserts a memory barrier that prevents the processor from reordering memory operations as follows:
        /// If a read or write appears before this method in the code, the processor cannot move it after this method.
        /// </summary>
        /// <typeparam name="T">The type of field to write. This must be a reference type, not a value type.</typeparam>
        /// <param name="location">The field where the object reference is written.</param>
        /// <param name="value">
        /// The object reference to write.
        /// The reference is written immediately so that it is visible to all processors in the computer.
        /// </param>
        public static void Write<T>(ref T location, T value) where T : class
        {
            Thread.MemoryBarrier();
            location = value;
        }
    }
}

#else

[assembly: TypeForwardedTo(typeof(Volatile))]

#endif
