import React from 'react'
import ReactDOM from 'react-dom'
import 'whatwg-fetch'
import xmlToJson from './xmlToJson'

const request = new Request('https://iracing-replay-director.s3.amazonaws.com?delimiter=/&prefix=versions/i', {
  headers: new Headers({
    'Accept': 'application/json'
  })
})
const mockedXml = `<ListBucketResult xmlns="http://s3.amazonaws.com/doc/2006-03-01/">
<Name>iracing-replay-director</Name>
<Prefix>versions/</Prefix>
<Marker/>
<MaxKeys>1000</MaxKeys>
<Delimiter>/</Delimiter>
<IsTruncated>false</IsTruncated>
<Contents>
<Key>versions/</Key>
<LastModified>2015-12-20T00:23:34.000Z</LastModified>
<ETag>"d41d8cd98f00b204e9800998ecf8427e"</ETag>
<Size>0</Size>
<StorageClass>STANDARD</StorageClass>
</Contents>
<Contents>
<Key>versions/iRacingReplayOverlay.1.0.1.45.exe</Key>
<LastModified>2015-12-20T00:44:59.000Z</LastModified>
<ETag>"c9dbf4d28d53b8ee2ca3f19bf8228ec1"</ETag>
<Size>1122304</Size>
<StorageClass>STANDARD</StorageClass>
</Contents>
<Contents>
<Key>versions/iRacingReplayOverlay.1.0.1.46.exe</Key>
<LastModified>2015-12-20T00:44:59.000Z</LastModified>
<ETag>"c9dbf4d28d53b8ee2ca3f19bf8228ec1"</ETag>
<Size>1122304</Size>
<StorageClass>STANDARD</StorageClass>
</Contents>
</ListBucketResult>`

export default React.createClass({
  
  getInitialState() {
    return {listings: []}
  },

  componentWillMount() {

    fetch(request)
      .then(resp => resp.text())
      .then(text => xmlToJson.parseString(text))
      .then(json => this.setState({listings: json.ListBucketResult[0].Contents}))

    //const list = xmlToJson.parseString(mockedXml)
    //this.setState({listings: list.ListBucketResult[0].Contents})
  },

  render() {
    const keys = this.state.listings.map( c => c.Key[0]._text)
    const sortedKeys = keys.sort( (a, b) => a === b ? 0 : a < b ? 1 : -1)
    const rows = sortedKeys.map(key => <tr key={key}><td><a href={"https://s3-ap-southeast-2.amazonaws.com/iracing-replay-director/" + key}>{key}</a></td></tr>)

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