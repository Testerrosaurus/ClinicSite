import 'whatwg-fetch'

const debug = process.env.NODE_ENV !== 'production'
const apiUrl = debug ? 'http://localhost:5000/api' : '/api'


function getCookie(cname) {
  let name = cname + "="
  let decodedCookie = decodeURIComponent(document.cookie)
  let cookiesArray = decodedCookie.split(';')
  for(let i = 0; i <cookiesArray.length; i++) {
      let c = cookiesArray[i]
      while (c.charAt(0) == ' ') {
          c = c.substring(1)
      }
      if (c.indexOf(name) == 0) {
          return c.substring(name.length, c.length)
      }
  }
  return ""
}

function jsonResponsePromise(url, options) {
  if (options && options.method === 'POST') {
    let csrfToken = getCookie("XSRF-TOKEN")

    options.headers = {
      'X-CSRF-TOKEN': csrfToken
    }
  }

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
      method: 'POST',
      credentials: 'include'
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
      method: 'POST',
      credentials: 'include'
    }

    return jsonResponsePromise(apiUrl + '/appointments/removeDt?doctorId=' + info.doctorId + '&dtId=' + info.dtId, options)
  },

  getDb() {
    let options = {
      credentials: 'include'
    }

    return jsonResponsePromise(apiUrl + '/appointments/getDb', options)
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