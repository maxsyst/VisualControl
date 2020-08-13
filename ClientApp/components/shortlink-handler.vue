<template>
  
</template>

<script>
export default {
    data() {
        return {

        }
    },

    async beforeRouteEnter(to, from, next) {
        next(async vm => {
            let shortLinkVm = (await vm.$http.get(`/api/shortlink/guid/${to.params.guid}`)).data
            vm.$router.push({ name: 'wafermeasurement-shortlink', 
                              params: {shortLinkVm: shortLinkVm, guid: shortLinkVm.generatedId, waferId: shortLinkVm.waferId, measurementName: shortLinkVm.measurementRecording.name.split('.')[1], guid: shortLinkVm.generatedId}
                            })
        })      
    },
}
</script>