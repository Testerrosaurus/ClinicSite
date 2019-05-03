<template>
  <div id="appointment-registrator">
    <br />
    <b-container>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="patient">Patient Name:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="patient" v-model="patientName"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="procedure">Procedure:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="procedure" :value="procedureName" @change="procedureChanged($event)">
            <option value="" disabled="true">
              Select
            </option>
            <option v-for="pn in proceduresNamesList" :key="pn" :value="pn">
              {{pn}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="doctor">Doctor:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="doctor" :disabled="procedureName === ''" :value="doctorName" @change="doctorChanged($event)">
            <option value="" disabled="true">
              Select
            </option>
            <option v-for="d in doctorsNamesList" :key="d" :value="d">
              {{d}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="date">Date:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="date" :disabled="doctorName === ''" :value="date" @change="dateChanged($event)">
            <option value="" disabled="true">
              Select
            </option>
            <option v-for="(date, index) in datesList" :key="index" :value="date">
              {{date}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="time">Time:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="time" :disabled="date === ''" :value="currentAppointment.time" @change="timeChanged($event)">
            <option value="" disabled="true">
              Select
            </option>
            <option v-for="a in appointmentsList" :key="a.time" :value="a.time">
              {{a.time}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
      <b-row class="my-4">
        <b-col cols="12">
          <b-button variant="success" @click="btn1ClickHandler">Register</b-button>
        </b-col>
      </b-row>
    </b-container>
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
      procedures: [],
      time: null
    }
  },

  computed: {
    proceduresNamesList() { 
      return this.procedures.map(p => p.name)
    },

    doctorsNamesList() {
      let procedure = this.procedures.find(p => p.name == this.procedureName)

      if (procedure)
        return procedure.doctors
      else
        return []
    },

    datesList() {
      return this.appointments.filter(a => a.doctor.name === this.doctorName).map(a => a.date)
    },

    appointmentsList() {
      return this.appointments.filter(a => a.date === this.date && a.doctor.name === this.doctorName)
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
      this.procedureName = event
      this.doctorName = ''
      this.date = ''
      this.currentAppointment = {time: ''}
    },

    doctorChanged(event) {
      this.doctorName = event
      this.date = ''
      this.currentAppointment = {time: ''}
    },

    dateChanged(event) {
      this.date = event
      this.currentAppointment = {time: ''}
    },

    timeChanged(event) {
      this.currentAppointment = this.appointmentsList.find(a => a.time === event)
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
