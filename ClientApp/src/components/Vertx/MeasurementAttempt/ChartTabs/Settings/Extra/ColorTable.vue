<template>
  <v-simple-table>
    <template #default>
      <thead>
        <tr>
          <th class="text-left">
            Аттрибут
          </th>
          <th class="text-left">
            Цвет
          </th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="(item, name) in colorObject"
          :key="name"
        >
          <td>{{ name }}</td>
          <td>
            <v-swatches
              v-model="colorObject[name]"
              :value="item"
            />
          </td>
        </tr>
      </tbody>
    </template>
  </v-simple-table>
</template>
<script>
import VSwatches from 'vue-swatches';

export default {
  name: 'ColorTable',
  components: {
    VSwatches,
  },
  props: ['colorObject'],
  data() {
    return {
    };
  },
};
</script>

<style scoped>
::v-deep .vue-swatches__container {
  z-index: 9999;
  position: relative;
}
</style>
