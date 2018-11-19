import HomePage from 'components/home-page'
import SelectBasic from 'components/select-basic'

export const routes = [
  { name: 'home', path: '/', component: HomePage, display: 'Начальный экран', icon: 'home' },
  { name: 'basic', path: '/select-basic', component: SelectBasic, display: 'Просмотр измерений', icon: 'file-medical-alt'}
];
