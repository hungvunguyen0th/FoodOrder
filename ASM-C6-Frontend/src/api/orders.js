// import api from './index'
//
// // READ
// export const getOrders = (params) => api.get('/orders', { params })
// export const getOrderById = (id) => api.get(`/orders/${id}`)
//
// // CREATE
// export const createOrder = (data) => api.post('/orders', data)
//
// // UPDATE
// export const updateOrderStatus = (id, status) => api.put(`/orders/${id}/status`, { status })
//
// // CANCEL
// export const cancelOrder = (id) => api.post(`/orders/${id}/cancel`)
//
// // STATS
// // export const getOrderStatistics = () => api.get('/orders/statistics')

//Compilot ở dưới
// src/api/orders.js
import api from './index'

// READ
export const getOrders = (params) => api.get('/orders', { params })
export const getOrderById = (id) => api.get(`/orders/${id}`)
export const getOrderStatistics = () => api.get('/orders/statistics')

// CREATE
export const createOrder = (data) => api.post('/orders', data)

// UPDATE
export const updateOrderStatus = (id, status) => api.put(`/orders/${id}/status`, { status })

// DELETE/CANCEL
export const cancelOrder = (id) => api.post(`/orders/${id}/cancel`)