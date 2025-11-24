// src/api/index.js
    import axios from 'axios'
    import { useAuthStore } from '../store/auth'

    const API_BASE_URL = 'http://localhost:5077/api'

    const apiClient = axios.create({
      baseURL: API_BASE_URL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json'
      }
    })

    // Interceptor: Thêm JWT token vào request headers
    apiClient.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('accessToken')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => {
        return Promise.reject(error)
      }
    )

    // Interceptor: Xử lý response & refresh token nếu hết hạn
    apiClient.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config
        const authStore = useAuthStore()

        // Nếu token hết hạn (401) và chưa retry
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true

          try {
            const refreshToken = localStorage.getItem('refreshToken')
            if (!refreshToken) {
              authStore.logout()
              return Promise.reject(error)
            }

            // Gọi API refresh token
            const response = await axios.post(`${API_BASE_URL}/auth/refresh-token`, {
              refreshToken: refreshToken
            })

            const { accessToken, refreshToken: newRefreshToken } = response.data.data

            // Lưu token mới
            localStorage.setItem('accessToken', accessToken)
            localStorage.setItem('refreshToken', newRefreshToken)

            // Retry request cũ với token mới
            originalRequest.headers.Authorization = `Bearer ${accessToken}`
            return apiClient(originalRequest)
          } catch (refreshError) {
            authStore.logout()
            return Promise.reject(refreshError)
          }
        }

        // Xử lý các lỗi khác
        if (error.response?.status === 403) {
          console.error('Forbidden: Bạn không có quyền truy cập tài nguyên này')
        }

        if (error.response?.status === 404) {
          console.error('Not Found: Tài nguyên không tồn tại')
        }

        if (error.response?.status === 500) {
          console.error('Server Error: Lỗi từ máy chủ')
        }

        return Promise.reject(error)
      }
    )

    export default apiClient

//compilot
