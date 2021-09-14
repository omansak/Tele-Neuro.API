using PlayCore.Core.Extension;

namespace PlayCore.Core.Model
{
    public class PageInfo
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPage => MathExtensions.UpDivision(TotalCount, PageSize);
    }
}
