import 'whatwg-fetch'

const debug = process.env.NODE_ENV !== 'production'
const apiUrl = debug ? 'http://localhost:5000/api/appointments' : '/api/appointments'

function jsonResponsePromise(url, options) {
  return new Promise(resolve => {
    fetch(url, options)
    .then(response => {
      if (response.ok)
        return response.json()
      else
        throw new Error('Error: ' + response.status + ' ' + response.statusText)
    })
    .catch(error => {
      throw new Error('Error: ' + error.message)
    })
    .then(json => {
      resolve(json)
    })
    .catch(error => {
      console.log(error.message)
    })
  })  
}



export default {
  removeDt(info) {
    let options = {
      method: 'POST'
    }

    return jsonResponsePromise(apiUrl + '/removeDt?doctorId=' + info.doctorId + '&dtId=' + info.dtId, options)
  },

  getDb() {
    return jsonResponsePromise(apiUrl + '/getDb')
  },

  
  getFilteredDb() {
    return jsonResponsePromise(apiUrl + '/getFilteredDb')
  },

  setAppointment(info) {
    let options = {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(info)
    }

    return jsonResponsePromise(apiUrl + '/setAppointment', options)
  }
}