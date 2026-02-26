import { useState } from 'react'
import AvailabilityOverlay from './AvailabilityOverlay'
import { useNavigate } from 'react-router-dom'
import { stopConnection } from '../services/signalRService'

function Topbar({ onAvailabilitySet }) {
  const [showAvailability, setShowAvailability] = useState(false)
  const navigate = useNavigate()
  const currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}')

  const handleClose = () => {
    setShowAvailability(false)
    if (onAvailabilitySet) onAvailabilitySet()
  }

  const handleLogout = () => {
    stopConnection()
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    navigate('/')
  }

  return (
    <>
      <div style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        padding: '10px 16px',
        width: '100%',
        boxSizing: 'border-box'
      }}>
        {/* Logout Button Left */}
        <button
          onClick={handleLogout}
          style={{
            border: 'none',
            background: 'none',
            fontSize: '20px',
            cursor: 'pointer',
            padding: '4px',
            color: '#aaa',
            display: 'flex',
            alignItems: 'center',
          }}
          title="Logout"
        >
          ↩
        </button>

        {/* User Name Center */}
        <div style={{ textAlign: 'center' }}>
          <div style={{ fontSize: '13px', fontWeight: 700, color: '#333' }}>
            {currentUser.name || 'User'}
          </div>
        </div>

        {/* Availability Button Right */}
        <button
          onClick={() => setShowAvailability(true)}
          style={{
            border: 'none',
            background: 'linear-gradient(135deg, #e0245e, #ff6b6b)',
            color: 'white',
            borderRadius: '12px',
            padding: '6px 14px',
            fontSize: '12px',
            fontWeight: 600,
            cursor: 'pointer',
            boxShadow: '0 2px 8px rgba(224,36,94,0.3)'
          }}
        >
          📅
        </button>

        {/* Overlay */}
        {showAvailability && (
          <AvailabilityOverlay onClose={handleClose} />
        )}
      </div>
    </>
  )
}

export default Topbar