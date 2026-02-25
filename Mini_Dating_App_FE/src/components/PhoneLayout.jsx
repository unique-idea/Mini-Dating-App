import Navbar from './Navbar'
import Topbar from './Topbar'


function PhoneLayout({ children, onAvailabilitySet }) {
  return (
    <div style={{
      minHeight: '100vh',
      backgroundColor: '#f0f2f5',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '20px'
    }}>
      <div style={{
        width: '100%',
        maxWidth: '420px',
        height: '580px',
        backgroundColor: 'white',
        borderRadius: '24px',
        boxShadow: '0 0 40px rgba(0,0,0,0.15)',
        overflow: 'hidden',
        display: 'flex',
        flexDirection: 'column',
        position: 'relative'
      }}>
        
        {/* Top navbar */}
        <Topbar onAvailabilitySet={onAvailabilitySet} />

        {/* Page content */}
        <div style={{ flex: 1, overflowY: 'auto', paddingBottom: '65px' }}>
          {children}
        </div>

        {/* Bottom navbar */}
        <Navbar />
      </div>
    </div>
  )
}

export default PhoneLayout