using iRacingSDK;

namespace iRacingDirector.Plugin
{
    public class EventData
    {
        private SessionData sessionData;

        public EventData(SessionData sessionData)
        {
            this.sessionData = sessionData;
        }

        public SessionData._WeekendInfo WeekendInfo
        { get { return sessionData.WeekendInfo; } }

        public SessionData._SessionInfo._Sessions._ResultsPositions[] QualifyingResults
        { get { return sessionData.SessionInfo.Sessions.Qualifying().ResultsPositions; } }

        public SessionData._DriverInfo._Drivers[] CompetingDrivers
        { get { return sessionData.DriverInfo.Drivers; } }

        public SessionData Raw { get { return sessionData; } }
    }
}