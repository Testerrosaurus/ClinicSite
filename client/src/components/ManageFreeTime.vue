<template>
  <div class="blue-form">
    <br />
    <b-container>
      <b-row class="my-1">
        <b-col cols="3">
          <label for="doctor">Врач:</label>
        </b-col>
        <b-col cols="9">
          <b-form-select id="doctor" :value="currentDoctorName" @change="doctorChanged($event)">
            <option value="">
              Все
            </option>
            <option v-for="doctor in doctors" :key="doctor.name" :value="doctor.name">
              {{doctor.name}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
      <b-row class="my-1">
        <b-col cols="12">
          <b-button size="sm" @click="addHandler" class="mr-2">Добавить новое</b-button>
        </b-col>
      </b-row>
    </b-container>

    <b-table :items="filteredFreeTimes" :fields="fields" sort-by="date">
      <template slot="actions" slot-scope="row">
        <b-button size="sm" @click="editHandler(row.item)" class="mr-2">Изменить</b-button>
        <b-button size="sm" @click="removeHandler(row.item)">Удалить</b-button>
      </template>
    </b-table>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'ManageFreeTime',

  data() {
    return {
      freeTimes: [],
      doctors: [],
      currentDoctorName: '',

      fields: [
          { key: 'doctor', label: 'Doctor', sortable: true, sortDirection: 'desc' },
          { key: 'date', label: 'Date', sortable: true, sortDirection: 'desc' },
          { key: 'start', label: 'Start', sortable: true, sortDirection: 'desc' },
          { key: 'end', label: 'End' },
          { key: 'actions', label: 'Actions' }
        ],
    }
  },

  computed: {
    filteredFreeTimes() {
      return this.freeTimes.filter(a => (this.currentDoctorName === '' || this.currentDoctorName === a.doctor))
    }
  },
  
  created(){
    api.getFreeTimes()
    .then(db => {
      this.freeTimes = db.freeTimes
      this.doctors = db.doctors
      console.log(db)
    })
  },

  methods: {
    doctorChanged(event) {
      this.currentDoctorName = event
    },

    addHandler(freeTime) {
      this.$router.push("/FreeTimePage")
    },

    editHandler(freeTime) {
      this.$router.push({name: "FreeTimePage", params: {id: String(freeTime.id)}})
    },

    removeHandler(freeTime) {
      let info = {
        id: freeTime.id,
        rowVersion: freeTime.rowVersion
      }

      api.removeFreeTime(info)
      .then(response => {
        if (response === 'Removed') {
          this.freeTimes.splice(this.freeTimes.findIndex(a => a.id === freeTime.id), 1)
        } else if (response === 'Fail') {
          alert('Fail: Item was modified since last page load')
        }
      })
    }
  }
}
</script>
