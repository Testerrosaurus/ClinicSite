<template>
  <div class="blue-form">
    <br />
    <b-container>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="radios">Status:</label>
        </b-col>
        <b-col cols="9">
          <b-form-radio-group id="radios"
            v-model="selected"
            :options="options"
            name="radio-inline">
          </b-form-radio-group>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="doctor">Doctor:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="doctor" :value="currentDoctorName" @change="doctorChanged($event)">
            <option value="">
              All
            </option>
            <option v-for="doctor in doctors" :key="doctor.name" :value="doctor.name">
              {{doctor.name}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
    </b-container>

    <b-table :items="filteredAppointments" :fields="fields" sort-by="start">
      <template slot="actions" slot-scope="row">
        <b-button size="sm" @click="editHandler(row.item)" class="mr-2">Edit</b-button>
        <b-button size="sm" @click="confirmHandler(row.item)" class="mr-2"
          :disabled="row.item.status !== 'Unconfirmed'">Confirm</b-button>
        <b-button size="sm" @click="removeHandler(row.item)">Remove</b-button>
      </template>
    </b-table>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'ManageAppointments',

  data() {
    return {
      appointments: [],
      doctors: [],
      currentDoctorName: '',

      fields: [
          { key: 'doctor', label: 'Doctor' },
          { key: 'patient', label: 'Patient' },
          { key: 'start', label: 'Start', sortable: true, sortDirection: 'desc' },
          { key: 'duration', label: 'Duration' },
          { key: 'status', label: 'Status' },
          { key: 'created', label: 'Created', sortable: true, sortDirection: 'desc'},
          { key: 'actions', label: 'Actions' }
        ],

      selected: 'all',
      options: [
        { text: 'All', value: 'all' },
        { text: 'Confirmed', value: 'Confirmed' },
        { text: 'Unconfirmed', value: 'Unconfirmed' }
      ]
    }
  },

  computed: {
    filteredAppointments() {
      return this.appointments.filter(a => (this.currentDoctorName === '' || this.currentDoctorName === a.doctor) &&
        (this.selected === 'all' || this.selected === a.status))
    }
  },
  
  created(){
    api.getAppointments()
    .then(db => {
      this.appointments = db.appointments
      this.doctors = db.doctors
      console.log(db)
    })
  },

  methods: {
    doctorChanged(event) {
      this.currentDoctorName = event
    },

    editHandler(appointment) {
      this.$router.push({name: "EditPage", params: {id: String(appointment.id)}})
    },

    confirmHandler(appointment) {
      let info = {
        id: appointment.id,
        rowVersion: appointment.rowVersion,
        date: appointment.date,
        time: appointment.time,
        duration: appointment.duration
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
