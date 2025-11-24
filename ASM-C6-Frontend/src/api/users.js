import api from './index'

// READ
export const getUsers = (params) => api.get('/users', { params })
export const getUserById = (id) => api.get(`/users/${id}`)
export const getUserRoles = (userId) => api.get(`/users/${userId}/roles`)

// CREATE
export const createUser = (data) => api.post('/users', data)

// UPDATE
export const updateUser = (id, data) => api.put(`/users/${id}`, data)
export const assignRole = (userId, role) => api.post(`/users/${userId}/roles/${role}`)
export const removeRole = (userId, role) => api.delete(`/users/${userId}/roles/${role}`)
export const deactivateUser = (id) => api.put(`/users/${id}/deactivate`)
export const activateUser = (id) => api.put(`/users/${id}/activate`)

// DELETE
export const deleteUser = (id) => api.delete(`/users/${id}`)
