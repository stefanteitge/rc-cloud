﻿namespace RcCloud.DateScraper.Application.Myrcm.Services
{
    public class MyrcmCountryCode
    {
        private readonly int code;

        private MyrcmCountryCode(int code)
        {
            this.code = code;
        }

        public static MyrcmCountryCode Germany => new(3);
    }
}
