using System;
using System.Collections.Generic;
using System.Linq;

namespace InfrastructureManagmentWebFramework.Helpers
{
    public sealed class CharityOperationTagInfo
    {
        public string? CategoryCode { get; set; }
        public string? CategoryText { get; set; }
        public string? ProjectName { get; set; }
        public string? GrantAgreementNumber { get; set; }
        public string? UserNotes { get; set; }

        public bool HasMetadata
            => !string.IsNullOrWhiteSpace(CategoryText)
               || !string.IsNullOrWhiteSpace(ProjectName)
               || !string.IsNullOrWhiteSpace(GrantAgreementNumber);
    }

    public static class CharityOperationNoteTags
    {
        private const string Prefix = "[CHARITY-LINK]";

        public static string Merge(
            string? userNotes,
            string? categoryCode,
            string? projectName,
            string? grantAgreementNumber,
            bool isRevenue)
        {
            var tokens = new List<string>();

            if (!string.IsNullOrWhiteSpace(categoryCode))
                tokens.Add($"CAT={categoryCode.Trim()}");

            if (!string.IsNullOrWhiteSpace(projectName))
                tokens.Add($"PROJ={Sanitize(projectName)}");

            if (!string.IsNullOrWhiteSpace(grantAgreementNumber))
                tokens.Add($"GRANT={Sanitize(grantAgreementNumber)}");

            if (isRevenue)
                tokens.Add("MODE=REV");
            else
                tokens.Add("MODE=PUR");

            var noteLines = new List<string>();
            if (tokens.Any())
                noteLines.Add($"{Prefix}|{string.Join('|', tokens)}");

            if (!string.IsNullOrWhiteSpace(userNotes))
                noteLines.Add(userNotes.Trim());

            return string.Join(Environment.NewLine, noteLines);
        }

        public static CharityOperationTagInfo Parse(string? notes)
        {
            var result = new CharityOperationTagInfo();

            if (string.IsNullOrWhiteSpace(notes))
                return result;

            var lines = notes.Replace("\r\n", "\n").Split('\n', StringSplitOptions.None).ToList();
            var firstLine = lines.FirstOrDefault()?.Trim();

            if (!string.IsNullOrWhiteSpace(firstLine) && firstLine.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                var parts = firstLine.Split('|', StringSplitOptions.RemoveEmptyEntries).Skip(1);
                string? mode = null;

                foreach (var part in parts)
                {
                    var pair = part.Split('=', 2);
                    if (pair.Length != 2)
                        continue;

                    var key = pair[0].Trim().ToUpperInvariant();
                    var value = pair[1].Trim();

                    switch (key)
                    {
                        case "CAT":
                            result.CategoryCode = value;
                            break;
                        case "PROJ":
                            result.ProjectName = value;
                            break;
                        case "GRANT":
                            result.GrantAgreementNumber = value;
                            break;
                        case "MODE":
                            mode = value;
                            break;
                    }
                }

                result.CategoryText = ToArabicCategory(result.CategoryCode, string.Equals(mode, "REV", StringComparison.OrdinalIgnoreCase));
                lines.RemoveAt(0);
            }

            var plain = string.Join(Environment.NewLine, lines).Trim();
            result.UserNotes = string.IsNullOrWhiteSpace(plain) ? null : plain;
            return result;
        }

        public static string ToArabicCategory(string? code, bool isRevenue)
        {
            if (string.IsNullOrWhiteSpace(code))
                return string.Empty;

            return code.Trim() switch
            {
                "ProgramSupplies" => "مستلزمات برامج ومساعدات",
                "OperationalSupplies" => "مستلزمات تشغيلية",
                "AssetPurchase" => "أصل أو تجهيز",
                "ServiceProcurement" => "خدمة أو تعاقد",
                "SelfRevenue" => "إيراد ذاتي",
                "ServiceRevenue" => "رسوم خدمة",
                "ProductRevenue" => "بيع منتجات",
                "EventRevenue" => "إيراد فعالية",
                _ => isRevenue ? "إيراد" : "مشتريات"
            };
        }

        private static string Sanitize(string value)
            => value.Replace("|", "/", StringComparison.Ordinal).Trim();
    }
}
