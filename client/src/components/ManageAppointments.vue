<template>
  <div class="blue-form">
    <b-container>
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

    <b-table :items="filteredAppointments" :fields="fields" sort-by="date">
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
          { key: 'date', label: 'Date', sortable: true, sortDirection: 'desc' },
          { key: 'time', label: 'Time' },
          { key: 'duration', label: 'Duration' },
          { key: 'status', label: 'Status' },
          { key: 'actions', label: 'Actions' }
        ],
    }
  },

  computed: {
    filteredAppointments() {
      if (this.currentDoctorName === '') return this.appointments

      return this.appointments.filter(a => a.doctor === this.currentDoctorName)
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
