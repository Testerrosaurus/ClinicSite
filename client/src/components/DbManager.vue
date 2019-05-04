<template>
  <div>
    Doctor:
    <select :value="currentDoctorName" @change="doctorChanged($event)">
      <option v-for="doctor in [{name:''}].concat(doctors)" :key="doctor.name" :value="doctor.name">
        {{doctor.name}}
      </option>
    </select>

    <table>
      <tr>
        <td>Date</td>
        <td>Time</td>
        <td>Status</td>
        <td></td>
      </tr>
      <tr v-for="a in filteredAppointments" :key="a.id">
        <td>{{a.date}}</td>
        <td>{{a.time}}</td>
        <td>{{a.status}}</td>
        <td><Button @click="confirmHandler(a)">Confirm</Button></td>
        <td><Button @click="removeHandler(a)">Remove</Button></td>
      </tr>
    </table>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'DbManager',

  data() {
    return {
      appointments: [],
      doctors: [],
      currentDoctorName: ''
    }
  },

  computed: {
    filteredAppointments() {
      return this.appointments.filter(a => a.doctor.name === this.currentDoctorName)
    }
  },
  
  created(){
    api.getDb()
    .then(db => {
      this.appointments = db.appointments
      this.doctors = db.doctors
      console.log(db)
    })
  },

  methods: {
    doctorChanged(event) {
      this.currentDoctorName = event.target.value
    },

    confirmHandler(appointment) {
      let info = {
        id: appointment.id,
        rowVersion: appointment.rowVersion
      }

      api.confirmAppointment(info)
      .then(response => {
        if (response === 'Confirmed') {
          this.appointments.find(a => a.id === appointment.id).status = 'Confirmed'
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        } else if (response === 'Invalid status') {
          alert('Fail: Invalid status')
        }
      })
    },

    removeHandler(appointment) {
      let info = {
        id: appointment.id,
        rowVersion: appointment.rowVersion
      }

      api.removeAppointment(info)
      .then(response => {
        if (response === 'Removed') {
          this.appointments.splice(this.appointments.findIndex(a => a.id === appointment.id), 1)
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        }
      })
    }
  }
}
</script>
