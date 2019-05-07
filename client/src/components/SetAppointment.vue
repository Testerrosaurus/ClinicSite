<template>
  <div class="blue-form">
    <br />
    <b-container>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="patient">Name:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="patient" v-model="patientName" type="text"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="phone">Phone:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="phone" v-model="patientPhone" type="tel"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="doctor">Doctor:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="doctor" :value="doctorName" @change="doctorChanged($event)">
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
            <option v-for="date in datesList" :key="date" :value="date">
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
          <b-form-select id="time" :disabled="date === ''" :value="time" @change="timeChanged($event)">
            <option value="" disabled="true">
              Select
            </option>
            <option v-for="timeObj in timeObjectsList" :key="timeObj.time" :value="timeObj.time">
              {{timeObj.time}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
      <b-row class="my-4">
        <b-col cols="12">
          <b-button variant="success" @click="btn1ClickHandler">Register</b-button>
        </b-col>
      </b-row>
      <b-modal id="modal" size= "sm" centered title="Title text" no-close-on-backdrop no-close-on-esc ok-only>
        <p class="my-4">Обработка информации Обработка информации Обработка информации Обработка информации Обработка информации Обработка информации Обработка информации</p>
      </b-modal>
    </b-container>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'SetAppointment',

  data() {
    return {
      patientName: '',
      patientPhone: '',

      doctorName: '',
      date: '',
      time: '',

      dateTimes: {}
    }
  },

  computed: {
    doctorsNamesList() {
      return Object.keys(this.dateTimes)
    },

    datesList() {
      if (this.doctorName === '') return []

      return [...new Set(this.dateTimes[this.doctorName].map(dt => dt.date))]
    },

    timeObjectsList() {
      if (this.doctorName === '') return []

      return this.dateTimes[this.doctorName].filter(dt => dt.date === this.date).map(dt => {
        return {
          time: dt.time,
          label: dt.time + ' - ' + dt.endTime
        }
      })
    }
  },

  created() {
    api.getAvailableDateTimes()
    .then(dateTimes => {
      this.dateTimes = dateTimes
      console.log(dateTimes)
    })
  },

  mounted() {
    this.$bvModal.show('modal')
  },

  methods: {
    doctorChanged(event) {
      this.doctorName = event
      this.date = ''
      this.time = ''
    },

    dateChanged(event) {
      this.date = event
      this.time = ''
    },

    timeChanged(event) {
      this.time = event
    },

    btn1ClickHandler() {
      let info = {
        patient: this.patientName,
        phone: this.patientPhone,
        doctor: this.doctorName,
        date: this.date,
        time: this.time
      }

      api.setAppointment(info).then(response => {
        if (response === 'Created') {
          this.$router.push('/AppointmentRegistered')
        }
        else if (response === 'Invalid info') {
          alert('Invalid information')
        }
        else if (response === 'Info changed') {
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
