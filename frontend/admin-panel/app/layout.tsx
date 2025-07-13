import './globals.css'
import { Providers } from './providers'

export const metadata = {
  title: 'Licensify Admin Panel',
  description: 'Administrative interface for Licensify license management system',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>
        <Providers>
          {children}
        </Providers>
      </body>
    </html>
  )
}
