<template>
  <div>
    <h2>Database Manager</h2>
    <DbManager />
  </div>
</template>

<script>
// @ is an alias to /src
import DbManager from '@/components/DbManager.vue'

export default {
  name: 'DbManagerPage',
  components: {
    DbManager
  }
}
</script>
