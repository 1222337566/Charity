namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class MonthlyAmountDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public string Label => $"{Month:00}/{Year}";
    }
}
