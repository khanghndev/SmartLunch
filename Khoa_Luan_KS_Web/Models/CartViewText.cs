namespace Khoa_Luan_KS_Web.Models;

/// <summary>Chuỗi giao diện giỏ hàng (Unicode escapes).</summary>
public static class CartViewText
{
    public const string PageTitle = "Gi\u1ecf h\u00e0ng";
    public const string MinimumPortionsHint = "20 su\u1ea5t";
    public const string Lead =
        "M\u1ed7i th\u1ef1c \u0111\u01a1n tu\u1ea7n l\u00e0 m\u1ed9t g\u00f3i: t\u00edch ch\u1ecdn ng\u00e0y c\u1ea7n d\u00f9ng su\u1ea5t (theo l\u1ecbch menu), nh\u1eadp s\u1ed1 b\u1ed9 \u2014 gi\u00e1 c\u1eadp nh\u1eadt t\u1ef1 \u0111\u1ed9ng. T\u1ed5ng su\u1ea5t t\u1ed1i thi\u1ec3u " + MinimumPortionsHint + " m\u1edbi \u0111\u01b0\u1ee3c \u0111\u1eb7t h\u00e0ng.";

    public const string ErrorMinimumPortions =
        "T\u1ed5ng su\u1ea5t trong gi\u1ecf ph\u1ea3i \u00edt nh\u1ea5t " + MinimumPortionsHint + " m\u1edbi ti\u1ebfp t\u1ee5c \u0111\u1eb7t h\u00e0ng.";

    public const string EmptyTitle = "Gi\u1ecf h\u00e0ng \u0111ang tr\u1ed1ng";
    public const string EmptyCta = "Xem th\u1ef1c \u0111\u01a1n";

    public const string BundleKicker = "Menu tu\u1ea7n";
    public const string BundleQtyLabel = "S\u1ed1 b\u1ed9 menu";
    public const string BundleDayHint = "Ch\u1ecdn ng\u00e0y d\u00f9ng su\u1ea5t (b\u1ecf ch\u1ecdn \u0111\u1ec3 lo\u1ea1i c\u1ea3 ng\u00e0y \u0111\u00f3)";
    public const string BundleDaySubHint = "M\u00f3n trong ng\u00e0y \u0111\u01b0\u1ee3c \u0111\u00f3ng g\u00f3i theo th\u1ef1c \u0111\u01a1n \u2014 kh\u00f4ng t\u00e1ch t\u1eebng m\u00f3n.";
    public const string BtnUpdateBundle = "C\u1eadp nh\u1eadt";
    public const string BtnRemoveBundle = "X\u00f3a g\u00f3i n\u00e0y";
    public const string LooseKicker = "M\u00f3n \u0111\u00e3 ch\u1ecdn";
    public const string BtnOrgMealOrder = "Thi\u1ebft l\u1eadp \u0111\u1eb7t su\u1ea5t";
    public const string BtnOrgMealOrderHint =
        "Chuy\u1ec3n sang trang \u0111\u1eb7t su\u1ea5t doanh nghi\u1ec7p \u2014 ch\u1ecdn m\u00f3n t\u1eeb gi\u1ecf h\u00e0ng khi l\u1eadp th\u1ef1c \u0111\u01a1n.";
    public const string BtnPersonalCheckout = "Thanh to\u00e1n c\u00e1 nh\u00e2n";
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
