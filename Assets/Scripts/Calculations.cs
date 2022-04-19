using System;

public static class Calculations {
    // Clamp the value of a double
    public static double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    public static class Distance
    {
        public static double ToAUFromSolarRadii(double solarRadii)
        {
            return solarRadii * Constants.solarRadiiToAU;
        }

        public static double ToAUFromUnity(double unity)
        {
            return ToAUFromSolarRadii(ToSolarRadiiFromUnity(unity));
        }

        public static double ToUnityFromSolarRadii(double solarRadii)
        {
            return solarRadii * Constants.solarScale;
        }

        public static double ToUnityFromAU(double au)
        {
            return ToUnityFromSolarRadii(au / Constants.solarRadiiToAU);
        }

        public static double ToSolarRadiiFromUnity(double unity)
        {
            return unity / Constants.solarScale;
        }

        public static double ToMFromSolarRadii(double solarRadii)
        {
            return solarRadii * Constants.solarRadiiToM;
        }

        public static double ToMFromUnity(double unity)
        {
            return ToMFromSolarRadii(ToSolarRadiiFromUnity(unity));
        }
    }

    public static class Mass
    {
        public static double ToKgFromSolarMass(double solarMass)
        {
            return solarMass * Constants.solarMassToKg;
        }
    }

    public static double GetGravityPull(double solarMass, double unity)
    {
        double m = Mass.ToKgFromSolarMass(solarMass);
        double distance = Distance.ToMFromUnity(unity);

        double acceleration = (Constants.gravitationalConstant * m) / (distance * distance);
        return acceleration * Math.Sqrt(Constants.solarScale / Constants.solarRadiiToM);
    }

    public static class Orbits {
        // Gets the orbital velocity of an object give 2 masses and the distance between them
        public static double GetVelocity(double solarMass, double solarmass, double solarRadii) {
            // Convert the parameters to their SI units
            double M = Mass.ToKgFromSolarMass(solarMass);
            double m = Mass.ToKgFromSolarMass(solarmass);
            double radius = Distance.ToMFromSolarRadii(solarRadii);

            // Calculate velocity of object
            double sqrVelocity = Constants.gravitationalConstant * (M + m) / (2 * radius);
            double velocity = Math.Sqrt(sqrVelocity);

            // Convert the distance aspect of the velocity back to the original unit
            return velocity * (Constants.solarScale / Constants.solarRadiiToM);
        } 

        // Gets the orbital velocity of an object give its orbital period and orbit radius
        public static double GetVelocity(double period, double solarRadii) {
            // Convert to SI units
            double radius = Distance.ToMFromSolarRadii(solarRadii);

            // Calculate velocity from period
            double velocity = (2 * Math.PI * radius) / period;

            // Convert the distance aspect of the velocity back to the original unit
            return velocity * (Constants.solarScale / Constants.solarRadiiToM);
        }

        // Gets the orbital period between 2 masses
        public static double GetPeriod(double solarMass, double solarmass, double solarRadii) {
            // Convert to SI units
            double M = Mass.ToKgFromSolarMass(solarMass);
            double m = Mass.ToKgFromSolarMass(solarmass);
            double radius = Distance.ToMFromSolarRadii(solarRadii);

            // Calculate the orbital period between the 2 masses in seconds
            return 2 * Math.PI * Math.Sqrt((radius * radius * radius) / (Constants.gravitationalConstant * (M + m)));
        }
    }

    public static class Time {
        public static double ToYearsFromSeconds(double seconds) {
            return seconds / (365.25 * 24 * 60 * 60);
        }

        public static (double, string) SecondsToSuitableTime(double seconds) {
            double hours = seconds / (60 * 60);
            double days = seconds / (24 * 60 * 60);
            double weeks = seconds / (7 * 24 * 60 * 60);
            
            if (days < 1) {
                return (hours, "hours");
            } else if (weeks < 1) {
                return (days, "days");
            } else if (weeks < 52) {
                return (weeks, "weeks");
            }
            return (ToYearsFromSeconds(seconds), "years");
        }
    }
}