using System;

namespace iRacingDirector.Plugin
{
    public class FlashCardPagingCalculator
    {
        static int GetNumberOfPages(EventData eventData, int driversPerPage)
        {
            var numberOfDrivers = eventData.CompetingDrivers.Length - 1;
            var numberOfPages = Math.Min(numberOfDrivers / driversPerPage, 3);
            if (((float)numberOfDrivers % driversPerPage) != 0)
                numberOfPages++;

            return numberOfPages;
        }

        public static int GetPageNumber(EventData eventData, int driversPerPage, long duration, long timestamp)
        {
            var pagePeriod = (float)timestamp / duration;

            var numberOfPages = GetNumberOfPages(eventData, driversPerPage);
            var page = (int)Math.Floor(pagePeriod * numberOfPages);
            return Math.Min(page, numberOfPages - 1);
        }
    }
}