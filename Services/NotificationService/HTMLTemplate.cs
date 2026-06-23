using System.Net;

namespace NotificationService
{
    public class HTMLTemplate
    {
        public static string getHTMLPage(string code, string subject)
        {
            var safeCode = WebUtility.HtmlEncode(code);
            var safeSubject = WebUtility.HtmlEncode(subject);
            var year = DateTime.Now.Year;
            var logoUrl = "https://github.com/VovaRydush/photosporthive/blob/main/image.png?raw=true";

            return $@"
<!doctype html>
<html lang=""uk"">
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
</head>
<body style=""margin:0;padding:0;background:#f4f4f4;font-family:Arial,sans-serif;color:#111;"">
    <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background:#f4f4f4;padding:28px 12px;"">
        <tr>
            <td align=""center"">
                <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""max-width:620px;background:#ffffff;border:2px solid #111111;box-shadow:6px 6px 0 rgba(0,0,0,.18);"">
                    <tr>
                        <td style=""padding:26px 28px 12px;text-align:center;border-bottom:2px solid #111111;"">
                            <img src=""{logoUrl}"" alt=""SportHive"" style=""max-width:170px;height:auto;margin-bottom:12px;"">
                            <h1 style=""margin:0;font-size:26px;line-height:1.2;text-transform:uppercase;"">{safeSubject}</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding:28px;"">
                            <p style=""font-size:16px;line-height:1.5;margin:0 0 18px;"">
                                Для завершення дії у SportHive використайте код підтвердження:
                            </p>
                            <div style=""font-size:34px;font-weight:800;letter-spacing:8px;text-align:center;border:2px solid #111111;background:#f7f7f7;padding:18px 12px;margin:18px 0;"">
                                {safeCode}
                            </div>
                            <p style=""font-size:14px;line-height:1.5;color:#555;margin:18px 0 0;"">
                                Код дійсний протягом 10 хвилин. Якщо ви не виконували цю дію, просто проігноруйте лист.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding:16px 28px;border-top:1px solid #dddddd;color:#666;font-size:12px;text-align:center;"">
                            © {year} SportHive. Усі права захищені.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
