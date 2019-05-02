<template>
  <div>
    Doctor:
    <select :value="currentDoctor.name" @change="doctorChanged($event)">
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
      <tr v-for="(dt, index) in dateTimes" :key="dt.id">
        <td>{{dt.date}}</td>
        <td>{{dt.time}}</td>
        <td>{{dt.status}}</td>
        <td><Button @click="confirmHandler(dt, index)">Confirm</Button></td>
        <td><Button @click="removeHandler(dt, index)">Remove</Button></td>
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
      doctors: [],
      currentDoctor: {name:''}
    }
  },

  computed: {
    dateTimes() {
      if (!this.currentDoctor.dateTimes) return []

      return this.currentDoctor.dateTimes
    }
  },
  
  created(){
    api.getDb()
    .then(db => this.doctors = db)
  },

  methods: {
    doctorChanged(event) {
      this.currentDoctor = [{name:''}].concat(this.doctors).find(d => d.name === event.target.value)
    },

    confirmHandler(dateTime, index) {
      let info = {
        id: dateTime.id,
        rowVersion: dateTime.rowVersion
      }

      api.confirmDt(info)
      .then(response => {
        if (response === 'Confirmed') {
          this.currentDoctor.dateTimes.find(dt => dt.id === dateTime.id).status = 'Confirmed'
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        }
      })
    },

    removeHandler(dateTime, index) {
      let info = {
        id: dateTime.id,
        rowVersion: dateTime.rowVersion
      }

      api.removeDt(info)
      .then(response => {
        if (response === 'Removed') {
          this.currentDoctor.dateTimes.splice(index, 1)
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        }
      })
    }
  }
}
</script>
