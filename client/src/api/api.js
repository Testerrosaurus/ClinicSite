const debug = process.env.NODE_ENV !== 'production'
const apiUrl = debug ? 'http://localhost:5000/api/appointments' : '/api/appointments'

function jsonResponsePromise(url, options) {
  return new Promise(resolve => {
    fetch(url, options)
    .then(response => {
      if (response.ok)
        return response.json()
      else
        throw new Error(response.status + ' ' + response.statusText)
    })
    .catch(error => {
      throw new Error(error.message)
    })
    .then(json => {
      resolve(json)
    })
    .catch(error => {
      console.log('Error: ' + error.message)
    })
  })  
}



export default {
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