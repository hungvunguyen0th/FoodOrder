import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/customer/HomeView.vue'
import MenuView from '../views/customer/MenuView.vue'
import CartView from '../views/customer/CartView.vue'

const routes = [
    {
        path: '/',
        name: 'Home',
        component: HomeView
    },
    {
        path: '/menu',
        name: 'Menu',
        component: MenuView
    },
    {
        path: '/cart',
        name: 'Cart',
        component: CartView
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router
