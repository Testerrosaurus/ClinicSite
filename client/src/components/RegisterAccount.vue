<template>
  <div>
    <h2>Registration page</h2>
    <button @click="registerHandler">Register</button>
  </div>
</template>

<script>
import api from '../api/api.js'

import {mapMutations} from 'vuex'

export default {
  name: 'RegisterAccount',

  data() {
    return {
      stub:''
    }
  },


  methods: {
    ...mapMutations([
      'setLoggedIn'
    ]),

    registerHandler() {
      let info = {
        email: 'userName1email@mail.ru',
        userName: 'userName1',
        password: 'Password123!'
      }

      api.register(info)
      .then(response => {
        if (response === "Registered") {
          console.log('Registration succeeded')

          this.setLoggedIn(true)
          this.$router.push({path: '/'})
        } else {
          console.log(response)
        }
      })
    }
  }
}
</script>
