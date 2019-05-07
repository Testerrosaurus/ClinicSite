<template>
  <div class="blue-form">
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
      currentDoctor: 'doctor1',


      fields: [
          { key: 'date', label: 'Дата' },
          { key: 'start', label: 'Начало' },
          { key: 'end', label: 'Конец' },
          { key: 'status', label: 'Статус' },
        ],

      items: [
        { date: '2019-05-17', start: '15:00', end: '15:30', status: 'Свободно'},
        { date: '2019-05-17', start: '15:30', end: '17:00', status: 'Иван Иванович Иванов'},
        { date: '2019-05-17', start: '18:00', end: '19:00', status: 'Свободно'},
        { date: '2019-05-17', start: '19:00', end: '20:00', status: 'Петр Петрович Петров'},
        { date: '2019-05-17', start: '20:00', end: '21:00', status: 'Свободно'},

        { date: '', start: '', end: '', status: '', _rowVariant: 'success'},

        { date: '2019-05-18', start: '15:00', end: '18:30', status: 'Свободно'},
        { date: '2019-05-18', start: '18:30', end: '19:00', status: 'Иван Иванович Иванов'},
        { date: '2019-05-18', start: '19:00', end: '21:00', status: 'Свободно'},

        { date: '', start: '', end: '', status: '', _rowVariant: 'success'},
        
        { date: '2019-05-19', start: '15:00', end: '16:00', status: 'Свободно'},
        { date: '2019-05-19', start: '16:00', end: '16:30', status: 'Иван Иванович Иванов'},
        { date: '2019-05-19', start: '16:30', end: '17:00', status: 'Петр Петрович Петров'},
        { date: '2019-05-19', start: '17:00', end: '19:30', status: 'Свободно'},
        { date: '2019-05-19', start: '19:30', end: '20:30', status: 'Иван Иванович Иванов'},
        { date: '2019-05-19', start: '20:30', end: '21:00', status: 'Свободно'},

        { date: '', start: '', end: '', status: '', _rowVariant: 'success'},

        { date: '2019-05-20', start: '15:00', end: '15:30', status: 'Свободно'},
        { date: '2019-05-20', start: '15:30', end: '17:00', status: 'Иван Иванович Иванов'},
        { date: '2019-05-20', start: '18:00', end: '19:00', status: 'Свободно'},
        { date: '2019-05-20', start: '19:00', end: '20:00', status: 'Петр Петрович Петров'},
        { date: '2019-05-20', start: '20:00', end: '21:00', status: 'Свободно'},

        { date: '', start: '', end: '', status: '', _rowVariant: 'success'},

        { date: '2019-05-21', start: '15:00', end: '17:00', status: 'Свободно'},
        { date: '2019-05-21', start: '17:00', end: '17:30', status: 'Петр Петрович Петров'},
        { date: '2019-05-21', start: '17:30', end: '18:30', status: 'Свободно'},
        { date: '2019-05-21', start: '18:30', end: '19:00', status: 'Иван Иванович Иванов'},
        { date: '2019-05-21', start: '19:00', end: '21:00', status: 'Свободно'},

        { date: '', start: '', end: '', status: '', _rowVariant: 'success'},
        
        { date: '2019-05-22', start: '15:00', end: '16:00', status: 'Свободно'},
        { date: '2019-05-22', start: '16:00', end: '16:30', status: 'Иван Иванович Иванов'},
        { date: '2019-05-22', start: '16:30', end: '17:00', status: 'Петр Петрович Петров'},
        { date: '2019-05-22', start: '17:00', end: '19:30', status: 'Свободно'},
        { date: '2019-05-22', start: '19:30', end: '20:30', status: 'Иван Иванович Иванов'},
        { date: '2019-05-22', start: '20:30', end: '21:00', status: 'Свободно'},
      ]
    }
  },

  computed: {
    appointments() {
      if (!this.db.appointments) return []

      let aps = this.db.appointments.filter(d => d.doctor === this.currentDoctor).map(a => {
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

      let fts = this.db.freeTimes.filter(d => d.doctor === this.currentDoctor).map(t => {
        return {
          date: t.date,
          start: t.start,
          end: t.end,
          status: 'Svobodno',
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
              status: 'Svobodno',
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
            status: 'Svobodno',
            startStamp: sStamp,
            endStamp: ft.endStamp
          })
        }
      })

      return res
    },

    timeRanges() {
      return this.appointments.concat(this.freeTimes).sort((a, b) => {
        return a.startStamp - b.startStamp
      })
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
    m1() {

    }
  }
}
</script>