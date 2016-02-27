using iRacingSDK;
using System.Linq;

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

        public SessionData._SessionInfo._Sessions Race
        { get { return sessionData.SessionInfo.Sessions.First(s => s.SessionType.ToLower().Contains("race")); } }

        public SessionData._SessionInfo._Sessions._ResultsPositions[] Results
        { get { return Race.ResultsPositions ?? new SessionData._SessionInfo._Sessions._ResultsPositions[0]; } }
        public SessionData Raw { get { return sessionData; } }
    }
}