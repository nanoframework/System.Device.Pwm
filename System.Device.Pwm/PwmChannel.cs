using System;
using System.Runtime.CompilerServices;

namespace System.Device.Pwm
{
    /// <summary>
    /// Represents a single PWM channel.
    /// </summary>
    public sealed class PwmChannel : IDisposable
    {
        // constant factor to convert property to native field
        private const double _dutyCycleFactor = 10000.0d;

#pragma warning disable IDE0044 // these fields are updated/read in native code
#pragma warning disable CS0649 // these fields are used in native code
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _frequency;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private uint _dutyCycle;
#pragma warning restore CS0649 // these fields are used in native code
#pragma warning restore IDE0044 // Add readonly modifier

#pragma warning disable 0414
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private PwmPulsePolarity _polarity;
#pragma warning restore 0414

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        // this field is to be used in native code
        private int _pinNumber;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _channelNumber;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private int _pwmTimer;

        /// <summary>
        /// The frequency in hertz.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Value must not be negative.
        /// </exception>
        public int Frequency
        {
            get => _frequency;
#pragma warning disable S4275 // Intended use: we want to call the native code to handle the setter
            set
#pragma warning restore S4275 // Getters and setters should access the expected fields
            {
                // when native method returns, the new frequency value has been stored in _frequency field
                NativeSetDesiredFrequency(value);
            }
        }

        /// <summary>
        /// The duty cycle represented as a value between 0.0 and 1.0.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Value must be between 0.0 and 1.0.
        /// </exception>
        public double DutyCycle
        {
            get => _dutyCycle / _dutyCycleFactor;
#pragma warning disable S4275 // Intended use: we want to call the native code to handle the setter
            set
#pragma warning restore S4275 // Getters and setters should access the expected fields
            {
                // when native method returns, the new duty cycle has been stored in _dutyCycle field
                SetActiveDutyCyclePercentage(value);
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
        /// Creates a new instance of the <see cref="PwmChannel"/> running on the current platform.
        /// </summary>
        /// <param name="chip">The PWM chip number, formally known as TIM channel.</param>
        /// <param name="channel">The channel number.</param>
        /// <param name="frequency">The frequency in hertz.</param>
        /// <param name="dutyCyclePercentage">The duty cycle percentage represented as a value between 0.0 and 1.0.</param>
        /// <returns>A <see cref="PwmChannel"/>.</returns>
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
        /// Creates a PwmChannel from a pin number.
        /// </summary>
        /// <param name="pin">The pin number.</param>
        /// <param name="frequency">The frequency in hertz.</param>
        /// <param name="dutyCyclePercentage">The duty cycle percentage represented as a value between 0.0 and 1.0.</param>
        /// <returns>A PWM channel</returns>
        public static PwmChannel CreateFromPin(int pin, int frequency = 400, double dutyCyclePercentage = 0.5)
        {
            int channel = -1;
            int chip;

            // we should have a channel from 0 to 8
            for (chip = 0; chip < 8; chip++)
            {
                channel = GetChannel(pin, chip);

                if (channel != -1)
                {
                    break;
                }
            }

            if (channel == -1)
            {
                return null;
            }

            return new PwmChannel(chip, channel, pin, frequency, dutyCyclePercentage);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PwmChannel"/> running on the current platform.
        /// </summary>
        /// <param name="chip">The PWM chip number, formally known as TIM channel.</param>
        /// <param name="channel">The PWM channel number.</param>
        /// <param name="frequency">The frequency in hertz.</param>
        /// <param name="dutyCyclePercentage">The duty cycle percentage represented as a value between 0.0 and 1.0.</param>
        /// <returns>A PWM channel</returns>
        /// <remarks>
        /// To create a <see cref="PwmChannel"/> from a GPIO pin number, please use <see cref="CreateFromPin"/>.
        /// </remarks>
        public PwmChannel(
            int chip,
            int channel,
            int frequency = 400,
            double dutyCyclePercentage = 0.5)

        {
            // no information on pin number
            _pinNumber = -1;

            _pwmTimer = chip;
            _channelNumber = channel;
            _polarity = PwmPulsePolarity.ActiveHigh;
            Frequency = frequency;
            NativeInit();
            DutyCycle = dutyCyclePercentage;
        }

        private PwmChannel(
            int chip,
            int channel,
            int pin,
            int frequency,
            double dutyCyclePercentage)
        {
            _pwmTimer = chip;
            _channelNumber = channel;
            _pinNumber = pin;
            _polarity = PwmPulsePolarity.ActiveHigh;
            Frequency = frequency;
            NativeInit();
            DutyCycle = dutyCyclePercentage;
        }

        private void SetActiveDutyCyclePercentage(double dutyCyclePercentage)
        {
            NativeSetActiveDutyCyclePercentage(dutyCyclePercentage);
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
        private extern void NativeSetDesiredFrequency(int desiredFrequency);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetActiveDutyCyclePercentage(double dutyCyclePercentage);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStart();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStop();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void DisposeNative();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int GetChannel(int pin, int timerId);

        #endregion
    }
}
