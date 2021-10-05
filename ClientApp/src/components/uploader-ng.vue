<template>
  <v-container>
      <v-row>
          <v-col lg="2" offset-lg="1">
            <v-radio-group v-model="selectedCodeProductFolder" @change="clearMeasurementRecordings">
                <v-radio v-for="cpf in codeProductFolders" :key="cpf.folderName" :value="cpf.folderName" :label="cpf.folderName"></v-radio>
            </v-radio-group>
          </v-col>
          <v-col lg="2" offset-lg="1">
            <v-radio-group v-model="selectedWaferFolder" @change="selectedMeasurementRecordings = []">
                <v-radio v-for="wf in waferFolders" :key="wf.folderName" :value="wf.folderName" :label="wf.folderName" ></v-radio>
            </v-radio-group>
          </v-col>
          <v-col lg="2" offset-lg="1">
              <v-checkbox v-model="selectedMeasurementRecordings" v-for="mr in measurementRecordings" :key="mr" :label="mr" :value="mr"></v-checkbox>
          </v-col>
          <v-col lg="2">
              <v-btn v-show="selectedCodeProductFolder && selectedWaferFolder && selectedMeasurementRecordings.length > 0" 
                     color="success"
                     block
                     @click="goToUploading(selectedCodeProductFolder, selectedWaferFolder, selectedMeasurementRecordings)">Перейти к загрузке</v-btn>
          </v-col>
      </v-row>
  </v-container>
</template>

<script>

export default {
  data() {
    return {
      codeProductFolders: [],
      waferFolders: [],
      measurementRecordings: [],
      selectedCodeProductFolder: '',
      selectedWaferFolder: '',
      selectedMeasurementRecordings: [],
    };
  },

  methods: {
    async initCodeProductFolders() {
      await this.$http
        .get('/api/folder/folders-cp')
        .then((response) => this.codeProductFolders = response.data)
    },

    goToUploading(selectedCodeProductFolder, selectedWaferFolder, selectedMeasurementRecordings) {
      this.$router.push({
        name: 'uploader-final',
        params: {
          codeProduct: selectedCodeProductFolder,
          wafer: selectedWaferFolder,
          measurementRecordings: selectedMeasurementRecordings,
        },
      });
    },

    clearMeasurementRecordings() {
      this.measurementRecordings = [];
    },

    showSnackBar(text) {
      this.$store.dispatch('alert/success', text);
    },
  },

  watch: {

    async selectedCodeProductFolder(newVal, oldVal) {
      this.$router.push({ name: 'uploader-cp', params: { selectedCodeProductFolder: newVal } });
      await this.$http
        .get(`/api/folder/folders-wafer/${newVal}`)
        .then((response) => {
          this.waferFolders = response.data;
        })
        .catch((error) => {
          if (error.response.status === 500) {
            this.showSnackBar('Ошибка при поиске папки');
          }
        });
    },

    async selectedWaferFolder(newVal) {
      const codeProductFolderName = this.selectedCodeProductFolder;
      if (newVal) {
        this.$router.push({ name: 'uploader-cpw', params: { selectedCodeProductFolder: codeProductFolderName, selectedWaferFolder: newVal } });
        await this.$http
          .get(`/api/folder/folders-idmr/${codeProductFolderName}/${newVal}`)
          .then((response) => {
            this.measurementRecordings = response.data;
          })
          .catch((error) => {
            if (error.response.status === 500) {
              this.showSnackBar('Ошибка при поиске папки');
            }
          });
      }
    },

    selectedMeasurementRecordings(newVal) {
      const mrArray = newVal.join('$');
      const waferFolderName = this.selectedWaferFolder;
      const codeProductFolderName = this.selectedCodeProductFolder;
      if (mrArray.length === 0) {
        if (waferFolderName) {
          this.$router.push({ name: 'uploader-cpw', params: { selectedCodeProductFolder: codeProductFolderName, selectedWaferFolder: waferFolderName } });
        } else {
          this.$router.push({ name: 'uploader-cp', params: { selectedCodeProductFolder: codeProductFolderName } });
        }
      } else {
        this.$router.push({ name: 'uploader-cpwi', params: { selectedCodeProductFolder: codeProductFolderName, selectedWaferFolder: waferFolderName, mrArray } });
      }
    },
  },

  async mounted() {
    this.initCodeProductFolders();
    if (this.$route.name === 'uploader-cp') {
      this.selectedCodeProductFolder = this.$route.params.selectedCodeProductFolder;
    }
    if (this.$route.name === 'uploader-cpw') {
      this.selectedCodeProductFolder = this.$route.params.selectedCodeProductFolder;
      this.selectedWaferFolder = this.$route.params.selectedWaferFolder;
    }
    if (this.$route.name === 'uploader-cpwi') {
      this.selectedCodeProductFolder = this.$route.params.selectedCodeProductFolder;
      this.selectedWaferFolder = this.$route.params.selectedWaferFolder;
      this.selectedMeasurementRecordings = this.$route.params.mrArray.split('$');
    }
  },
};
</script>

<style>

</style>
