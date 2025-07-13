import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { authApi } from '@/lib/api'

interface User {
  id: string
  email: string
  role: string
  username: string
}

interface AuthState {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
  refreshToken: () => Promise<void>
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,

      login: async (email: string, password: string) => {
        try {
          const response = await authApi.login(email, password)
          const { user, token } = response.data

          set({
            user,
            token,
            isAuthenticated: true,
          })
        } catch (error: any) {
          throw new Error(error.response?.data?.message || 'Login failed')
        }
      },

      logout: () => {
        set({
          user: null,
          token: null,
          isAuthenticated: false,
        })
      },

      refreshToken: async () => {
        try {
          const currentToken = get().token
          if (!currentToken) return

          const response = await authApi.refreshToken(currentToken)
          const { token } = response.data

          set({ token })
        } catch (error) {
          // If refresh fails, logout
          get().logout()
        }
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        user: state.user,
        token: state.token,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
)
