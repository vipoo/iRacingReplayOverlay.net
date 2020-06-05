[![Board Status](https://merlincooper.visualstudio.com/e6f13734-ec60-4c67-b13c-08641fd28b90/aba5fdb2-a95f-4c59-a57a-c7bc2140e80e/_apis/work/boardbadge/7e4ac0d3-9a8d-4dd6-a01a-49909ee3bae6)](https://merlincooper.visualstudio.com/e6f13734-ec60-4c67-b13c-08641fd28b90/_boards/board/t/aba5fdb2-a95f-4c59-a57a-c7bc2140e80e/Microsoft.RequirementCategory)


iRacingReplayOverlay.net
========================

Please note - as I no longer own an active license for iRacing, nor do i have any time to commit to this project - I have to
abandon support for the project - its the challenge of open-source

I strongly encourage anyone who wishes - to fork this repo - and take over management/deployment/hosting of this application




[![Build status](https://ci.appveyor.com/api/projects/status/15batnoaijxhvfjd?svg=true)](https://ci.appveyor.com/project/vipoo/iracingreplayoverlay-net)

This program converts your iRacing replay's into edited video files for uploading to youtube.  It overlays the race with leaderboards and other race data.

It creates 2 videos from your replays; the full replay and a 10 minute highlight video.

The highlight video will contain the battles, incidents and remove any boring bits from the final video.

You can see some of the videos the application has created from my replays on my youtube channel at:
https://www.youtube.com/user/nethd/videos

https://www.youtube.com/watch?v=h6BETG8z-_w&feature=youtu.be

https://www.youtube.com/watch?v=Zs4VyBPOCHw&list=UUjHTFbxIv3vi7KGreofr8Aw

Please feel free to log feature requests, or bug issues in github issues tracker for the repo at: https://github.com/vipoo/iRacingReplayOverlay.net/issues

You configure the rules for cameras selection - then with iRacing replay loaded, you press the big button saying 'Begin'.

The program then does the following:

1. Analyse your replay session - looking for all the spins and crashes etc. - so it makes sure it can capture those

2. Next, capture some scenic views of the track - to use as a backdrop for the introduction sequence - this is where it details the race location, etc - and shows the qualifying positions

3. Then it positions the replay to just before the start of the race - and starts capturing again

Whilst it is capturing the race using your video capture program, its recording all the driver positions, fastest laps, overtakes etc.

After the race is finish the program stops your video capture program - and then u can press the 'Transcode Video' button.

This is where it joins the introduction scenic views recorded above and the main race - cuts the boring bits, and adds the overlays.

You still need a conventional capture software to capture your game video to a video file - I use Nvidia's ShadowPlay.  
The program will send the required keystroke to the video capture software, once its position the replay to the start of the race.

Installation
============

The application can be installed at: https://iracingreplaydirector.s3-ap-southeast-2.amazonaws.com/index.html

Or if you a feeling adventurous, you can try ealier releases at: https://iracingreplaydirector-beta.s3.amazonaws.com/index.html

How to use
===================

1. Start iRacing with the replay you wish to convert. (Make sure iRacing is running at a optimal resolution for video encoding. eg: 720p or 1080p)
2. With iRacing still running, switch back to the desktop and start iRacing Replay Director
3. Click the button "Configure Track Cameras"
4. Select the track of your replay currently loaded.
5. Select % options for the cameras you would like to use during the replay.
6. Ensure your video capture software is configured to respond to the key press ALT+F9 or at least F9 to activate and deactivate video capture
7. Press the Begin Capture button when ready to capture 
8. (You may need to press spacebar in iRacing to remove iRacing's replay controls) 
9. Once race capture is completed, you can then Encode your full race and highlight videos

Known Issues/Requirements
============

* Only support MPEG/H.264 video codec for capture 
* (not compatibile with Frap's custom codec)
* Only tested with audio codec PCM
* Video capture software must respond to ALT+F9 or F9 keypress to start/stop recording
* Replays must captured details of all competitors (Under the graphics options, set the Max Cars to a number greater than number of competitors before recording a replay session)
* Tested on Windows 7/Windows 8 only
* Requires 64x version of windows
