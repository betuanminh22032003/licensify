'use client'

import { NextUIProvider } from '@nextui-org/react'
import { Toaster } from 'react-hot-toast'
import { ThemeProvider as NextThemesProvider } from 'next-themes'

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <NextUIProvider>
      <NextThemesProvider attribute="class" defaultTheme="light">
        {children}
        <Toaster position="top-right" />
      </NextThemesProvider>
    </NextUIProvider>
  )
}
