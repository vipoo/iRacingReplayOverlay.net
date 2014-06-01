iRacingReplayOverlay.net
========================

The program tries to automate the entire process of creating an edited highlights of a iRacing replay.  

You configure the rules for cameras selection - then with iRacing replay loaded, you press the big button saying 'Begin'.

The program then does the following:

1. Analyse your replay session - looking for all the spins and crashes etc. - so it makes sure it can capture those

2. Next, capture some scenic views of the track - to use as a backdrop for the introduction sequence - this is where it details the race location, etc - and shows the qualifying positions

3. Then it positions the replay to just before the start of the race - and starts capturing again

Whilst it is capturing the race using your video capture program, its recording all the driver positions, fastest laps, overtakes etc.

After the race is finish the program stops your video capture program - and then u can press the 'Transcode Video' button.

This is where it joins the introduction scenic views recorded above and the main race - cuts the boring bits, and adds the overlays.

There was a lot of work to get quality data from iRacing - there were a few bugs in the data stream I had to work around!

You still need a conventional capture software to capture your game video to a video file - I use Nvidia's ShadowPlay.  
The program will send the required keystroke to the video capture software, once its position the replay to the start of the race.


Whats next

* I want to get more editing going - Its not identifying enough boring bits - so depending on the replay I only get 10-15mins chomped.
* I want to add the number of pits stops each drivers done to the leaderboard - and add them to the race commentary section
* Fix some bugs
* Perhaps provide a way to add the championship standings

Compiling/Building

As all development is pushed to the master branch - it is possible that at any point in time this code will not compile or will have
bugs.  Tags are created for 'stable' versions of the code. Review the message of each of the tags to find the versions
of dependent projects and other release notes.  

```git tag -l -n5```

The tagged versions should compile OK.
