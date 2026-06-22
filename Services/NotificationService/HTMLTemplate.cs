using System.Security.Cryptography.X509Certificates;

namespace NotificationService
{
    public class HTMLTemplate
    {
        public static string getHTMLPage(string code,string Subject)
{
    // Ваш base64-зображення
    string base64Image = "https://github.com/VovaRydush/photosporthive/blob/main/image.png?raw=true"; // вставте ваше Base64 тут

    return $@"
<!DOCTYPE html>
<html lang='uk'>
<head>
<meta charset='UTF-8' />
<meta name='viewport' content='width=device-width, initial-scale=1.0' />
<title>Підтвердження email</title>
<style>
body {{
  background-color: #fff;
  font-family: 'Georgia', serif;
  color: #111;
  margin: 0;
  padding: 0;
}}
/* Центрування обгортки */
.wrapper {{
  max-width: 640px;
  margin: 40px auto;
  padding: 50px 30px;
  background-color: #fff;
  border-radius: 12px;
  box-shadow: 0 8px 20px rgba(0,0,0,0.15);
  position: relative;
  text-align: center; /* Важливо для центрування всього всередині */
}}
/* Логотип або зображення */
.logo-img {{
  max-width: 200px;
  width: 100%;
  height: auto;
  margin-bottom: 30px;
}}
/* Заголовок */
h1 {{
  font-family: 'Times New Roman', serif;
  font-size: 32px;
  font-weight: bold;
  letter-spacing: 1px;
  margin-bottom: 30px;
  color: #222;
}}
p {{
  font-size: 16px;
  line-height: 1.6;
  color: #555;
  margin: 20px 0;
}}
.code {{
  display: inline-block;
  font-family: 'Courier New', monospace;
  font-size: 50px;
  font-weight: bold;
  letter-spacing: 8px;
  color: #222;
  background-color: #f7f7f7;
  padding: 20px 40px;
  border-radius: 8px;
  margin: 30px 0;
  box-shadow: inset 0 0 10px rgba(0,0,0,0.05);
}}
/* Підказка безпеки */
.security-tip {{
  background-color: #eef6f9;
  padding: 15px;
  border-radius: 8px;
  margin: 30px 0;
  font-size: 14px;
  color: #444;
  line-height: 1.4;
}}
/* Нижній колонтитул */
.footer {{
  font-size: 13px;
  color: #999;
  margin-top: 60px;
  text-align: center;
  border-top: 1px solid #ddd;
  padding-top: 20px;
}}
@media(max-width: 640px) {{
  .wrapper {{
    padding: 30px 20px;
  }}
  .code {{
    font-size: 40px;
    padding: 15px 20px;
  }}
}}
</style>
</head>
<body>
<div class='wrapper'>
  <!-- Тут ваше зображення зверху по центру -->
  <img src='{base64Image}' alt='Sport Hive Logo' class='logo-img' />
  <h1>{Subject}</h1>
  <div class='security-tip'>
    <strong>На замітку:</strong> Код дійсний тільки протягом 15 хвилин. Якщо ви не робили цю дію,проігноруйте дане повідомлення.
  </div>
  <div style='display:flex; flex-direction:column; align-items:center;'>
    <div class='code'>{code}</div>
  </div>
  <div class='footer'>
    © {DateTime.Now.Year} SportHive. Усі права захищені.<br>
    <a href='https://yourdomain.com/support' style='color:#999; text-decoration:none;'>Підтримка</a> | <a href='https://yourdomain.com/privacy' style='color:#999; text-decoration:none;'>Політика конфіденційності</a>
  </div>
</div>
</body>
</html>";
}
    }
}