import React from 'react'
import ReactDOM from 'react-dom'
import 'whatwg-fetch'
import xmlToJson from './xmlToJson'

const request = new Request('https://s3-ap-southeast-2.amazonaws.com/iracingreplaydirector-test?delimiter=/&prefix=versions/', {
  headers: new Headers({
    'Accept': 'application/json'
  })
})

const Listing = React.createClass({
  
  render() {
    const contents = this.props.listings.ListBucketResult[0].Contents
    const keys = contents.map( c => c.Key[0]._text)
    const rows = keys.map(key => <tr key={key}><td>{key}</td></tr>)

    return (
     <table>
     <thead>
     <tr><th>
     Files
     </th></tr>
     </thead>
     <tbody>
     {rows}
     </tbody>
     </table>
    )
  }

})


fetch(request)
  .then(resp => resp.text())
  .then(text => xmlToJson.parseString(text))
  .then(json => ReactDOM.render(<Listing listings={json} />, document.getElementById('listing')))

