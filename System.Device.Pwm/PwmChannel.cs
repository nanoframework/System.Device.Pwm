using System;
using System.Runtime.CompilerServices;

namespace System.Device.Pwm
{
    /// <summary>
    /// Represents a single PWM channel.
    /// </summary>
    public sealed class PwmChannel : IDisposable
    {
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _frequency;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private double _dutyCyclePercentage;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private uint _dutyCycle;

        #pragma warning disable 0414
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private PwmPulsePolarity _polarity;
        #pragma warning restore 0414

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _pinNumber;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _pwmTimer;

        /// <summary>
        /// The frequency in hertz.
        /// </summary>
        public int Frequency
        {
            get => _frequency;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must not be negative.");
                }

                var actualFrequencey = NativeSetDesiredFrequency((uint)value);
                _frequency = (int)actualFrequencey;
            }
        }

        /// <summary>
        /// The duty cycle represented as a value between 0.0 and 1.0.
        /// </summary>
        public double DutyCycle
        {
            get => _dutyCyclePercentage;
            set
            {
                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0.0 and 1.0.");
                }

                SetActiveDutyCyclePercentage(value);
                _dutyCyclePercentage = value;
            }
        }

        /// <summary>
        /// Starts the PWM channel.
        /// </summary>
        public void Start()
        {
            NativeStart();
        }

        /// <summary>
        /// Stops the PWM channel.
        /// </summary>
        public void Stop()
        {
            NativeStop();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PwmChannel"/> running on the current platform. (Windows 10 IoT or Unix/Raspbian)
        /// </summary>
        /// <param name="chip">The PWM chip number.</param>
        /// <param name="channel">The PWM channel number.</param>
        /// <param name="frequency">The frequency in hertz.</param>
        /// <param name="dutyCyclePercentage">The duty cycle percentage represented as a value between 0.0 and 1.0.</param>
        /// <returns>A PWM channel</returns>
        public static PwmChannel Create(
            int chip,
            int channel,
            int frequency = 400,
            double dutyCyclePercentage = 0.5)
        {
            var pwmChannel = new PwmChannel(chip, channel, frequency, dutyCyclePercentage);
            return pwmChannel;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PwmChannel"/> running on the current platform. (Windows 10 IoT or Unix/Raspbian)
        /// </summary>
        /// <param name="chip">The PWM chip number.</param>
        /// <param name="channel">The PWM channel number.</param>
        /// <param name="frequency">The frequency in hertz.</param>
        /// <param name="dutyCyclePercentage">The duty cycle percentage represented as a value between 0.0 and 1.0.</param>
        /// <returns>A PWM channel</returns>
        public PwmChannel(
        int chip,
            int channel,
            int frequency = 400,
            double dutyCyclePercentage = 0.5)

        {
            _pwmTimer = chip;
            _pinNumber = channel;
            _polarity = PwmPulsePolarity.ActiveHigh;
            Frequency = frequency;
            NativeInit();
            DutyCycle = dutyCyclePercentage;
        }


        private void SetActiveDutyCyclePercentage(double dutyCyclePercentage)
        {
            _dutyCycle = (uint)(dutyCyclePercentage * 10000);
            NativeSetActiveDutyCyclePercentage(_dutyCycle);
        }

        #region IDisposable Support

        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                DisposeNative();

                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PwmChannel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region external calls to native implementations

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeInit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern uint NativeSetDesiredFrequency(uint desiredFrequency);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetActiveDutyCyclePercentage(uint dutyCyclePercentage);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStart();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStop();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void DisposeNative();
        #endregion
    }
}
