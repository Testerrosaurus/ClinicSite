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
            <option value="" disabled="true">
              Все
            </option>
            <option v-for="doctor in db.doctors" :key="doctor.name" :value="doctor.name">
              {{doctor.name}}
            </option>
          </b-form-select>
        </b-col>
      </b-row>
    </b-container>

    <b-table striped :items="timeRanges" :fields="fields"></b-table>
  </div>
</template>

<script>
import api from '../api/api.js'

export default {
  name: 'CalendarPage',

  data() {
    return {
      db: {},
      currentDoctorName: '',


      fields: [
        { key: 'date', label: 'Дата' },
        { key: 'start', label: 'Начало' },
        { key: 'end', label: 'Конец' },
        { key: 'status', label: 'Статус' },
      ]  
    }
  },

  computed: {
    appointments() {
      if (!this.db.appointments) return []

      let aps = this.db.appointments.filter(d => d.doctor === this.currentDoctorName).map(a => {
        return {
          date: a.date,
          start: a.start,
          end: a.end,
          status: a.patient,
          startStamp: Number(new Date(a.date + 'T' + a.start)),
          endStamp: Number(new Date(a.date + 'T' + a.end))
        }
      })

      return aps.sort((a, b) => {
        return a.startStamp - b.startStamp
      })
    },

    freeTimes() {
      if (!this.db.freeTimes) return []

      let fts = this.db.freeTimes.filter(d => d.doctor === this.currentDoctorName).map(t => {
        return {
          date: t.date,
          start: t.start,
          end: t.end,
          startStamp: Number(new Date(t.date + 'T' + t.start)),
          endStamp: Number(new Date(t.date + 'T' + t.end))
        }
      }).sort((a, b) => {
        return a.startStamp - b.startStamp
      })


      let res = []

      fts.forEach(ft => {
        let sStamp = ft.startStamp
        let s = ft.start

        this.appointments.forEach(a => {
          if (a.endStamp <= ft.startStamp || a.startStamp >= ft.endStamp) {
            return // continue
          }

          if (sStamp + 5 * 60 * 1000 < a.startStamp) {
            res.push({
              date: ft.date,
              start: s,
              end: a.start,
              status: 'Свободно',
              startStamp: sStamp,
              endStamp: a.startStamp
            })
          }

          sStamp = a.endStamp
          s = a.end
        })

        if (sStamp + 5 * 60 * 1000 < ft.endStamp) {
          res.push({
            date: ft.date,
            start: s,
            end: ft.end,
            status: 'Свободно',
            startStamp: sStamp,
            endStamp: ft.endStamp
          })
        }
      })

      return res
    },

    timeRanges() {
      let res =  this.appointments.concat(this.freeTimes).sort((a, b) => {
        return a.startStamp - b.startStamp
      })
      
      if (res.length === 0) return []


      let daySeparators = []
      let lastDate = res[0].date
      res.forEach(t => {
        if (t.date !== lastDate) {
          daySeparators.push({
            date: '',
            start: '',
            end: '',
            status: '',
            startStamp: Number(new Date(t.date + 'T' + '04:00')),
            endStamp: Number(new Date(t.date + 'T' + '04:00')),
            _rowVariant: 'success'
          })

          lastDate = t.date
        }
      })


      res = res.concat(daySeparators).sort((a, b) => {
        return a.startStamp - b.startStamp
      })

      return res
    }
  },
  
  created(){
    api.getDb()
    .then(db => {
      this.db = db
      console.log(db)
    })
  },

  methods: {
    doctorChanged(event) {
      this.currentDoctorName = event
    },
  }
}
</script>