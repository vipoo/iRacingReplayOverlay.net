namespace iRacingDirector.Plugin
{
    public class LeaderBoard
    {
        public readonly double StartTime;
        public readonly Driver[] Drivers;
        public readonly string RacePosition;
        public readonly string LapCounter;
    }

    public class Driver
    {
        public readonly int? Position;
        public readonly string CarNumber;
        public readonly string UserName;
        public readonly int PitStopCount;
        public readonly string ShortName;
    }
}