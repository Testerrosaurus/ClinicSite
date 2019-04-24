<template>
  <div class="test">
    <input v-model="input1" />
    <button @click="btn1ClickHandler">Register</button>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'AppointmentRegistrator',

  data() {
    return {
      input1: ''
    }
  },

  methods: {
    btn1ClickHandler() {
      console.log(this.input1)
      api.setAppointment(this.input1).then(answer => console.log(answer))
    }
  }
}
</script>
