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

using iRacingReplayOverlay.Phases.Capturing;
using iRacingReplayOverlay.Phases.Direction.Support;
using iRacingReplayOverlay.Support;
using iRacingSDK;
using iRacingSDK.Support;
using System;
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
        readonly EditMarker editMarker;
        readonly TimeSpan battleStickyPeriod;
        readonly TimeSpan battleCameraChangePeriod;
        readonly TimeSpan battleGap;
        readonly double battleFactor;

        bool isInBattle = false;
        TimeSpan battleEndTime;
        TimeSpan cameraChangeTime;
        Car battleFollower;
        Car battleLeader;
        Action directionAction;
        TrackCamera camera;
        SessionData._DriverInfo._Drivers car;

        public RuleBattle(CameraControl cameraControl, RemovalEdits removalEdits, TimeSpan cameraStickyPeriod, TimeSpan battleStickyPeriod, TimeSpan battleGap, double battleFactor)
        {
            this.cameraControl = cameraControl;
            this.editMarker = removalEdits.For(InterestState.Battle);
            this.battleStickyPeriod = battleStickyPeriod;
            this.battleCameraChangePeriod = cameraStickyPeriod;
            this.battleGap = battleGap;
            this.battleFactor = battleFactor;
        }

        public bool IsActive(DataSample data)
        {
            var state = GetBattlePosition(data);

            switch(state.State)
            {
                case BattlePosition.Started:
                    directionAction = () =>
                    {
                        SwitchToBattle(data, state.Driver);
                        editMarker.Start(battleFollower.CarIdx);
                    };
                    return true;

                case BattlePosition.Inside:
                    directionAction = () =>
                    {
                        UpdateBattleCamera(data);
                        UpdateCameraIfOvertake(data);
                    };
                    return true;

                case BattlePosition.Finished:
                    directionAction = () => editMarker.Stop(battleFollower.CarIdx);
                    return true;

                case BattlePosition.Outside:
                    directionAction = () => { };
                    return false;
            }

            throw new Exception("Invalid Battle state {0}".F(state));
        }

        void UpdateBattleCamera(DataSample data)
        {
            if (data.Telemetry.SessionTimeSpan <= cameraChangeTime)
                return;

            cameraChangeTime = data.Telemetry.SessionTimeSpan + this.battleCameraChangePeriod;

            camera = cameraControl.FindACamera(CameraAngle.LookingInfrontOfCar, CameraAngle.LookingBehindCar, CameraAngle.LookingAtCar);
            car = ChangeCarForCamera(data, camera, battleFollower.Driver);

            TraceInfo.WriteLine("{0} Changing camera to driver: {1}; camera: {2}", data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName);
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        public void Direct(DataSample data)
        {
            directionAction();
        }

        public void Redirect(DataSample data)
        {
            TraceInfo.WriteLine("{0} Changing camera back to driver: {1}; camera: {2}; within {3}", data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName, battleGap);
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        void SwitchToBattle(DataSample data, SessionData._DriverInfo._Drivers follower)
        {
            battleFollower = data.Telemetry.Cars[follower.CarIdx];
            battleLeader = data.Telemetry.Cars.First(c => c.Position == battleFollower.Position - 1);

            camera = cameraControl.FindACamera(CameraAngle.LookingInfrontOfCar, CameraAngle.LookingBehindCar,  CameraAngle.LookingAtCar);
            car = ChangeCarForCamera(data, camera, follower);

            TraceInfo.WriteLine("{0} Changing camera to driver: {1}; camera: {2}; within {3}", data.Telemetry.SessionTimeSpan, car.UserName, camera.CameraName, battleGap);
            iRacing.Replay.CameraOnDriver((short)car.CarNumber, camera.CameraNumber);
        }

        void UpdateCameraIfOvertake(DataSample data)
        {
            if (BattlersHaveSwappedPositions(data))
            {
                battleEndTime = data.Telemetry.SessionTimeSpan + this.battleStickyPeriod;
                TraceInfo.WriteLine("{0} {1} has overtaken {2}", data.Telemetry.SessionTimeSpan, battleFollower.UserName, battleLeader.UserName);
                SwitchToBattle(data, battleLeader.Driver);
                editMarker.Start(battleFollower.CarIdx);
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
                TraceInfo.WriteLine("{0} Changing to forward car, with reverse camera", data.Telemetry.SessionTimeSpan);
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
            var battleDriver = Battle.Find(data, battleGap, battleFactor, Settings.Default.PreferredDrivers);
            if (battleDriver == null)
            {
                isInBattle = false;
                return notFound();
            }

            isInBattle = true;
            battleEndTime = data.Telemetry.SessionTimeSpan + this.battleStickyPeriod;
            cameraChangeTime = data.Telemetry.SessionTimeSpan + this.battleCameraChangePeriod;
            return new BattleState(BattlePosition.Started, battleDriver);
        }

        bool HasBattleTimeout(DataSample data)
        {
            if (data.Telemetry.SessionTimeSpan > cameraChangeTime && !Battle.IsInBattle(data, battleGap, battleFollower.Driver, battleLeader.Driver))
            {
                TraceInfo.WriteLine("{0} Battle has stopped.", data.Telemetry.SessionTimeSpan);
                return true;
            }

            return data.Telemetry.SessionTimeSpan > battleEndTime;
        }

        public string Name
        {
            get { return GetType().Name; }
        }
    }
}
