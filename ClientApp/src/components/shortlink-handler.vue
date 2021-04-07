<template>

</template>

<script>
export default {
  data() {
    return {

    };
  },

  async beforeRouteEnter(to, from, next) {
    next(async (vm) => {
      try {
        const shortLinkVm = (await vm.$http.get(`/api/shortlink/guid/${to.params.guid}`)).data;
        vm.$router.push({
          name: 'wafermeasurement-shortlink',
          params: {
            shortLinkVm, guid: shortLinkVm.generatedId, waferId: shortLinkVm.waferId, measurementName: shortLinkVm.measurementRecording.name.split('.')[1], guid: shortLinkVm.generatedId,
          },
        });
      } catch (err) {
        vm.showSnackbar('Ссылка не найдена в БД');
        vm.$router.push({ name: 'pwafer' });
      }
    });
  },

  methods: {
    showSnackbar(text) {
      this.$store.dispatch('alert/error', text);
    },
  },
};
</script>
