<template>
  <v-data-table
    :headers="headers"
    :items-per-page="25"
    :footer-props="{
      itemsPerPageText: 'Измерений на странице',
      disableItemsPerPage: true
    }"
    :items="tableData"
    @click:row="goToSingleMeasurement"
  >
    <template #item.creationDate="{ item }">
      <span>{{ new Date(item.creationDate).toLocaleString("ru-RU", { year: 'numeric', month: 'long', day: 'numeric' }) }}</span>
    </template>
    <template #item.durationSeconds="{ item }">
      <span>{{ (moment.duration(item.durationSeconds, 'seconds').format("hh:mm:ss")) }}</span>
    </template>
    <template #item.durationPreSeconds="{ item }">
      <span>{{ (item.durationPreSeconds === 0 ? "00:00" : moment.duration(item.durationPreSeconds, 'seconds').format("hh:mm:ss")) }}</span>
    </template>
  </v-data-table>
</template>

<script>

export default {
  name: 'MeasurementTable',
  props: ['tableData'],
  data() {
    return {
      headers: [
        {
          text: 'Название', align: 'center', sortable: false, value: 'name', divider: true, width: '10%',
        },
        {
          text: 'Дата включения', sortable: false, align: 'center', value: 'creationDate', divider: true, width: '13.5%',
        },
        {
          text: 'Время включения', sortable: false, align: 'center', value: 'durationPreSeconds', divider: true, width: '7.5%',
        },
        {
          text: 'Продолжительность теста', sortable: false, align: 'center', value: 'durationSeconds', divider: true, width: '7.5%',
        },
        {
          text: 'Плата согласования', sortable: false, align: 'center', value: 'matchingBoard', divider: true, width: '5%',
        },
        {
          text: 'Согласование', sortable: false, align: 'center', value: 'matching', divider: true, width: '5%',
        },
        {
          text: 'Vgate, В', sortable: false, align: 'center', value: 'vgate', divider: true, width: '4%',
        },
        {
          text: 'Vpower, В', sortable: false, align: 'center', value: 'vpower', divider: true, width: '4%',
        },
        {
          text: 'Датчик температуры', sortable: false, align: 'center', value: 'temperatureSensor', divider: true, width: '5%',
        },
        {
          text: 'Измерительный канал', sortable: false, align: 'center', value: 'measurementChannel', divider: true, width: '5%',
        },
      ],
    };
  },
  methods: {
    async goToSingleMeasurement(payload) {
      await this.$router.push({ name: 'measurement-single', params: { measurementId: payload.id } });
    },
  },
};

</script>

<style scoped>

</style>
