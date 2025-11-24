// import api from './index'
//
// // READ
// export const getUserAddresses = () => api.get('/addresses')
//
// // CREATE
// export const createAddress = (data) => api.post('/addresses', data)
//
// // UPDATE
// export const updateAddress = (id, data) => api.put(`/addresses/${id}`, data)
//
// // DELETE
// export const deleteAddress = (id) => api.delete(`/addresses/${id}`)

//Compilot ở dưới

// src/api/address.js
import api from './index'

// READ
export const getUserAddresses = () => api.get('/addresses')
export const getAddressById = (id) => api.get(`/addresses/${id}`)

// CREATE
export const createAddress = (data) => api.post('/addresses', data)

// UPDATE
export const updateAddress = (id, data) => api.put(`/addresses/${id}`, data)

// DELETE
export const deleteAddress = (id) => api.delete(`/addresses/${id}`)