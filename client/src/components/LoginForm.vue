<template>
  <div>
    <h2>Sign in page</h2>
    <button @click="loginHandler">Войти</button>
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
        userName: 'userName2',
        password: 'Password123!',
        remember: false
      }

      api.login(info)
      .then(response => {
        if (response === 'Success') {
          this.setLoggedIn(true)
          this.$router.push({path: '/'})
        } else if (response === 'Fail') {
          alert('Invalid login information')
        }     
      })
    }
  }
}
</script>
