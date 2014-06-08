// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.

using iRacingReplayOverlay.Phases.Analysis;
using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Support;
using iRacingSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iRacingReplayOverlay.Phases.Direction
{
    public class RuleBattle : IVetoRule
    {
        enum BattlePosition { Started, Inside, Finished, Outside };
        struct BattleState
        {
            public BattleState(BattlePosition state) : this(state, null) { }
            public BattleState(BattlePosition state, SessionData._DriverInfo._Drivers driver)
            {
                this.State = state;
                this.Driver = driver;
            }
            public BattlePosition State;
            public SessionData._DriverInfo._Drivers Driver;
        }

        readonly CameraControl cameraControl;
        readonly RemovalEdits removalEdits;
        readonly TimeSpan battleStickyTime;
        readonly TimeSpan battleGap;

        bool isInBattle = false;
        TimeSpan battleEndTime;
        Car battleFollower;
        Car battleLeader;
        Action directionAction;
        TrackCamera camera;
        SessionData._DriverInfo._Drivers car;

        public RuleBattle(CameraControl cameraControl, RemovalEdits removalEdits, TimeSpan battleStickyTime, TimeSpan battleGap)
        {
            this.cameraControl = cameraControl;
            this.removalEdits = removalEdits;
            this.battleStickyTime = battleStickyTime;
            this.battleGap = battleGap;
        }

        public bool IsActive(DataSample data)
        {
            var state = GetBattlePosition(data);

            switch(state.State)
            {
                case BattlePosition.Started:
                    directionAction = () =>
                    {
                        removalEdits.InterestingThingHappend(data);
                        SwitchToBattle(data, state.Driver);
                    };
                    return true;

                case BattlePosition.Inside:
                    directionAction = () =>
                    {
                        UpdateCameraIfOvertake(data);
                        removalEdits.InterestingThingHappend(data);
                    };
                    return true;

                case BattlePosition.Finished:
                    directionAction = () => removalEdits.InterestingThingHappend(data);
                    return true;

                case BattlePosition.Outside:
                    directionAction = () => { };
                    return false;
            }

            throw new Exception("Invalid Battle state {0}".F(state));
        }

        public void Direct(DataSample data)
        {
            directionAction();
        }

        public void Redirect(DataSample data)
        {
            Trace.WriteLine("{0} Changing camera back to driver: {1}; camera: {2}; within {3}".F(data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName, battleGap), "INFO");
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        void SwitchToBattle(DataSample data, SessionData._DriverInfo._Drivers follower)
        {
            battleFollower = data.Telemetry.Cars[follower.CarIdx];
            battleLeader = data.Telemetry.Cars.First(c => c.Position == battleFollower.Position - 1);

            camera = cameraControl.FindACamera(CameraAngle.LookingInfrontOfCar, CameraAngle.LookingBehindCar,  CameraAngle.LookingAtCar);
            car = ChangeCarForCamera(data, camera, follower);

            Trace.WriteLine("{0} Changing camera to driver: {1}; camera: {2}; within {3}".F(data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName, battleGap), "INFO");
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        void UpdateCameraIfOvertake(DataSample data)
        {
            if (BattlersHaveSwappedPositions(data))
            {
                battleEndTime = data.Telemetry.SessionTimeSpan + this.battleStickyTime;
                Trace.WriteLine("{0} {1} has overtaken {2}".F(data.Telemetry.SessionTimeSpan, battleFollower.UserName, battleLeader.UserName), "INFO");
                SwitchToBattle(data, battleLeader.Driver);
            }
        }

        bool BattlersHaveSwappedPositions(DataSample data)
        {
            var followersLatestPosition = data.Telemetry.Cars[battleFollower.CarIdx].Position;
            var leadersLatestPosition = data.Telemetry.Cars[battleLeader.CarIdx].Position;

            return followersLatestPosition < leadersLatestPosition;
        }

        static SessionData._DriverInfo._Drivers ChangeCarForCamera(DataSample data, TrackCamera camera, SessionData._DriverInfo._Drivers driver)
        {
            if (driver == null)
                return null;

            var car = data.Telemetry.Cars[driver.CarIdx];

            if (camera.CameraAngle == CameraAngle.LookingBehindCar)
            {
                Trace.WriteLine("{0} Changing to forward car, with reverse camera".F(data.Telemetry.SessionTimeSpan));
                car = data.Telemetry.Cars.First(c => c.Position == car.Position - 1);
                return data.SessionData.DriverInfo.Drivers[car.CarIdx];
            }

            return driver;
        }

        BattleState GetBattlePosition(DataSample data)
        {
            if (isInBattle)
            {
                if (!HasBattleTimeout(data))
                    return new BattleState(BattlePosition.Inside);

                return SearchForNextBattle(data, notFound: () => new BattleState(BattlePosition.Finished) );
            }

            return SearchForNextBattle(data, notFound: () => new BattleState(BattlePosition.Outside));
        }

        BattleState SearchForNextBattle(DataSample data, Func<BattleState> notFound)
        {
            var battleDriver = FindNewBattle(data);
            if (battleDriver == null)
            {
                isInBattle = false;
                return notFound();
            }

            isInBattle = true;
            battleEndTime = data.Telemetry.SessionTimeSpan + this.battleStickyTime;

            return new BattleState(BattlePosition.Started, battleDriver);
        }

        bool HasBattleTimeout(DataSample data)
        {
            return data.Telemetry.SessionTimeSpan > battleEndTime;
        }

        SessionData._DriverInfo._Drivers FindNewBattle(DataSample data)
        {
            var range = battleGap.TotalSeconds;

            var distances = data.Telemetry.CarIdxDistance
                    .Select((d, i) => new { CarIdx = i, Distance = d })
                    .Skip(1)
                    .Where(d => d.Distance > 0)
                    .OrderByDescending(d => d.Distance)
                    .ToList();

            if (distances.Count == 0)
                return null;

            var gap = Enumerable.Range(1, distances.Count - 1)
                    .Select(i => new
                    {
                        CarIdx = distances[i].CarIdx,
                        Distance = distances[i - 1].Distance - distances[i].Distance,
                        Position = i
                    });

            var timeGap = gap.Select(g => new
            {
                CarIdx = g.CarIdx,
                Time = g.Distance * data.Telemetry.Session.ResultsAverageLapTime,
                Position = g.Position
            });

            var closest = timeGap
                .OrderBy(d => d.Position)
                .FirstOrDefault(d => d.Time < range);

            return closest == null ? null : data.SessionData.DriverInfo.Drivers[closest.CarIdx];
        }
    }
}
