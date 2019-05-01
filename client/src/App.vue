<template>
  <div id="app">
    <div id="nav">
      <router-link to="/RegisterAccount">Register account</router-link>|
      <router-link v-if="!loggedIn" to="/LoginForm">Sign in</router-link>
    </div>
    <div id="nav">
      <router-link to="/">Home</router-link>|
      <router-link to="/DbManagerPage">Manage database</router-link> |
      <router-link to="/AppointmentPage">Set appointment</router-link>
    </div>
    <router-view/>
    <button v-if="loggedIn" @click="logoutHandler">Logout</button>
  </div>
</template>

<script>
import api from './api/api.js'

import {mapState, mapMutations} from 'vuex'

export default {
  name: 'App',

  computed: {
    ...mapState([
      'loggedIn'
    ]),
  },

  methods: {
    ...mapMutations([
      'setLoggedIn'
    ]),

    logoutHandler() {
      api.logout()
      .then(response => {
        this.setLoggedIn(false)
        this.$router.push({path: '/'})
      })
    }
  }
}
</script>

<style lang="scss" src="./css/all.scss">
</style>
