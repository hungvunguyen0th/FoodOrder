// import api from './index'
//
// // READ
// export const getToppings = () => api.get('/toppings')
// export const getToppingById = (id) => api.get(`/toppings/${id}`)
//
// // CREATE
// export const createTopping = (data) => api.post('/toppings', data)
//
// // UPDATE
// export const updateTopping = (id, data) => api.put(`/toppings/${id}`, data)
//
// // DELETE
// export const deleteTopping = (id) => api.delete(`/toppings/${id}`)

//Compilot ở dưới
// src/api/toppings.js
import api from './index'

// READ
export const getToppings = () => api.get('/toppings')
export const getToppingById = (id) => api.get(`/toppings/${id}`)

// CREATE
export const createTopping = (data) => api.post('/toppings', data)

// UPDATE
export const updateTopping = (id, data) => api.put(`/toppings/${id}`, data)

// DELETE
export const deleteTopping = (id) => api.delete(`/toppings/${id}`)