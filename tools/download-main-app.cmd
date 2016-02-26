set IRACING_REPLAY_OVERLAY=1.0.3.21

cd packages
rm -rf iRacingReplayOverlay.net.%IRACING_REPLAY_OVERLAY%
mkdir iRacingReplayOverlay.net.%IRACING_REPLAY_OVERLAY%
cd iRacingReplayOverlay.net.%IRACING_REPLAY_OVERLAY%
curl -L https://github.com/vipoo/iRacingReplayOverlay.net/releases/download/%IRACING_REPLAY_OVERLAY%/release.zip --output release.zip
7z x release.zip
rm release.zip
cd .\..\..
