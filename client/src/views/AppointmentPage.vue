<template>
  <div>
    <h2>Appointment registration:</h2>
    <AppointmentRegistrator />
  </div>
</template>

<script>
// @ is an alias to /src
import AppointmentRegistrator from '@/components/AppointmentRegistrator.vue'

export default {
  name: 'AppointmentPage',
  components: {
    AppointmentRegistrator
  }
}
</script>
