<template>
  <div class="test">
    Summary:
    <input v-model="summary" />

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

let doctors = [
  {
    name: 'doctor1',
    procedures: [
      {
        name: 'procedure1',
      },
      {
        name: 'procedure2',
      }
    ],
    dateTimes: [
      {
        date: '2019-04-26',
        times: ['13:00', '14:00']
      },
      {
        date: '2019-04-27',
        times: ['10:00', '11:00']
      }
    ]
  },
  {
    name: 'doctor2',
    procedures: [
      {
        name: 'procedure1',
      },
      {
        name: 'procedure3',
      }
    ],
    dateTimes: [
      {
        date: '2019-04-26',
        times: ['9:30', '10:30']
      },
      {
        date: '2019-04-27',
        times: ['9:30', '12:30']
      },
      {
        date: '2019-04-28',
        times: ['8:00', '11:00']
      }
    ]
  }
]

export default {
  name: 'AppointmentRegistrator',

  data() {
    return {
      summary: '',

      procedureName: '',
      doctorName: '',
      date: '',
      time: '',

      doctors: doctors,
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
        return [''].concat(doctor.dateTimes.map(dt => dt.date))
      else
        return ['']
    },

    timesList() {
      let doctor = this.doctors.find(d => d.name === this.doctorName)

      if (doctor) {
        let date = doctor.dateTimes.find(dt => dt.date === this.date)
        if (date) return [''].concat(doctor.dateTimes.find(dt => dt.date === this.date).times)
      }

      return ['']
    }
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
        summary: this.summary,
        date: this.date,
        time: this.time
      }

      api.setAppointment(info).then(answer => console.log(answer))
    }
  }
}
</script>
