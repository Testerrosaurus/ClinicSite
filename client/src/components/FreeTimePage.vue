<template>
  <div class="blue-form">
    <br />
    <b-container>
      <b-row class="my-1" v-if="freeTime.id == -1">
        <b-col cols="3">
          <label for="doctor">Doctor:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="doctor" :value="currentDoctorName" @change="doctorChanged($event)">
            <option value="" disabled="true">
              All
            </option>
            <option v-for="doctor in doctors" :key="doctor.name" :value="doctor.name">
              {{doctor.name}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>

      <b-row class="my-1">
        <b-col cols="3">
          <label for="date">Date:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="date" v-model="freeTime.date" type="date" :disabled="currentDoctorName === ''"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="start">Start time:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="start" v-model="freeTime.start" type="time" :disabled="currentDoctorName === ''"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="end">End time:</label>
        </b-col>
        <b-col cols="9">
          <b-form-input id="end" v-model="freeTime.end" type="time" :disabled="currentDoctorName === ''"></b-form-input>
        </b-col>
      </b-row>
      <b-row class="my-4">
        <b-col cols="12">
          <b-button variant="success" @click="addHandler">{{buttonName}}</b-button>
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
      freeTimes: [],
      doctors: [],

      freeTime: {},

      currentDoctorName: ''
    }
  },

  computed: {
    buttonName() {
      if (this.freeTime.id === -1) return 'Add'
      else return 'Save'
    }
  },
  
  created(){
    api.getFreeTimes()
    .then(db => {
      this.freeTimes = db.freeTimes
      this.doctors = db.doctors
      console.log(db)

      let ft = db.freeTimes.find(ft => ft.id === Number(this.$route.params.id))
      if (ft) {
        this.currentDoctorName = ft.doctor
      } else {
        ft = {id: -1}
      }

      this.freeTime = JSON.parse(JSON.stringify(ft))
    })
  },

  methods: {
    doctorChanged(event) {
      this.currentDoctorName = event
    },

    addHandler() {
      let info = {
        id: this.freeTime.id,
        rowVersion: this.freeTime.rowVersion,
        doctor: this.freeTime.id == -1 ? this.currentDoctorName : this.freeTime.doctor,
        date: this.freeTime.date,
        start: this.freeTime.start,
        end: this.freeTime.end
      }

      api.addFreeTime(info)
      .then(response => {
        if (response === 'Success') {
            this.$router.push('/ManageFreeTime')
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        }
      })
    }
  }
}
</script>