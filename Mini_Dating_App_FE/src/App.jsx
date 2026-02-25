import { BrowserRouter, Routes, Route, useLocation, Navigate } from 'react-router-dom'
import MainPage from './pages/MainPage'
import ProfilesPage from './pages/ProfilesPage'
import MatchesPage from './pages/MatchesPage'
import Navbar from './components/Navbar'
import { useEffect } from 'react'
import { startConnection, joinUserGroup } from './services/signalRService'

function ProtectedRoute({ children }) {
  const token = localStorage.getItem('token')
  if (!token) return <Navigate to="/" replace />


  return children
}

function AppRoutes() {
  const location = useLocation()

  return (
    <Routes>
      <Route path="/" element={<MainPage />} />
      <Route path="/profiles" element={
        <ProtectedRoute><ProfilesPage key={location.key} /></ProtectedRoute>
      } />
      {/* <Route path="/likes" element={
        <ProtectedRoute><LikesPage /></ProtectedRoute>
      } /> */}
      <Route path="/matches" element={
        <ProtectedRoute><MatchesPage key={location.key} /></ProtectedRoute>
      } />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

function App() {
  useEffect(() => {
    const token = localStorage.getItem('token')
    const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}')

    if (token && currentUser.userId) {
      startConnection().then(() => joinUserGroup(currentUser.userId))
    }
  }, [])

  return (
    <BrowserRouter>
      <AppRoutes />
    </BrowserRouter>
  )
}

export default App