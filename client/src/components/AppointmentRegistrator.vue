<template>
  <div id="appointment-registrator">
    Patient Name:
    <input v-model="patientName" />

    <br />
    Procedure:
    <select :value="procedureName" @change="procedureChanged($event)">
      <option v-for="procedureName in proceduresList" :key="procedureName" :value="procedureName">
        {{procedureName}}
      </option>
    </select>

    <br />
    Doctor:
    <select :value="doctorName" @change="doctorChanged($event)">
      <option v-for="doctor in doctorsList" :key="doctor.name" :value="doctor.name">
        {{doctor.name}}
      </option>
    </select>

    <br />
    Date:
    <select :value="date" @change="dateChanged($event)">
      <option v-for="date in datesList" :key="date" :value="date">
        {{date}}
      </option>
    </select>

    <br />
    Time:
    <select :value="currentDt.time" @change="timeChanged($event)">
      <option v-for="dt in timesList" :key="dt.id" :value="dt.time">
        {{dt.time}}
      </option>
    </select>

    <br />
    <button @click="btn1ClickHandler">Register</button>
  </div>
</template>

<script>
import api from '../api/api.js'

let proceduresNames = [
  'procedure1',
  'procedure2',
  'procedure3'
]

export default {
  name: 'AppointmentRegistrator',

  data() {
    return {
      patientName: '',

      procedureName: '',
      doctorName: '',
      date: '',
      currentDt: {time: ''},

      doctors: [],
      proceduresNames: proceduresNames
    }
  },

  computed: {
    proceduresList() { 
      return [''].concat(this.proceduresNames)
    },

    doctorsList() {
      let doctorsDoingChosenProcedure = this.doctors.filter(d => d.procedures.find(p => p.name === this.procedureName))

      return [''].concat(doctorsDoingChosenProcedure)
    },

    datesList() {
      let doctor = this.doctors.find(d => d.name === this.doctorName)

      if (doctor)
        return [''].concat([... new Set(doctor.dateTimes.map(dt => dt.date))])
      else
        return ['']
    },

    timesList() {
      let doctor = this.doctors.find(d => d.name === this.doctorName)

      if (doctor) {
        return [{time: ''}].concat(doctor.dateTimes.filter(dt => dt.date === this.date))
      }

      return ['']
    }
  },

  created(){
    api.getFilteredDb()
    .then(db => this.doctors = db)
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
      this.currentDt = {time: ''}
    },

    timeChanged(event) {
      this.currentDt = this.timesList.find(dt => dt.time === event.target.value)
    },

    btn1ClickHandler() {
      let info = {
        patient: this.patientName,
        procedure: this.procedureName,
        id: this.currentDt.id,
        rowVersion: this.currentDt.rowVersion
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
