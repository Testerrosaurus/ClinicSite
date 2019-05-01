<template>
  <div>
    <button @click="loginHandler">Login</button>
    <button @click="test">test</button>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'LoginForm',

  data() {
    return {
      stub:''
    }
  },


  methods: {
    loginHandler() {
      let info = {
        userName: 'userName1',
        password: 'Password123!',
        remember: false
      }

      api.login(info)
      .then(response => {
        console.log(response)
      })
    },

    test() {
      api.isLoggedIn()
    .then(response => {
      console.log(response)
    })
    }
  }
}
</script>
