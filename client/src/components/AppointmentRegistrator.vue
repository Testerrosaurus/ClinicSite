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
    <select :value="time" @change="timeChanged($event)">
      <option v-for="time in timesList" :key="time" :value="time">
        {{time}}
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
      time: '',

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
        return [''].concat(doctor.dateTimes.filter(dt => dt.date === this.date).map(dt => dt.time))
      }

      return ['']
    }
  },

  created(){
    api.getDb()
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
      this.time = ''
    },

    timeChanged(event) {
      this.time = event.target.value
    },

    btn1ClickHandler() {
      let info = {
        patient: this.patientName,
        procedure: this.procedureName,
        doctor: this.doctorName,
        date: this.date,
        time: this.time
      }

      api.setAppointment(info).then(answer => {
        if (answer === 'Created') {
          this.$router.push('/AppointmentRegistered')
        }
        else if (answer === 'Invalid info') {
          alert('Invalid information')
        }
        else if (answer === 'Already booked') {
          api.getDb()
          .then(db => {
            this.doctors = db
            alert('Time was already booked')
          })
        }
      })
    }
  }
}
</script>
