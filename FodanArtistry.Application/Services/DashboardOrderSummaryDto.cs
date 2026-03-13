namespace FodanArtistry.Application.Services
{
    public class DashboardOrderSummaryDto
    {
        public int TodayCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public int WeekCount { get; set; }
        public decimal WeekRevenue { get; set; }
        public int MonthCount { get; set; }
        public decimal MonthRevenue { get; set; }
        public int PendingCount { get; set; }
        public int ProcessingCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int YearCount { get; internal set; }
        public decimal YearRevenue { get; internal set; }
        public decimal PendingRevenue { get; internal set; }
        public decimal ProcessingRevenue { get; internal set; }
        public decimal CompletedRevenue { get; internal set; }
        public decimal CancelledRevenue { get; internal set; }
        public decimal AverageOrderValue { get; internal set; }
    }
}