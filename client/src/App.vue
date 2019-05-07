<template>
  <div id="app">
    <template v-if="loggedIn">
      <button  @click="logoutHandler">Logout</button>
      <div class="nav">
        <router-link to="/ManageAppointments">Manage appointments</router-link> |
        <router-link to="/ManageFreeTime">Manage free time</router-link> |
        <router-link to="/CalendarPage">Calendar</router-link>
      </div>
    </template>
    <div v-else class="nav">
      <router-link to="/LoginForm">Sign in</router-link> |
      <router-link to="/RegisterAccount">Register</router-link>
    </div>

    <div class="nav">
      <router-link to="/">Home</router-link>|
      <router-link to="/SetAppointment">Set appointment</router-link>
    </div>

    <router-view/>
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
