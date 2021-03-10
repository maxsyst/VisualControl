<template>
  <v-data-table
    :headers="headers"
    :items-per-page="10"
    :footer-props="{
      itemsPerPageText: 'Измерений на странице',
      disableItemsPerPage: true
    }"
    :items="tableData"
    @click:row="goToMeasurementAttempt"
  >
    <template #item.measurementAttemptId="{ item }">
      <span>{{ item.measurementAttemptId }}</span>
    </template>
    <template #item.code="{ item }">
      <span>{{ item.code }}</span>
    </template>
    <template #item.waferId="{ item }">
      <span>{{ item.waferId }}</span>
    </template>
    <template #item.measurementName="{ item }">
      <span>{{ item.measurementName }}</span>
    </template>
    <template #item.lastUpdateDate="{ item }">
      <span>{{ new Date(item.lastUpdateDate).toLocaleString("ru-RU", { year: 'numeric', month: 'long', day: 'numeric' }) }}</span>
    </template>
  </v-data-table>
</template>

<script>
export default {
  props: ['tableData'],
  data () {
    return {
      headers: [
        {
          text: 'Номер кристалла', align: 'center', sortable: false, value: 'code', divider: true, width: '20%'
        },
        {
          text: 'Номер пластины', sortable: false, align: 'center', value: 'waferId', divider: true, width: '20%'
        },
        {
          text: 'Последнее измерение', sortable: false, align: 'center', value: 'measurementName', divider: true, width: '20%'
        },
        {
          text: 'Время последнего измерения', sortable: false, align: 'center', value: 'lastUpdateDate', divider: true, width: '20%'
        },
        {
          text: 'measurementAttemptId', sortable: false, align: 'center', value: 'measurementAttemptId', divider: true, width: '10%'
        }
      ]
    }
  },
  methods: {
    async goToMeasurementAttempt (payload) {
      await this.$router.push({ name: 'measurementAttempt', params: { measurementAttemptId: payload.measurementAttemptId } })
    }
  }
}
</script>

<style scoped>

</style>
