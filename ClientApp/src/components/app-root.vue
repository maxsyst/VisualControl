<template>
    <v-app dark>
      <v-navigation-drawer v-if="auth" fixed
                           :value="drawer"
                           app>
        <v-toolbar flat class="transparent">
          <v-list>
            <v-list-item>
              <v-list-item-avatar>
                <avatar :username="username"
                        :size="40">
                </avatar>
              </v-list-item-avatar>
              <v-list-item-content>
                <v-list-item-title>{{username}}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>

          </v-list>

        </v-toolbar>
        <v-divider class="pt-0"></v-divider>
        <v-list nav dense>
          <v-list-group>
            <template v-slot:activator>
              <v-list-item-content>
                <v-list-item-title>Загрузка измерений</v-list-item-title>
              </v-list-item-content>
            </template>

            <v-list-item
              v-for="r in routes.filter(x=>x.nav === true && x.uploadingArea === true)" :to="r.path"
              :key="r.path">
               <v-list-item-action>
                <v-icon>{{r.icon}}</v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-title v-text="r.display"></v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-group>
      </v-list>
        <v-list nav dense v-for="(route, index) in routes.filter(x=>x.nav === true && x.uploadingArea !== true)" :key="index">

          <v-list-item ripple :to="route.path">
            <v-list-item-action>
              <v-icon>{{route.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>{{route.display}}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>

        </v-list>
      </v-navigation-drawer>
      <v-app-bar v-if="auth" color="indigo" fixed app>
        <v-app-bar-nav-icon v-if="auth" @click.stop="changeDrawer(drawer)"><v-icon>drag_indicator</v-icon></v-app-bar-nav-icon>
        <v-toolbar-title>Система контроля за измерениями 2.0</v-toolbar-title>
        <v-spacer></v-spacer>
        <v-btn class="ma-2" color="indigo" to="/" dark fab small><v-icon>home</v-icon></v-btn>
        <v-btn to="/login" dark outlined>Выйти из системы </v-btn>
      </v-app-bar>
      <v-main>
        <v-container fluid>
          <v-row justify-start align-center>
           <router-view/>
          </v-row>
          <v-snackbar v-model="$store.state.alert.visible" top>
            {{ $store.state.alert.message }}
            <v-btn color="pink" timeout="1500" text @click="$store.state.alert.visible = false">Закрыть</v-btn>
        </v-snackbar>
        <v-dialog
            v-model="$store.state.loading.visible"
            hide-overlay
            persistent
            width="500">
          <v-card color="indigo" dark>
              <v-card-text>
                {{$store.state.loading.text}}
                <v-progress-linear
                  indeterminate
                  color="white"
                  class="mb-0">
                </v-progress-linear>
              </v-card-text>
          </v-card>
        </v-dialog>
        </v-container>
      </v-main>
    </v-app>
</template>

<script>
  import { routes } from '../router/routesArray'
  import Avatar from 'vue-avatar'
    export default {
    
     data () {
        return {
          routes: [...routes]
        }
      },

     components:
     {
        Avatar
     },

     methods: {

       changeDrawer(drawer) {
          this.$store.dispatch("service/changeDrawer", drawer)
       }

     },

     computed: {

       drawer() {
         return this.$store.getters['service/drawer']
       },

       auth() {
         if (this.$route.name === "login" || this.$route.name === "registration") {
           return false
         } else {
           this.$store.dispatch("service/changeDrawer", true)
           return true;
         }
       },

       username() {
         if (this.$store.state.authentication.user !== null) {
           return this.$store.state.authentication.user.firstName + " " + this.$store.state.authentication.user.surname
         } else {
           return ""
         }
           
       }
       
     },

     watch: {
       $route(to, from) {
          
       }
     }

     
    }
</script>

<style lang="scss">
  #app {
    font-family: Avenir, Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }
</style>
