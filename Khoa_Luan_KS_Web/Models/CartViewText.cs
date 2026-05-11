namespace Khoa_Luan_KS_Web.Models;

/// <summary>Chuỗi giao diện giỏ hàng (Unicode escapes).</summary>
public static class CartViewText
{
    public const string PageTitle = "Gi\u1ecf h\u00e0ng";
    public const string Lead =
        "M\u1ed7i th\u1ef1c \u0111\u01a1n tu\u1ea7n l\u00e0 m\u1ed9t g\u00f3i: ch\u1ecdn ng\u00e0y d\u00f9ng su\u1ea5t, nh\u1eadp s\u1ed1 b\u1ed9 menu \u0111\u1ec3 nh\u00e2n \u0111\u1ed3ng lo\u1ea1t t\u1ea5t c\u1ea3 su\u1ea5t \u0111\u00e3 ch\u1ecdn.";

    public const string EmptyTitle = "Gi\u1ecf h\u00e0ng \u0111ang tr\u1ed1ng";
    public const string EmptyCta = "Xem th\u1ef1c \u0111\u01a1n";

    public const string BundleKicker = "Menu tu\u1ea7n";
    public const string BundleQtyLabel = "S\u1ed1 b\u1ed9 menu";
    public const string BundleDayHint = "Ch\u1ecdn su\u1ea5t theo ng\u00e0y";
    public const string BtnUpdateBundle = "C\u1eadp nh\u1eadt";
    public const string BtnRemoveBundle = "X\u00f3a g\u00f3i n\u00e0y";
    public const string BtnWorkdays = "Ch\u1ec9 T2\u2013T6";
    public const string BtnAllDays = "C\u1ea3 tu\u1ea7n";
    public const string LooseKicker = "M\u00f3n th\u00eam l\u1ebb";
    public const string SubtotalLabel = "T\u1ea1m t\u00ednh";
    public const string BtnClearCart = "X\u00f3a gi\u1ecf";
    public const string BtnCheckout = "\u0110\u1eb7t h\u00e0ng";
    public const string QtyShort = "SL";

    public const string ErrorNoSlotSelected =
        "Vui l\u00f2ng ch\u1ecdn \u00edt nh\u1ea5t m\u1ed9t su\u1ea5t trong menu (b\u1ecf tick c\u00e1c ng\u00e0y kh\u00f4ng d\u00f9ng).";

    public static string DayShortVi(DateTime d) => d.DayOfWeek switch
    {
        DayOfWeek.Monday => "T2",
        DayOfWeek.Tuesday => "T3",
        DayOfWeek.Wednesday => "T4",
        DayOfWeek.Thursday => "T5",
        DayOfWeek.Friday => "T6",
        DayOfWeek.Saturday => "T7",
        DayOfWeek.Sunday => "CN",
        _ => d.DayOfWeek.ToString()
    };
}
