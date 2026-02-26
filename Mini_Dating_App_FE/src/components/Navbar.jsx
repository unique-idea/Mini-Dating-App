import { useNavigate, useLocation } from 'react-router-dom'

const tabs = [
  { path: '/profiles', icon: '👥', label: 'Profiles' },
  { path: '/matches', icon: '💘', label: 'Matches' },
]

function Navbar({ hasMatchUpdate }) {
  const navigate = useNavigate()
  const location = useLocation()

  return (
    <div style={{
      position: 'absolute',
      bottom: 0, left: 0, right: 0,
      height: '65px',
      backgroundColor: 'white',
      borderTop: '1px solid #eee',
      display: 'flex',
      alignItems: 'center',
      zIndex: 100
    }}>
      {tabs.map(tab => {
        const isActive = location.pathname === tab.path
        return (
          <button
            key={tab.path}
            onClick={() => navigate(tab.path)}
            style={{
              flex: 1,
              height: '100%',
              border: 'none',
              background: 'none',
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              gap: '2px',
              cursor: 'pointer',
              color: isActive ? '#e0245e' : '#aaa',
              borderTop: isActive ? '2px solid #e0245e' : '2px solid transparent',
              filter: isActive ? 'drop-shadow(0 0 4px rgba(224,36,94,0.6))' : 'none',
              transition: 'all 0.25s ease',
              position: 'relative'
            }}
          >
            <span style={{ fontSize: '20px' }}>{tab.icon}</span>
            <span style={{
              fontSize: '11px',
              fontWeight: isActive ? 700 : 400,
              textShadow: isActive ? '0 0 8px rgba(224,36,94,0.7)' : 'none',
              transition: 'all 0.25s ease'
            }}>
              {tab.label}
            </span>

            {/* Pulsing dot for match updates */}
            {tab.path === '/matches' && hasMatchUpdate && (
              <div style={{
                position: 'absolute',
                top: '8px',
                right: '22px',
              }}>
                {/* outer ping ring */}
                <div style={{
                  position: 'absolute',
                  width: '12px',
                  height: '12px',
                  borderRadius: '50%',
                  background: 'rgba(224,36,94,0.4)',
                  animation: 'ping 1s ease-out infinite'
                }} />
                {/* inner solid dot */}
                <div style={{
                  width: '12px',
                  height: '12px',
                  borderRadius: '50%',
                  background: '#e0245e',
                }} />
              </div>
            )}
          </button>
        )
      })}
    </div>
  )
}

export default Navbar