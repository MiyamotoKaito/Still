using System;

namespace Still.Escape
{
    ///<summary>
    ///鍵を出すために調べる必要があるもの
    ///</summary>
    [Flags]
    public enum EscapeItemType
    {
        None = 0,
        Tv = 1 << 0,
        Music = 1 << 1,
        Book = 1 << 2,
    }
}
