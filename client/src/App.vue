<template>
  <div id="app">
    <div id="nav">
      <router-link to="/RegisterAccount">Register Account</router-link>|
      <router-link to="/LoginForm">Log In</router-link>|
      <router-link to="/">Home</router-link>|
      <router-link to="/DbManagerPage">Manage Database</router-link> |
      <router-link to="/AppointmentPage">Set Appointment</router-link>
    </div>
    <router-view/>
  </div>
</template>

<script>
import api from './api/api.js'

export default {
  name: 'App',

  data() {
    return {
      loggedIn: false
    }
  },

  created() {
    api.isLoggedIn()
    .then(response => {
      console.log(response)
      this.loggedIn = response
    })
  }
}
</script>

<style lang="scss" src="./css/all.scss">
</style>
