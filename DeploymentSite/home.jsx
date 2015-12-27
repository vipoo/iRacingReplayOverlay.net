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
              <p><a className="btn btn-lg btn-success" href="https://iracingreplaydirector.s3-ap-southeast-2.amazonaws.com/setup.exe" role="button">Install ...</a></p>
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
                  <tr><td><a className="btn btn-success" href="https://s3-ap-southeast-2.amazonaws.com/iracing-replay-director/test/setup.exe" role="button">Install ...</a></td></tr>
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
              <div className="col-lg-6">
                <Listings />
              </div>
            
              <div className="col-lg-6">
                <table className="table">
                  <thead>
                    <tr><th>Details</th></tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td><a target="_new" href="https://github.com/vipoo/iRacingReplayOverlay.net/blob/master/README.md">View Readme File ...</a></td>
                    </tr>
                    <tr><td><a target="_new" href="https://github.com/vipoo/iRacingReplayOverlay.net/issues">View Issues</a></td></tr>
                    <tr><td><a target="_new" href="http://members.iracing.com/jforum/posts/list/3271807.page">iRacing Forum</a></td></tr>
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