<template>
  <div id="appointment-registrator">
    Patient Name:
    <input v-model="patientName" />

    <br />
    Procedure:
    <select :value="procedureName" @change="procedureChanged($event)">
      <option v-for="pn in proceduresNamesList" :key="pn" :value="pn">
        {{pn}}
      </option>
    </select>

    <br />
    Doctor:
    <select :value="doctorName" @change="doctorChanged($event)">
      <option v-for="d in doctorsNamesList" :key="d" :value="d">
        {{d}}
      </option>
    </select>

    <br />
    Date:
    <select :value="date" @change="dateChanged($event)">
      <option v-for="(date, index) in datesList" :key="index" :value="date">
        {{date}}
      </option>
    </select>

    <br />
    Time:
    <select :value="currentAppointment.time" @change="timeChanged($event)">
      <option v-for="a in appointmentsList" :key="a.time" :value="a.time">
        {{a.time}}
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
      patientName: '',

      procedureName: '',
      doctorName: '',
      date: '',
      currentAppointment: {time: ''},

      appointments: [],
      procedures: []
    }
  },

  computed: {
    proceduresNamesList() { 
      return [''].concat(this.procedures.map(p => p.name))
    },

    doctorsNamesList() {
      let procedure = this.procedures.find(p => p.name == this.procedureName)

      if (procedure)
        return [''].concat(procedure.doctors)
      else
        return ['']
    },

    datesList() {
      return [''].concat(this.appointments.filter(a => a.doctor.name === this.doctorName).map(a => a.date))
    },

    appointmentsList() {
      return [{time: ''}].concat(this.appointments.filter(a => a.date === this.date && a.doctor.name === this.doctorName))
    }
  },

  created(){
    api.getFilteredDb()
    .then(db => {
      this.appointments = db.appointments
      this.procedures = db.procedures
    })
  },

  methods: {
    procedureChanged(event) {
      this.procedureName = event.target.value
      this.doctorName = ''
    },

    doctorChanged(event) {
      this.doctorName = event.target.value
      this.date = ''
    },

    dateChanged(event) {
      this.date = event.target.value
      this.currentAppointment = {time: ''}
    },

    timeChanged(event) {
      this.currentAppointment = this.appointmentsList.find(a => a.time === event.target.value)
    },

    btn1ClickHandler() {
      let info = {
        patient: this.patientName,
        procedure: this.procedureName,
        id: this.currentAppointment.id,
        rowVersion: this.currentAppointment.rowVersion
      }

      api.setAppointment(info).then(answer => {
        if (answer === 'Created') {
          this.$router.push('/AppointmentRegistered')
        }
        else if (answer === 'Invalid info') {
          alert('Invalid information')
        }
        else if (answer === 'Info changed') {
          api.getDb()
          .then(db => {
            this.doctors = db
            alert('Fail: Item was modified since last page load')
          })
        }
      })
    }
  }
}
</script>
