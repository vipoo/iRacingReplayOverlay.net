import React from 'react'
import ReactDOM from 'react-dom'
import Listings from './listings.jsx'

export default React.createClass({
  getInitialState() {
    return {}
  },

  render() {
    return <div className="container">
            <div className="header clearfix">
              <h3 className="text-muted">iRacing Replay Director</h3>
            </div>

            <div className="jumbotron">
              <h1>iRacing Replay Director</h1>
              <p className="lead">Click here to download setup application</p>
              <p><a className="btn btn-lg btn-success" href="https://s3-ap-southeast-2.amazonaws.com/iracing-replay-director/main/setup.exe" role="button">Install ...</a></p>
              <p>Create short highlight videos of your favourite races!</p>
            </div>

            <div className="row marketing">
              <div className="col-lg-6">
                <table className="table">
                  <thead><tr><th>Beta Stream</th></tr></thead>
                  <tbody>
                  <tr><td><a className="btn btn-success" href="https://s3-ap-southeast-2.amazonaws.com/iracing-replay-director/beta/setup.exe" role="button">Install ...</a></td></tr>
                  <tr><td>
                    <p>Install and always have the latest beta version:</p>
                    <p>This version is updated often, and has had less testing/feedback than the main release</p>
                  </td></tr>
                  </tbody>
                </table>
              </div>

             <div className="col-lg-6">
                <table className="table">
                  <thead><tr><th>Test Stream</th></tr></thead>
                  <tbody>
                  <tr><td><a className="btn btn-success" href="https://s3-ap-southeast-2.amazonaws.com/iracing-replay-director/stest/setup.exe" role="button">Install ...</a></td></tr>
                  <tr><td>
                    <p>Install and always have the latest test version:</p>
                    <p>This version is updated on all code changes, and has had very limited testing - But its the latest and greatest version</p>
                  </td></tr>
                  </tbody>
                </table>
              </div>
            </div>

            <div className="row marketing">
              <div className="col-lg-12">
                <div className="scrollable">
                  <div className="items">
                      <img src="application.png" alt="Application Screenshot" />
                      <img src="settings.png" alt="General Settings" />
                  </div>
                </div>​
              </div>
            </div>
            
            <div className="row marketing">
              <div className="col-lg-12">
              <table className="table">
                <thead><tr><th><h4>Windows Warnings</h4></th></tr></thead>
                <tbody><tr><td>
                  <p>As this application and installer have not been certified by any third parties, windows will
                  present you with warnings about installing and running applications from an 'Unknown Publisher'</p>
                  <p>For a free open-source application, it does not make sense to purchase a certificate (usually in the hundreds of dollars), to 
                  allow winodws to install without warnings.  So you will need to accept these warnings from windows</p>
                  <p>Starting with windows 8, these warnings are presented, with the option to override hidden away.  The following screen images, show how to accept these warnings
                  to install iRacing Replay Director</p>
                  <div style={{marginLeft: "50px"}}>
                  <p><img src="./moreinfo.png" width="600px" height="239px" /></p>
                  <p><img src="./run.png"  width="600px" eeight="241px" /></p>
                  </div>
                  <p>You will need to accept the running of both the setup.exe, and the actual installed exe (iRacingReplayOverlay.exe)</p>
                </td></tr></tbody>
                </table>
              </div>
            </div>

            <div className="row marketing">
              <div className="col-lg-6">
                <Listings />
              </div>
            
              <div className="col-lg-6">
                <table className="table">
                  <thead>
                    <tr><th>Details</th></tr>
                  </thead>
                  <tbody>
                    <tr><td><a target="_new" href="https://github.com/vipoo/iRacingReplayOverlay.net/blob/master/README.md">View Readme File ...</a></td></tr>
                    <tr><td><a target="_new" href="https://github.com/vipoo/iRacingReplayOverlay.net/issues">View Issues</a></td></tr>
                    <tr><td><a target="_new" href="http://members.iracing.com/jforum/posts/list/3271807.page">iRacing Forum</a></td></tr>
                    <tr><td><a target="_new" href="https://ci.appveyor.com/project/vipoo/iracingreplayoverlay-net/history">Summary of changes</a></td></tr>
                  </tbody>
                </table>
              </div>

            </div>



            <footer className="footer">
              <p>&copy; Dean Netherton 2016</p>
            </footer> 
          </div>
  
  }

})