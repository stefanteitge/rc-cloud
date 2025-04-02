namespace RcCloud.DateScraper.Application.Myrcm.Services
{
    public class CountryCode
    {
        private readonly int code;

        private CountryCode(int code)
        {
            this.code = code;
        }

        public static CountryCode Germany => new(3);
    }
}
