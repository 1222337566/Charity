using System.Text;

namespace InfrastructureManagmentServices.Reporting
{
    public class CharityWordExportService : ICharityWordExportService
    {
        public byte[] BuildWordDocument(string title, string htmlBody)
        {
            var html = $@"<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8' />
<title>{title}</title>
<style>
body {{ font-family: Tahoma, Arial; direction: rtl; }}
table {{ border-collapse: collapse; width: 100%; }}
th, td {{ border: 1px solid #999; padding: 6px; text-align: right; }}
h1 {{ margin-bottom: 16px; }}
</style>
</head>
<body>
<h1>{title}</h1>
{htmlBody}
</body>
</html>";

            return Encoding.UTF8.GetBytes(html);
        }
    }
}
