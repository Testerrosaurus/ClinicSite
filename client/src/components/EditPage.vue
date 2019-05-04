<template>
  <div class="blue-form">
    <br />
    <b-container>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="date">Date:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="date" v-model="appointment.date" type="date"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="time">Time:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="time" v-model="appointment.time" type="time"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="duration">Duration (in minutes):</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="duration" v-model="appointment.duration" type="number"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-4">
        <b-col cols="12">
          <b-button variant="success" @click="confirmHandler">{{buttonName}}</b-button>
        </b-col>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'EditPage',

  data() {
    return {
      appointments: [],
      doctors: [],
      currentDoctorName: '',

      appointment: {},
      appointment: {}
    }
  },

  computed: {
    buttonName() {
      if (this.appointment.status === "Unconfirmed") return 'Confirm'
      else return 'Save'
    }
  },
  
  created(){
    api.getAppointments()
    .then(db => {
      this.appointments = db.appointments
      this.doctors = db.doctors
      console.log(db)
      this.appointment = JSON.parse(JSON.stringify(db.appointments.find(a => a.id === Number(this.$route.params.id))))
    })
  },

  methods: {
    confirmHandler() {
      let info = {
        id: this.appointment.id,
        rowVersion: this.appointment.rowVersion,
        date: this.appointment.date,
        time: this.appointment.time,
        duration: this.appointment.duration
      }

      api.confirmAppointment(info)
      .then(response => {
        if (response === 'Confirmed') {
          api.getAppointments()
          .then(db => {
            this.appointments = db.appointments
            this.doctors = db.doctors
            console.log(db)
            this.appointment = JSON.parse(JSON.stringify(db.appointments.find(a => a.id === Number(this.$route.params.id))))

            if (this.appointment.status === "Unconfirmed") {
              this.appointment.status = "Confirmed"
              alert('Confirmed')
            } else {
              alert('Saved')
            }
          })
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        } else if (response === 'Invalid status') {
          alert('Fail: Invalid status')
        }
      })
    }
  }
}
</script>