using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FitTrack.Core.Dtos;
using FitTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FitTrack.Web.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private const int MatrixSize = 29;

    private readonly IQRCodeService _qrCodeService;

    public IndexModel(IQRCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    public QRCodeDto? QRCode { get; private set; }

    public string QRCodeSvg { get; private set; } = string.Empty;

    public bool NeedsMembership { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Challenge();
        }

        try
        {
            QRCode = await _qrCodeService.GetOrCreateForUserAsync(userId.Value);
            QRCodeSvg = BuildQRCodeSvg(QRCode.Code);
        }
        catch (InvalidOperationException)
        {
            NeedsMembership = true;
        }

        return Page();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }

    private static string BuildQRCodeSvg(string value)
    {
        var cells = BuildMatrix(value);
        var svg = new StringBuilder();
        var viewBoxSize = MatrixSize + 4;

        svg.Append($"""<svg class="qr-svg" viewBox="0 0 {viewBoxSize} {viewBoxSize}" role="img" aria-label="Member QR code" xmlns="http://www.w3.org/2000/svg">""");
        svg.Append($"""<rect width="{viewBoxSize}" height="{viewBoxSize}" rx="1.5" fill="#f7f8fa"/>""");

        for (var y = 0; y < MatrixSize; y++)
        {
            for (var x = 0; x < MatrixSize; x++)
            {
                if (cells[y, x])
                {
                    svg.Append($"""<rect x="{x + 2}" y="{y + 2}" width="1" height="1" fill="#060709"/>""");
                }
            }
        }

        svg.Append("</svg>");

        return svg.ToString();
    }

    private static bool[,] BuildMatrix(string value)
    {
        var cells = new bool[MatrixSize, MatrixSize];

        AddFinder(cells, 0, 0);
        AddFinder(cells, MatrixSize - 7, 0);
        AddFinder(cells, 0, MatrixSize - 7);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var bitIndex = 0;

        for (var y = 0; y < MatrixSize; y++)
        {
            for (var x = 0; x < MatrixSize; x++)
            {
                if (IsFinderArea(x, y))
                {
                    continue;
                }

                var hashByte = hash[(bitIndex / 8) % hash.Length];
                var bit = (hashByte >> (bitIndex % 8)) & 1;
                cells[y, x] = bit == 1;
                bitIndex++;
            }
        }

        return cells;
    }

    private static void AddFinder(bool[,] cells, int startX, int startY)
    {
        for (var y = 0; y < 7; y++)
        {
            for (var x = 0; x < 7; x++)
            {
                var isOuter = x == 0 || x == 6 || y == 0 || y == 6;
                var isCenter = x >= 2 && x <= 4 && y >= 2 && y <= 4;
                cells[startY + y, startX + x] = isOuter || isCenter;
            }
        }
    }

    private static bool IsFinderArea(int x, int y)
    {
        return x < 8 && y < 8
            || x >= MatrixSize - 8 && y < 8
            || x < 8 && y >= MatrixSize - 8;
    }
}
