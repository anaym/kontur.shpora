using System;
using System.Globalization;

namespace DLibrary.Graph
{
    public struct UDouble : IComparable<UDouble>, IEquatable<UDouble>
    {
        private readonly double value;

        public UDouble(UDouble d) : this((double)d)
        { }

        public UDouble(double d = 0)
        {
            value = d;
            if (d < 0)
                throw new ArgumentOutOfRangeException($"Value must be positive, but we recived {d}");
        }

        public static implicit operator double(UDouble d) => d.value;

        public static explicit operator UDouble(double d) => new UDouble(d);

        public static UDouble operator + (UDouble a, UDouble b) => new UDouble(a.value + b.value);
        public static UDouble operator * (UDouble a, UDouble b) => new UDouble(a.value*b.value);
        public static UDouble operator / (UDouble a, UDouble b) => new UDouble(a.value / b.value);

        public static double operator - (UDouble a) => -a.value;
        public static double operator - (UDouble a, UDouble b) => a.value - b;


        public static readonly UDouble Zero = new UDouble(0);
        public static readonly UDouble One = new UDouble(1);
        public int CompareTo(UDouble other) => value.CompareTo(other.value);

        public bool Equals(UDouble other) => value.Equals(other);

        public override bool Equals(object obj) => (obj as UDouble?)?.Equals(this) ?? false;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();
        public string ToString(CultureInfo culture) => value.ToString(culture);
    }
}