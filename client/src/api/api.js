import 'whatwg-fetch'

const debug = process.env.NODE_ENV !== 'production'
const apiUrl = debug ? 'http://localhost:5000/api' : '/api'

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
  isLoggedIn() {
    let options = {
      credentials: 'include'
    }

    return jsonResponsePromise(apiUrl + '/account/isLoggedIn', options)
  },

  register(info) {
    let options = {
      method: 'POST'
    }

    return jsonResponsePromise(apiUrl + '/account/register?email=' + info.email + '&userName=' + info.userName + '&password=' + info.password, options)
  },

  login(info) {
    let options = {
      method: 'POST',
      credentials: 'include'
    }

    return jsonResponsePromise(apiUrl + '/account/login?userName=' + info.userName + '&password=' + info.password + '&remember=' + info.remember, options)
  },

  logout() {
    let options = {
      method: 'POST',
      credentials: 'include'
    }

    return jsonResponsePromise(apiUrl + '/account/logout', options)
  },

  removeDt(info) {
    let options = {
      method: 'POST'
    }

    return jsonResponsePromise(apiUrl + '/appointments/removeDt?doctorId=' + info.doctorId + '&dtId=' + info.dtId, options)
  },

  getDb() {
    return jsonResponsePromise(apiUrl + '/appointments/getDb')
  },

  
  getFilteredDb() {
    return jsonResponsePromise(apiUrl + '/appointments/getFilteredDb')
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

    return jsonResponsePromise(apiUrl + '/appointments/setAppointment', options)
  }
}