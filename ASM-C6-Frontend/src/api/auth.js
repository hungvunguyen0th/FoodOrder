// import api from './index'
//
// export const login = (data) => api.post('/auth/login', data)
// export const register = (data) => api.post('/auth/register', data)
// export const getCurrentUser = () => api.get('/auth/me')
// export const changePassword = (data) => api.post('/auth/change-password', data)
// export const logout = () => api.post('/auth/logout')


//Compilot ở dưới
// src/api/auth.js
import api from './index'

// AUTH
export const register = (data) => api.post('/auth/register', data)
export const login = (data) => api.post('/auth/login', data)
export const refreshToken = (refreshToken) => api.post('/auth/refresh-token', { refreshToken })
export const changePassword = (data) => api.post('/auth/change-password', data)