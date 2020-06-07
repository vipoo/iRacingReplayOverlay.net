using iRacingDirector.Plugin;
using System;
using System.Drawing;
using System.Linq;

namespace JockeOverlays
{
    public partial class MyPlugin
    {
        public Driver[] PreferredDriverNames;

        void DrawOutroFlashCard(int page)
        {
            var r = DrawFlashCardHeading("Race Results");

            DrawFlashCardOutro(r, page);
        }

        void DrawFlashCardOutro(GraphicRect r, int page)
        {
            var rsession = EventData.Race;
            var results = EventData.Results;

            var offset = 5;
            Graphics.InRectangle(FlashCardLeft, r.Rectangle.Top, FlashCardWidth, 10)
                .WithPen(Styles.ThickBlackPen)
                .DrawLine(FlashCardLeft + 8, r.Rectangle.Top - offset, FlashCardLeft + FlashCardWidth - 16, r.Rectangle.Top - offset);

            var LeaderTime = TimeSpan.FromSeconds(results[0].Time);

            foreach (var racerResult in results.Skip(DriversPerPage * page).Take(DriversPerPage))
            {
                var driver = EventData.GetCompetingDriverByIndex(racerResult.CarIdx);

                var Gap = TimeSpan.FromSeconds(racerResult.Time) - LeaderTime; // Gap calculation
                if (Gap == TimeSpan.Zero) //For the leader we want to display the race duration
                    Gap = LeaderTime;

                r.WithBrush(PreferredDriverNames.Any(d => d.UserName == driver.UserName) ? Styles.RedBrush : Styles.BlackBrush);

                r.Center(cg => cg
                            .DrawText(racerResult.Position.ToString())
                            .AfterText(racerResult.Position.ToString())
                            .MoveRight(1)
                            .WithFont(Settings.FontName, 16, FontStyle.Bold)
                            .DrawText(racerResult.Position.Ordinal()))
                    .ToRight(width: 190, left: 30)
                    .DrawText(Gap.ToString("hh\\:mm\\:ss\\.fff"))
                    .ToRight(width: 80, left: 20)
                    .DrawText(driver.CarNumber)
                    .ToRight(width: 350)
                    .DrawText(driver.UserName);

                r = r.ToBelow();

                Graphics.InRectangle(FlashCardLeft, r.Rectangle.Top, FlashCardWidth, 10)
                    .WithPen(Styles.ThickBlackPen)
                    .DrawLine(FlashCardLeft + 8, r.Rectangle.Top - offset, FlashCardLeft + FlashCardWidth - 16, r.Rectangle.Top - offset);
            }
        }
    }
}
