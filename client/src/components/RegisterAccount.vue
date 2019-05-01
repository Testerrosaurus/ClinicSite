<template>
  <div>
    <button @click="registerHandler">Register</button>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'RegisterAccount',

  data() {
    return {
      stub:''
    }
  },


  methods: {
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
        } else {
          console.log(response)
        }
      })
    }
  }
}
</script>
