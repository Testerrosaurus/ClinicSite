<template>
  <div class="background-img">
    <h1>"Bla Bla Bla"</h1>
    
    <h3>Text Text Text</h3>
    <ul>
      <li><a href="https://google.com" target="_blank" rel="noopener">google</a></li>
      <li><a href="https://google.com" target="_blank" rel="noopener">google</a></li>
      <li><a href="https://google.com" target="_blank" rel="noopener">google</a></li>
    </ul>
  </div>
</template>

<script>
export default {
  name: 'HomePage'
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: lightyellow;
}
</style>
