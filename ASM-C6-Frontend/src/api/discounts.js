// import api from './index'
//
// // READ
// export const getDiscounts = (params) => api.get('/discounts', { params })
// export const getDiscountById = (id) => api.get(`/discounts/${id}`)
// export const getDiscountByCode = (code) => api.get(`/discounts/code/${code}`)
// export const validateDiscount = (code, orderAmount) => api.post('/discounts/validate', { code, orderAmount })
// export const getUserDiscounts = () => api.get('/discounts/user')
//
// // CREATE
// export const createDiscount = (data) => api.post('/discounts', data)
//
// // UPDATE
// export const updateDiscount = (id, data) => api.put(`/discounts/${id}`, data)
// export const assignDiscountToUser = (discountId, userId) => api.post('/discounts/assign', { discountId, userId })
//
// // DELETE
// export const deleteDiscount = (id) => api.delete(`/discounts/${id}`)

//Compilot ở dưới
// src/api/discounts.js
import api from './index'

// READ
export const getDiscounts = (params) => api.get('/discounts', { params })
export const getDiscountById = (id) => api.get(`/discounts/${id}`)
export const getDiscountByCode = (code) => api.get(`/discounts/code/${code}`)
export const getUserDiscounts = () => api.get('/discounts/user')

// CREATE
export const createDiscount = (data) => api.post('/discounts', data)

// UPDATE
export const updateDiscount = (id, data) => api.put(`/discounts/${id}`, data)

// DELETE
export const deleteDiscount = (id) => api.delete(`/discounts/${id}`)

// VALIDATE
export const validateDiscount = (code, orderAmount) =>
  api.post('/discounts/validate', { code, orderAmount })
