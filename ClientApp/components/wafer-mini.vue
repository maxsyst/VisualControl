<template>
    <v-container fluid grid-list-lg>
        <svg :style="svgRotation" :height="fieldHeight" :width="fieldWidth" :viewBox="fieldViewBox">      
            <g v-for="(die, key) in dies" :key="die.id">
                <rect :dieIndex="key" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill" @contextmenu="showmenu" />
            </g>
        </svg>
    </v-container>
</template>

<script>
  import Loading from 'vue-loading-overlay';
  export default {
    props: ['waferId', 'avbSelectedDies', 'dirtyCells', 'streetSize', 'fieldHeight', 'fieldWidth'],
    components: { Loading },
    data() {
      return {
        dies: [],
        isloading: false,
        activeBtn: 1,
        showNav: false,
        x: 0,
        y: 0,
        initialOrientation: -1,
        currentOrientation: -1,
        fieldViewBox: ""
      }
    },

    methods:
    {      
      
    },
    
    watch:
    {
      waferId: {
        immediate: true,
        handler(newVal, oldVal) {
          this.isloading = true;
          let fieldObject = {waferId: this.waferId, fieldHeight: this.fieldHeight, fieldWidth: this.fieldWidth, streetSize: this.streetSize};
          this.$http({
            method: "post",
            url: `/api/wafermap/getformedwafermap`, data: fieldObject, config: {
              headers: {
                'Accept': "application/json",
                'Content-Type': "application/json"
              }
            }
          })
            .then((response) => {
              if (response.status === 200) {
                this.dies = JSON.parse(response.data.waferMapFormed);               
                this.initialOrientation = +response.data.orientation;
                this.currentOrientation = this.initialOrientation;
                this.isloading = false;
                this.showNav = false;
                this.dies.forEach(function(cell) {
                    cell.fill = "#A1887F";
                    cell.isActive = false;
                });
              }
            })
            .catch((error) => {
              if (error.response.status === 400) {
                this.dies = [];
                this.isloading = false;
              }
            });
        }
      },

      fieldWidth: {
        immediate: true,
        handler(newVal, oldVal) {
          this.fieldViewBox = `0 0 ${this.fieldHeight} ${this.fieldWidth}`;
        }
      },


      selectedDies: function() {
        if (this.avbSelectedDies.length > 0 && this.selectedDies.length > 0)
        {

          for (var i = 0; i < this.avbSelectedDies.length; i++) {
            let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
            if(this.dirtyCells.includes(die.id)) {
                die.fill = "#E91E63";
            }
            else {
                die.fill = "#4CAF50";
            }
           
            die.isActive = true;
          }  
       
        //   for (var i = 0; i < this.selectedDies.length; i++) {            
        //     this.dies.find(d => d.id === this.selectedDies[i]).fill = "#3D5AFE";           
        //   }           
         
        }

       }
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },
       svgRotation() {
         return {
           transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`         
        }
      }
    }
  }
</script>
