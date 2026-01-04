using ContractManager.Contract;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager
{
    internal static class Utils
    {
        internal static string FormatDistance(double distanceInMeters, string format = "{0:N1}{1}")
        {
            string unit = "m";
            double distance = distanceInMeters;
            if (distanceInMeters >= 1e7d)
            {
                distance /= 1e6d;
                unit = "Gm";
            }
            else
            if (distanceInMeters >= 1e4d)
            {
                distance /= 1e3d;
                unit = "km";
            }
            if (Double.IsNaN(distance))
            {
                unit = "";
            }
            return String.Format(format, distance, unit);
        }

        public static string FormatSimTimeAsYearDayTime(KSA.SimTime simTime)
        {
            if (simTime.IsNaN())
            {
                return "NaN";
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0:0000} ", Math.Floor(simTime.Years));
            stringBuilder.AppendFormat("{0:000} ", Math.Floor(simTime.Days) % TimeConstants.daysInMonth);

            stringBuilder.AppendFormat("{0:00}:", Math.Floor(simTime.Hours) % TimeConstants.hoursInDay);
            stringBuilder.AppendFormat("{0:00}:", Math.Floor(simTime.Minutes) % TimeConstants.minutesInHour);
            stringBuilder.AppendFormat("{0:00}", Math.Floor(simTime.Seconds()) % TimeConstants.secondsInMinute);

            return stringBuilder.ToString();
        }

        public static string FormatSimTimeAsDateTime(KSA.SimTime simTime)
        {
            if (simTime.IsNaN())
            {
                return "NaN";
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0:0000}-", Math.Floor(simTime.Years));
            stringBuilder.AppendFormat("{0:00}-", Math.Floor(simTime.Months) % TimeConstants.monthsInYear);
            stringBuilder.AppendFormat("{0:00} ", Math.Floor(simTime.Days) % TimeConstants.daysInMonth);

            stringBuilder.AppendFormat("{0:00}:", Math.Floor(simTime.Hours) % TimeConstants.hoursInDay);
            stringBuilder.AppendFormat("{0:00}:", Math.Floor(simTime.Minutes) % TimeConstants.minutesInHour);
            stringBuilder.AppendFormat("{0:00}", Math.Floor(simTime.Seconds()) % TimeConstants.secondsInMinute);

            return stringBuilder.ToString();
        }

        public static string FormatSimTimeAsRelative(KSA.SimTime simTime, bool showOnlyNonZero = false)
        {
            if (simTime.IsNaN())
            {
                return "NaN";
            }

            StringBuilder stringBuilder = new StringBuilder();
            if (!showOnlyNonZero) {
                stringBuilder.AppendFormat("{0:0000} ", Math.Floor(simTime.Years));
                stringBuilder.AppendFormat("{0:000} ", Math.Floor(simTime.Days) % TimeConstants.daysInMonth);
            }
            else
            {
                if (simTime.Years >= 1)
                {
                    stringBuilder.AppendFormat("{0:0} ", Math.Floor(simTime.Years));
                    stringBuilder.AppendFormat("{0:000} ", Math.Floor(simTime.Days));
                }
                else
                if (simTime.Days >= 1)
                {
                    stringBuilder.AppendFormat("{0:0} ", Math.Floor(simTime.Days));
                }
            }

            stringBuilder.AppendFormat("{0:00}:", Math.Floor(simTime.Hours) % TimeConstants.hoursInDay);
            stringBuilder.AppendFormat("{0:00}:", Math.Floor(simTime.Minutes) % TimeConstants.minutesInHour);
            stringBuilder.AppendFormat("{0:00}", Math.Floor(simTime.Seconds()) % TimeConstants.secondsInMinute);

            return stringBuilder.ToString();
        }
    }
    internal class Colors
    {
        // Red, bit darker, used for normal button.
        public static Brutal.Numerics.float4 redDark = new Brutal.Numerics.float4 { X = 0.5f, Y = 0.1f, Z = 0.1f, W = 1.0f };
        // Red, bit whiter, used for hovering color.
        public static Brutal.Numerics.float4 redLight = new Brutal.Numerics.float4 { X = 0.75f, Y = 0.25f, Z = 0.25f, W = 1.0f };
        // Red, bright, used for pushed or normal text.
        public static Brutal.Numerics.float4 red = new Brutal.Numerics.float4 { X = 0.75f, Y = 0.1f, Z = 0.1f, W = 1.0f };
        
        // Green, bit darker, used for normal button.
        public static Brutal.Numerics.float4 greenDark = new Brutal.Numerics.float4 { X = 0.2f, Y = 0.5f, Z = 0.2f, W = 1.0f };
        // Green, bit whiter, used for hovering color.
        public static Brutal.Numerics.float4 greenLight = new Brutal.Numerics.float4 { X = 0.35f, Y = 0.75f, Z = 0.35f, W = 1.0f };
        // Green, bright, used for pushed or normal text.
        public static Brutal.Numerics.float4 green = new Brutal.Numerics.float4 { X = 0.2f, Y = 0.75f, Z = 0.2f, W = 1.0f };
        
        // Blue, bit darker, used for normal button.
        public static Brutal.Numerics.float4 blueDark = new Brutal.Numerics.float4 { X = 0.25f, Y = 0.35f, Z = 0.75f, W = 1.0f };
        // Blue, bit whiter, used for hovering color.
        public static Brutal.Numerics.float4 blueLight = new Brutal.Numerics.float4 { X = 0.25f, Y = 0.35f, Z = 1.0f, W = 1.0f };
        // Blue, bright, used for pushed or normal text.
        public static Brutal.Numerics.float4 blue = new Brutal.Numerics.float4 { X = 0.0f, Y = 0.0f, Z = 1.0f, W = 1.0f };
        
        // Yellow, bit darker, used for normal button.
        public static Brutal.Numerics.float4 yellowDark = new Brutal.Numerics.float4 { X = 0.75f, Y = 0.75f, Z = 0.0f, W = 1.0f };
        // Yellow, bit whiter, used for hovering color.
        public static Brutal.Numerics.float4 yellowLight = new Brutal.Numerics.float4 { X = 0.8f, Y = 0.8f, Z = 0.35f, W = 1.0f };
        // Yellow, bright, used for pushed or normal text.
        public static Brutal.Numerics.float4 yellow = new Brutal.Numerics.float4 { X = 0.9f, Y = 0.9f, Z = 0.0f, W = 1.0f };
        
        // Orange, bit darker, used for normal button.
        public static Brutal.Numerics.float4 orangeDark = new Brutal.Numerics.float4 { X = 0.8f, Y = 0.5f, Z = 0.1f, W = 1.0f };
        // Orange, bit whiter, used for hovering color.
        public static Brutal.Numerics.float4 orangeLight = new Brutal.Numerics.float4 { X = 1.0f, Y = 0.6f, Z = 0.2f, W = 1.0f };
        // Orange, bright, used for pushed or normal text.
        public static Brutal.Numerics.float4 orange = new Brutal.Numerics.float4 { X = 1.0f, Y = 0.5f, Z = 0.0f, W = 1.0f };

        // Gray, bit darker, used for normal button.
        public static Brutal.Numerics.float4 grayDark = new Brutal.Numerics.float4 { X = 0.25f, Y = 0.25f, Z = 0.25f, W = 1.0f };
        // Gray, bit whiter, used for hovering color.
        public static Brutal.Numerics.float4 grayLight = new Brutal.Numerics.float4 { X = 0.75f, Y = 0.75f, Z = 0.75f, W = 1.0f };
        // Gray, bright, used for pushed or normal text.
        public static Brutal.Numerics.float4 gray = new Brutal.Numerics.float4 { X = 0.5f, Y = 0.5f, Z = 0.5f, W = 1.0f };

        public static Brutal.Numerics.float4 GetTrackedRequirementStatusColor(Contract.TrackedRequirementStatus status)
        {
            if (status == TrackedRequirementStatus.NOT_STARTED)
            {
                return Colors.gray;
            }
            else
            if (status == TrackedRequirementStatus.TRACKED)
            {
                return Colors.orange;
            }
            else
            if (status == TrackedRequirementStatus.MAINTAINED)
            {
                return Colors.yellow;
            }
            else
            if (status == TrackedRequirementStatus.ACHIEVED)
            {
                return Colors.green;
            }
            else
            if (status == TrackedRequirementStatus.FAILED)
            {
                return Colors.red;
            }
            return Colors.gray;
        }
    }

    public class TimeConstants
    {
        public static double secondsInMinute = 60;
        public static double minutesInHour = 60;
        public static int hoursInDay = 24;
        public static int daysInMonth = 30;
        public static int monthsInYear = 12;
        public static int daysInYear = 360;
    }

}
