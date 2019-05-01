import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/DbManagerPage',
      name: 'DbManagerPage',
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import(/* webpackChunkName: "about" */ './views/DbManagerPage.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/AppointmentPage',
      name: 'AppointmentPage',
      component: () => import('./views/AppointmentPage.vue')
    },
    {
      path: '/AppointmentRegistered',
      name: 'AppointmentRegistered',
      component: () => import('./views/AppointmentRegistered.vue')
    },
    {
      path: '/RegisterAccount',
      name: 'RegisterAccount',
      component: () => import('./components/RegisterAccount.vue'),
      meta: {
        requiresVisitor: true
      }
    },
    {
      path: '/LoginForm',
      name: 'LoginForm',
      component: () => import('./components/LoginForm.vue'),
      meta: {
        requiresVisitor: true
      }
    }
  ]
})
