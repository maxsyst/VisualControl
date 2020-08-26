<template>
    <v-container>

    </v-container>   
</template>

<script>
export default {
    data() {
        return {
            waferId: "",
            initialArray: [],
            digitMeasurementArray: []
        }
    },

    async mounted() {
        this.waferId = this.$route.params.waferId
        await this.$http.get(`/api/measurementrecording/wafer/${this.waferId}/dietype/0`)
            .then(response => {
                if(response.status === 200) {
                    this.initialArray = response.data
                }
                if(response.status === 204) {
                    this.initialArray = []
                }                            
            })
            .catch((error) => {
                this.showSnackbar(error)
            });
    },

    methods: {
        showSnackbar(text) {
            this.$store.dispatch("alert/success", text)
        }
    },

    computed: {

    }
}
</script>