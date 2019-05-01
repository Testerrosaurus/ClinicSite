<template>
  <div>
    <h2>Sign in page</h2>
    <button @click="loginHandler">Login</button>
  </div>
</template>

<script>
import api from '../api/api.js'

import {mapMutations} from 'vuex'

export default {
  name: 'LoginForm',

  data() {
    return {
      stub:''
    }
  },


  methods: {
    ...mapMutations([
      'setLoggedIn'
    ]),

    loginHandler() {
      let info = {
        userName: 'userName1',
        password: 'Password123!',
        remember: false
      }

      api.login(info)
      .then(response => {
        this.setLoggedIn(true)
        this.$router.push({path: '/'})
      })
    }
  }
}
</script>
