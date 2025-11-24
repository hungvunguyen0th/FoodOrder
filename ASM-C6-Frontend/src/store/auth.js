import { defineStore } from 'pinia'
import { login as apiLogin, register as apiRegister, getCurrentUser } from '../api/auth'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: null,
        token: localStorage.getItem('accessToken'),
        loading: false,
        error: null
    }),

    getters: {
        isAuthenticated: (state) => !!state.token,
        userRole: (state) => state.user?.roles?.[0] || 'Guest',
        isAdmin: (state) => ['Admin', 'SuperAdmin', 'QuanLyDoAn'].includes(state.user?.roles?.[0]),
        isStaff: (state) => ['NhanVien', 'QuanLyDoAn'].includes(state.user?.roles?.[0]),
        isSuperAdmin: (state) => state.user?.roles?.[0] === 'SuperAdmin'
    },

    actions: {
        async login(email, password) {
            this.loading = true
            this.error = null
            try {
                const res = await apiLogin({ email, password })
                this.token = res.data.data.token
                this.user = res.data.data.user
                localStorage.setItem('accessToken', this.token)
                localStorage.setItem('userRole', this.user.roles[0])
                return { success: true }
            } catch (err) {
                this.error = err.response?.data?.message || 'Đăng nhập thất bại'
                return { success: false, message: this.error }
            } finally {
                this.loading = false
            }
        },

        async register(data) {
            this.loading = true
            this.error = null
            try {
                await apiRegister(data)
                return { success: true }
            } catch (err) {
                this.error = err.response?.data?.message || 'Đăng ký thất bại'
                return { success: false, message: this.error }
            } finally {
                this.loading = false
            }
        },

        async fetchUser() {
            if (!this.token) return
            try {
                const res = await getCurrentUser()
                this.user = res.data
            } catch (err) {
                this.logout()
            }
        },

        logout() {
            this.user = null
            this.token = null
            localStorage.removeItem('accessToken')
            localStorage.removeItem('userRole')
        }
    }
})
