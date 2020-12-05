iRacingReplayDirector
=====================

This project is the sucessor of the discontinued iRacingReplayDirector.NET application developed by Vipoo. 

As Vipoo no longer owns an active license for iRacing - nor has the time to commit to this project - I will take over managing the original repository (vipoo/iRacingReplayDirector.net) and continue to coordinate future enhancements as well.  

For the ease of processing most of the development activites will be managed in the fork MerlinCooper/iRacingReplayDirector. 

Based on Azure DevOps continous integration is setup for two branches "master" and "Fast_Video_Recording_With_OBS". Whereas the master branch is being used for changes/modifications within the current program structure and the "Fast_Recording_With_OBS" branch is taking a different approach to significantly reduce the time to create a highlight video from long replays. 

Current CI/CD Status
--------------------
Master   
![Build Status](https://dev.azure.com/MerlinCooperDev/iRacingReplayDirector/_apis/build/status/iRacingReplayDirector_Master?branchName=master)

Fast_Video_Recording_With_OBS   
[![Build Status](https://dev.azure.com/MerlinCooperDev/iRacingReplayDirector/_apis/build/status/iRacingReplayDirector_OBS%20Fast%20Record%20Branch%20(alpha)?branchName=Fast_Video_Recording_With_OBS)](https://dev.azure.com/MerlinCooperDev/iRacingReplayDirector/_build/latest?definitionId=3&branchName=Fast_Video_Recording_With_OBS)

Installation
============

To install the application you need to download the sources and compile them at the moment. Automated process to provide most recent binaries is being worked on. 
 

Former Readme File (to be updated)  
==================================

 This program converts your iRacing replay's into edited video files for uploading to youtube.  It overlays the race with leaderboards and other race data.

It creates 2 videos from your replays; the full replay and a 10 minute highlight video.

The highlight video will contain the battles, incidents and remove any boring bits from the final video.

You can see some of the videos the application has created from my replays on my youtube channel at:
https://www.youtube.com/user/nethd/videos

https://www.youtube.com/watch?v=h6BETG8z-_w&feature=youtu.be

https://www.youtube.com/watch?v=Zs4VyBPOCHw&list=UUjHTFbxIv3vi7KGreofr8Aw

Please feel free to log feature requests, or bug issues in github issues tracker for the repo at: https://github.com/vipoo/iRacingReplayDirector.net/issues

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
