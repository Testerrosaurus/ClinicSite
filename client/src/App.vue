<template>
  <div id="app">
    <template v-if="loggedIn">
      <button  @click="logoutHandler">Выйти</button>
      <div class="nav">
        <router-link to="/ManageAppointments">Управление записями</router-link> |
        <router-link to="/ManageFreeTime">Управление свободным временем</router-link> |
        <router-link to="/CalendarPage">Календарь</router-link>
      </div>
    </template>
    <div v-else class="nav">
      <router-link to="/LoginForm">Войти</router-link> |
      <router-link to="/RegisterAccount">Зарегистрироваться</router-link>
    </div>

    <div class="nav">
      <router-link to="/">Домой</router-link>|
      <router-link to="/SetAppointment">Записаться на прием</router-link>
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
