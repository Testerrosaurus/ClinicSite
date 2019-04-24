<template>
  <div class="test">
    Summary:
    <input v-model="summary" />

    <br />
    Date:
    <input type="date" v-model="date" />

    <br />
    Time:
    <select :value="time" @change="timeChanged($event)">
       <option v-for="val in values" :key="val" :value="val">
          {{val}}
        </option>
    </select>

    <br />
    <button @click="btn1ClickHandler">Register</button>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'AppointmentRegistrator',

  data() {
    return {
      summary: '',
      date: '',
      values: ['10:00', '16:20', '17:40'],
      time: '16:20'
    }
  },

  methods: {
    timeChanged(event) {
      this.time = event.target.value
    },

    btn1ClickHandler() {
      let info = {
        summary: this.summary,
        date: this.date,
        time: this.time
      }

      api.setAppointment(info).then(answer => console.log(answer))
    }
  }
}
</script>
