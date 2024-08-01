using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    static class NumericFormatExtensions {
        public const int RadixOffset = 0;
        public const int StyleOffset = 4;
        public const int FlagOffset = 12;

        public static Radix GetRadix(this FormatFlags fmt) 
            => (Radix)((int)(fmt & FormatFlags.RadixMask) >> RadixOffset);
        
        public static int GetRadixBase(this FormatFlags fmt)
            => GetRadix(fmt).GetBaseNumber();
        
        public static FormatStyle GetStyle(this FormatFlags fmt)
            => (FormatStyle)(fmt & FormatFlags.StyleMask);
        
        public static FormatOptionFlags GetFlags(this FormatFlags fmt)
            => (FormatOptionFlags)(fmt & FormatFlags.FlagMask);

        public static bool HasFlag(this FormatFlags fmt, FormatOptionFlags flag)
            => GetFlags(fmt).HasFlag(flag);
    }

    enum FormatStyle {
        Default = 0,
        Character = 1 << NumericFormatExtensions.StyleOffset,
        SiPrefixed = 2 << NumericFormatExtensions.StyleOffset,
        KibiPrefixed = 3 << NumericFormatExtensions.StyleOffset,
        DayOfWeek = 4 << NumericFormatExtensions.StyleOffset,
        DateTime = 5 << NumericFormatExtensions.StyleOffset, // todo 廃止 DateTime
        TimeSpan = 6 << NumericFormatExtensions.StyleOffset, // todo 廃止? TimeSpan
        WebColor = 7 << NumericFormatExtensions.StyleOffset,
    }

    [Flags]
    enum FormatOptionFlags {
        None = 0,
        Integer = (1 << 0) << NumericFormatExtensions.FlagOffset,
    }

    // todo FormatFlags: Val に紐付くフラグだと分かるようにしたい
    [Flags]
    enum FormatFlags {
        Default = 0,

        RadixMask = 0xF << NumericFormatExtensions.RadixOffset,
        StyleMask = 0xFF << NumericFormatExtensions.StyleOffset,
        FlagMask = 0xFF << NumericFormatExtensions.FlagOffset,

        Decimal = Radix.Decimal << NumericFormatExtensions.RadixOffset,
        Hexadecimal = Radix.Hexadecimal << NumericFormatExtensions.RadixOffset,
        Binary = Radix.Binary << NumericFormatExtensions.RadixOffset,
        Octal = Radix.Octal << NumericFormatExtensions.RadixOffset,

        Character = Hexadecimal | FormatStyle.Character | FormatOptionFlags.Integer,
        SiPrefixed = Decimal | FormatStyle.SiPrefixed,
        BinaryPrefixed = Decimal | FormatStyle.KibiPrefixed,
        DayOfWeek = Decimal | FormatStyle.DayOfWeek | FormatOptionFlags.Integer,
        DateTime = Decimal | FormatStyle.DateTime, // todo 廃止 DateTime
        TimeSpan = Decimal | FormatStyle.TimeSpan, // todo 廃止? TimeSpan
        WebColor = Hexadecimal | FormatStyle.WebColor,
    }

}
