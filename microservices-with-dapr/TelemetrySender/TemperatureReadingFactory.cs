using System;

namespace TelemetrySender
{
    public class TemperatureReadingFactory
    {
        public static TemperatureReading CreateTemperatureReading()
        {
            var random = new Random((int)DateTime.Now.Ticks);
            return new TemperatureReading { Timestamp = DateTime.Now, Value = random.Next(0, 40) };
        }
    }
}
