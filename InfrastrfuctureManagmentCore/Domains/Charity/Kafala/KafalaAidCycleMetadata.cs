using System;
using System.Linq;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Kafala
{
    public sealed class KafalaAidCycleLineMetadata
    {
        public Guid? KafalaCaseId { get; init; }
        public string? KafalaCaseNumber { get; init; }
        public Guid? SponsorId { get; init; }
        public string? SponsorName { get; init; }
        public string? SponsorshipType { get; init; }
        public string? Frequency { get; init; }
    }

    public static class KafalaAidCycleMetadata
    {
        public const string CyclePrefix = "KAFALA_CYCLE";
        public const string LinePrefix = "KAFALA_LINE";

        public static string BuildCycleNotes(Guid? sponsorId, string? sponsorName, DateTime plannedDate)
        {
            var scope = sponsorId.HasValue ? "Sponsor" : "All";
            return string.Join('|',
                CyclePrefix,
                $"scope={scope}",
                $"sponsorId={sponsorId?.ToString() ?? string.Empty}",
                $"sponsorName={Escape(sponsorName)}",
                $"planned={plannedDate:yyyy-MM-dd}");
        }

        public static string BuildLineNotes(KafalaCase kafalaCase)
        {
            return string.Join('|',
                LinePrefix,
                $"caseId={kafalaCase.Id}",
                $"caseNo={Escape(kafalaCase.CaseNumber)}",
                $"sponsorId={kafalaCase.SponsorId}",
                $"sponsor={Escape(kafalaCase.Sponsor?.FullName)}",
                $"type={Escape(kafalaCase.SponsorshipType)}",
                $"freq={Escape(kafalaCase.Frequency)}");
        }

        public static bool IsKafalaCycleNotes(string? notes)
            => !string.IsNullOrWhiteSpace(notes) && notes.StartsWith(CyclePrefix, StringComparison.OrdinalIgnoreCase);

        public static bool TryParseLineNotes(string? notes, out KafalaAidCycleLineMetadata metadata)
        {
            metadata = new KafalaAidCycleLineMetadata();
            if (string.IsNullOrWhiteSpace(notes) || !notes.StartsWith(LinePrefix, StringComparison.OrdinalIgnoreCase))
                return false;

            Guid? caseId = null;
            Guid? sponsorId = null;
            string? caseNo = null;
            string? sponsorName = null;
            string? type = null;
            string? frequency = null;

            var parts = notes.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts.Skip(1))
            {
                var idx = part.IndexOf('=');
                if (idx <= 0 || idx == part.Length - 1)
                    continue;

                var key = part[..idx];
                var value = Unescape(part[(idx + 1)..]);

                switch (key)
                {
                    case "caseId" when Guid.TryParse(value, out var parsedCaseId):
                        caseId = parsedCaseId;
                        break;
                    case "caseNo":
                        caseNo = value;
                        break;
                    case "sponsorId" when Guid.TryParse(value, out var parsedSponsorId):
                        sponsorId = parsedSponsorId;
                        break;
                    case "sponsor":
                        sponsorName = value;
                        break;
                    case "type":
                        type = value;
                        break;
                    case "freq":
                        frequency = value;
                        break;
                }
            }

            metadata = new KafalaAidCycleLineMetadata
            {
                KafalaCaseId = caseId,
                KafalaCaseNumber = caseNo,
                SponsorId = sponsorId,
                SponsorName = sponsorName,
                SponsorshipType = type,
                Frequency = frequency
            };

            return caseId.HasValue;
        }

        private static string Escape(string? value)
            => string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Replace("|", "/").Replace("=", ":").Trim();

        private static string Unescape(string value) => value.Trim();
    }
}
