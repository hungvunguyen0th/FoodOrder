// import api from './index'
//
// // READ
// export const getCombos = (params) => api.get('/combos', { params })
// export const getComboById = (id) => api.get(`/combos/${id}`)
//
// // CREATE
// export const createCombo = (data) => api.post('/combos', data)
//
// // UPDATE
// export const updateCombo = (id, data) => api.put(`/combos/${id}`, data)
//
// // DELETE
// export const deleteCombo = (id) => api.delete(`/combos/${id}`)

//Compilot ở dưới
// src/api/combos.js
import api from './index'

// READ
export const getCombos = (params) => api.get('/combos', { params })
export const getComboById = (id) => api.get(`/combos/${id}`)

// CREATE
export const createCombo = (data) => api.post('/combos', data)

// UPDATE
export const updateCombo = (id, data) => api.put(`/combos/${id}`, data)

// DELETE
export const deleteCombo = (id) => api.delete(`/combos/${id}`)