using SmartLunch.Backend.Service.Application.DTOs.Response.Cart;

namespace SmartLunch.Backend.Service.Application.Cart;

public static class ShoppingCartMath
{
    public static void RecalculateTotals(ShoppingCartDto cart)
    {
        foreach (var line in cart.Items)
            line.LineTotal = decimal.Round(line.UnitPrice * line.Quantity, 2, MidpointRounding.AwayFromZero);

        cart.TotalAmount = decimal.Round(cart.Items.Sum(i => i.LineTotal), 2, MidpointRounding.AwayFromZero);
    }
}
