// import api from './index'
//
// export const getCategories = () => api.get('/categories')
// export const getCategoryById = (id) => api.get(`/categories/${id}`)
// export const createCategory = (data) => api.post('/categories', data)
// export const updateCategory = (id, data) => api.put(`/categories/${id}`, data)
// export const deleteCategory = (id) => api.delete(`/categories/${id}`)


//Compilot ở dưới

// src/api/categories.js
import api from './index'

// READ
export const getCategories = () => api.get('/categories')
export const getCategoryById = (id) => api.get(`/categories/${id}`)

// CREATE
export const createCategory = (data) => api.post('/categories', data)

// UPDATE
export const updateCategory = (id, data) => api.put(`/categories/${id}`, data)

// DELETE
export const deleteCategory = (id) => api.delete(`/categories/${id}`)