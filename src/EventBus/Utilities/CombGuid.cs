using System;

namespace EventBus.Utilities
{
    public static class CombGuid
    {
        /// <summary>
        /// Generate a new <see cref="Guid" /> using the comb algorithm.
        /// </summary>
        public static Guid Generate()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var now = DateTime.UtcNow;

            // Get the days and milliseconds which will be used to build the byte string
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var timeOfDay = now.TimeOfDay;

            // Convert to a byte array
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
            var daysArray = BitConverter.GetBytes(days.Days);
            var millisecondArray = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering
            Array.Reverse(daysArray);
            Array.Reverse(millisecondArray);

            // Copy the bytes into the guid
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(millisecondArray, millisecondArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;
    }
}
