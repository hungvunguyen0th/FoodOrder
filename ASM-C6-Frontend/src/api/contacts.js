// import api from './index'
//
// // READ
// export const getContacts = (params) => api.get('/contacts', { params })
// export const getContactById = (id) => api.get(`/contacts/${id}`)
//
// // CREATE
// export const createContact = (data) => api.post('/contacts', data)
//
// // UPDATE
// export const updateContactStatus = (id, status) => api.put(`/contacts/${id}/status`, { status })
// export const replyContact = (id, reply) => api.post(`/contacts/${id}/reply`, { reply })
//
// // DELETE
// export const deleteContact = (id) => api.delete(`/contacts/${id}`)

//Compilot ở dưới
// src/api/contacts.js
import api from './index'

// READ
export const getContacts = (params) => api.get('/contacts', { params })
export const getContactById = (id) => api.get(`/contacts/${id}`)

// CREATE
export const createContact = (data) => api.post('/contacts', data)

// UPDATE
export const markAsRead = (id) => api.put(`/contacts/${id}/read`)
export const respondContact = (id, response) => api.put(`/contacts/${id}/respond`, { response })

// DELETE
export const deleteContact = (id) => api.delete(`/contacts/${id}`)