import axios from 'axios'

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5047'

// Create axios instance
export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  if (typeof window !== 'undefined') {
    const authData = localStorage.getItem('auth-storage')
    if (authData) {
      const { token } = JSON.parse(authData).state
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
    }
  }
  return config
})

// Response interceptor to handle token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true
      
      // Try to refresh token
      try {
        const authData = localStorage.getItem('auth-storage')
        if (authData) {
          const { token } = JSON.parse(authData).state
          if (token) {
            const response = await authApi.refreshToken(token)
            const newToken = response.data.token
            
            // Update stored token
            const updatedAuthData = { 
              ...JSON.parse(authData),
              state: { 
                ...JSON.parse(authData).state, 
                token: newToken 
              }
            }
            localStorage.setItem('auth-storage', JSON.stringify(updatedAuthData))
            
            // Retry original request with new token
            originalRequest.headers.Authorization = `Bearer ${newToken}`
            return api(originalRequest)
          }
        }
      } catch (refreshError) {
        // Refresh failed, redirect to login
        localStorage.removeItem('auth-storage')
        window.location.href = '/'
      }
    }
    
    return Promise.reject(error)
  }
)

// Auth API
export const authApi = {
  login: (email: string, password: string) =>
    api.post('/api/auth/login', { email, password }),
  
  refreshToken: (token: string) =>
    api.post('/api/auth/refresh', { token }),
  
  logout: () =>
    api.post('/api/auth/logout'),
  
  getCurrentUser: () =>
    api.get('/api/auth/me'),
}

// Products API
export const productsApi = {
  getAll: (page = 1, limit = 10, search?: string) =>
    api.get('/api/products', { params: { page, limit, search } }),
  
  getById: (id: string) =>
    api.get(`/api/products/${id}`),
  
  create: (product: any) =>
    api.post('/api/products', product),
  
  update: (id: string, product: any) =>
    api.put(`/api/products/${id}`, product),
  
  delete: (id: string) =>
    api.delete(`/api/products/${id}`),
}

// Licenses API
export const licensesApi = {
  getAll: (page = 1, limit = 10) =>
    api.get('/api/licenses', { params: { page, limit } }),
  
  getById: (id: string) =>
    api.get(`/api/licenses/${id}`),
  
  getByKey: (licenseKey: string) =>
    api.get(`/api/licenses/by-key/${licenseKey}`),
  
  getByCustomer: (customerId: string) =>
    api.get(`/api/licenses/customer/${customerId}`),
  
  getByProduct: (productId: string) =>
    api.get(`/api/licenses/product/${productId}`),
  
  getByStatus: (status: string) =>
    api.get(`/api/licenses/status/${status}`),
  
  getExpiring: (beforeDate?: string) =>
    api.get('/api/licenses/expiring', { params: { beforeDate } }),
  
  getExpired: () =>
    api.get('/api/licenses/expired'),
  
  create: (license: any) =>
    api.post('/api/licenses', license),
  
  activate: (id: string) =>
    api.put(`/api/licenses/${id}/activate`),
  
  suspend: (id: string, reason: string) =>
    api.put(`/api/licenses/${id}/suspend`, { reason }),
  
  revoke: (id: string, reason: string) =>
    api.put(`/api/licenses/${id}/revoke`, { reason }),
  
  extend: (id: string, newExpirationDate: string) =>
    api.put(`/api/licenses/${id}/extend`, { newExpirationDate }),
  
  validate: (licenseKey: string) =>
    api.post('/api/licenses/validate', { licenseKey }),
}

// Audit API
export const auditApi = {
  getAll: (skip = 0, take = 100) =>
    api.get('/api/auditlogs', { params: { skip, take } }),
  
  getById: (id: string) =>
    api.get(`/api/auditlogs/${id}`),
  
  getByEntity: (entityType: string, entityId: string) =>
    api.get(`/api/auditlogs/entity/${entityType}/${entityId}`),
  
  getByUser: (userId: string) =>
    api.get(`/api/auditlogs/user/${userId}`),
  
  getByAction: (action: string) =>
    api.get(`/api/auditlogs/action/${action}`),
  
  getByDateRange: (from: string, to: string) =>
    api.get('/api/auditlogs/date-range', { params: { from, to } }),
  
  getBySeverity: (severity: string) =>
    api.get(`/api/auditlogs/severity/${severity}`),
  
  create: (auditLog: any) =>
    api.post('/api/auditlogs', auditLog),
}
