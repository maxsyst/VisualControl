<template>
    <v-container>
        <v-row  v-for="digitMeasurement in digitMeasurementDictionary"
                :key="digitMeasurement.digitMeasurementName">
            <v-col lg="8" offset-lg="1">
               <StageRow :waferId="waferId" :digitMeasurement="digitMeasurement"></StageRow>
            </v-col>
            <v-col lg="3">
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
const StageRow = () => import('./StageRow.vue');
export default {
  data() {
    return {
      waferId: '',
      waferMap: {},
      initialArray: [],
      digitMeasurementDictionary: [],
    };
  },

  components: {
    StageRow,
  },

  async mounted() {
    this.waferId = this.$route.params.waferId;
    this.waferMap = (await this.$http.get(`/api/wafermap/getformedwafermap?waferMapFieldViewModelJSON=${JSON.stringify({
      waferId: this.waferId, fieldHeight: 200, fieldWidth: 200, streetSize: 2,
    })}`)).data;
    await this.$http.get(`/api/measurementrecording/wafer/${this.waferId}/dietype/all`)
      .then((response) => {
        if (response.status === 200) {
          this.initialArray = response.data.reduce((p, c) => [...p, ...c.measurementRecordingList.map((mr) => ({
            key: mr.name.split('.')[1].split('_')[0],
            trueElement: mr.element,
            virtualElement: mr.name.split('_').slice(1).reduce((vp, vc) => `${vp}_${vc}`, '').slice(1),
            idmr: mr.id,
            stage: { id: c.id, name: c.name },
          }))], []);
          const initialArray = [...this.initialArray];
          this.digitMeasurementDictionary = [...new Set(initialArray.map((x) => x.key).filter((f) => Number.isInteger(+f.charAt(0)))
            .sort((a, b) => +a - +b))]
            .map((digitName) => {
              const digitMeasurementArray = initialArray.filter((x) => x.key === digitName);
              return {
                digitMeasurementName: digitName,
                stageArray: [...new Map(digitMeasurementArray.map((x) => x.stage).map((item) => [item.id, item])).values()],
                elementMeasurementArray: digitMeasurementArray.map((x, index) => ({
                  trueElement: x.trueElement,
                  virtualElement: x.virtualElement.length > 0 ? x.virtualElement : `EL_${index + 1}`,
                  idmr: x.idmr,
                  statDirtyCellsPercentage: 0.0,
                })),
              };
            });
        }
        if (response.status === 204) {
          this.showSnackbar('Ошибка при загрузке информации');
        }
      })
      .catch((error) => {
        this.showSnackbar(error);
      });
  },

  methods: {
    showSnackbar(text) {
      this.$store.dispatch('alert/success', text);
    },
  },

  computed: {

  },
};
</script>
