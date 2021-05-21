//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

namespace System.Device.Pwm
{
    /// <summary>
    /// Describes which polarity the PWM signal should start in.
    /// </summary>
    public enum PwmPulsePolarity
    {
        /// <summary>
        /// Configures the PWM signal to start in the active high state.
        /// </summary>
        ActiveHigh,
        /// <summary>
        /// Configures the PWM signal to start in the active low state.
        /// </summary>
        ActiveLow
    }
}
