import React from 'react'
import ReactDOM from 'react-dom'
import 'whatwg-fetch'
import xmlToJson from './xmlToJson'

const request = new Request('https://s3-ap-southeast-2.amazonaws.com/iracing-replay-director?delimiter=/&prefix=versions/', {
  headers: new Headers({
    'Accept': 'application/json'
  })
})
const mockedXml = `<ListBucketResult>
    <Name>iracingreplaydirector-test</Name>
    <Prefix>versions/</Prefix>
    <Marker/>
    <MaxKeys>1000</MaxKeys>
    <Delimiter>/</Delimiter>
    <IsTruncated>false</IsTruncated>
    <Contents>
    <Key>versions/iRacingReplayOverlay.43.exe</Key>
    <LastModified>2015-12-19T11:33:02.000Z</LastModified>
    <ETag>26bc26b548e0a975cddd2e677c8e5209</ETag>
    <Size>1122304</Size>
    <StorageClass>STANDARD</StorageClass>
    </Contents>
    </ListBucketResult>`


export default React.createClass({
  
  componentWillMount() {
/*
fetch(request)
  .then(resp => resp.text())
  .then(() => mockedXml)
  .then(json => ReactDOM.render(<Listing listings={json} />, document.getElementById('listing')))

*/

    this.setState({listings: xmlToJson.parseString(mockedXml)})
  },

  render() {
    const contents = this.state.listings.ListBucketResult[0].Contents
    const keys = contents.map( c => c.Key[0]._text)
    const rows = keys.map(key => <tr key={key}><td>{key}</td></tr>)

    return (
     <table className="table">
     <thead>
     <tr><th>
      Manual Download of any version
     </th></tr>
     </thead>
     <tbody>
     {rows}
     </tbody>
     </table>
    )
  }

})