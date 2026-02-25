import { useState, useEffect } from 'react'
import axiosClient from '../services/axiosClientService'

const ALL_DAYS = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']

function getMonday(date) {
  const d = new Date(date)
  const day = d.getDay()
  const diff = day === 0 ? -6 : 1 - day
  d.setDate(d.getDate() + diff)
  d.setHours(0, 0, 0, 0)
  return d
}

function formatDate(date) {
  return date.toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit' })
}

function formatHeaderDate(date) {
  return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

function getDateKey(date) {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

// Generate days from tomorow to 3 weeks ahead
function generateDaysFromToday() {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  tomorrow.setHours(0, 0, 0, 0)

  const days = []
  for (let i = 0; i < 21; i++) {
    const date = new Date(tomorrow)
    date.setDate(tomorrow.getDate() + i)
    const dayName = ALL_DAYS[date.getDay() === 0 ? 6 : date.getDay() - 1]
    days.push({ day: dayName, date })
  }
  return days
}

// Group days into weeks
function groupIntoWeeks(days) {
  const weeks = []
  for (let i = 0; i < days.length; i += 7) {
    weeks.push(days.slice(i, i + 7))
  }
  return weeks
}

function AvailabilityOverlay({ onClose }) {
  const [mode, setMode] = useState('loading') 
  const [existingAvailability, setExistingAvailability] = useState([])
  const [weekOffset, setWeekOffset] = useState(0)
  const [availability, setAvailability] = useState({})
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  const allDays = generateDaysFromToday()
  const weeks = groupIntoWeeks(allDays)
  const currentWeekDays = weeks[weekOffset] || []

  useEffect(() => {
    const fetchAvailability = async () => {
      try {
        const res = await axiosClient.get('/Availability')
        if (res.data && res.data.length > 0) {
          setExistingAvailability(res.data)
          setMode('view')
        } else {
          setMode('create')
        }
      } catch {
        setMode('create')
      }
    }
    fetchAvailability()
  }, [])

  const handleTimeChange = (dateKey, field, value) => {
    setAvailability(prev => ({
      ...prev,
      [dateKey]: { ...prev[dateKey], [field]: value }
    }))
  }

  const handleSend = async () => {
    const payload = Object.entries(availability)
      .filter(([, slot]) => slot.startTime && slot.endTime)
      .map(([date, slot]) => ({
        date,
        startTime: slot.startTime,
        endTime: slot.endTime
      }))
     
    if (payload.length === 0) {
      setError('Please select at least one time slot!')
      return
    }

    setLoading(true)
    setError('')
    try {
      await axiosClient.post('/Availability/setting', payload)
      setSuccess('Availability saved! 🎉')
      setTimeout(() => onClose(), 2500)
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save availability.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <>
      {/* Backdrop */}
      <div onClick={onClose} style={{
        position: 'absolute', inset: 0,
        background: 'rgba(0,0,0,0.5)',
        zIndex: 200, borderRadius: '24px'
      }} />

      {/* Panel */}
      <div style={{
        position: 'absolute', bottom: 0, left: 0, right: 0,
        zIndex: 201, backgroundColor: 'white',
        borderRadius: '24px 24px 0 0',
        padding: '20px', maxHeight: '85%',
        display: 'flex', flexDirection: 'column', gap: '14px',
        boxShadow: '0 -4px 24px rgba(0,0,0,0.15)'
      }}>

        {/* Handle */}
        <div style={{
          width: '40px', height: '4px', background: '#eee',
          borderRadius: '2px', alignSelf: 'center'
        }} />

        {/* Title row */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h6 style={{ margin: 0, fontWeight: 700 }}>📅 Availability</h6>
          <button onClick={onClose} style={{
            border: 'none', background: 'none',
            fontSize: '18px', cursor: 'pointer', color: '#aaa'
          }}>✕</button>
        </div>

        {/* Loading */}
        {mode === 'loading' && (
          <div style={{ textAlign: 'center', padding: '40px', color: '#aaa' }}>
            <div className="spinner-border spinner-border-sm text-danger" />
            <p style={{ marginTop: '8px', fontSize: '13px' }}>Loading...</p>
          </div>
        )}

        {/* View mode */}
        {mode === 'view' && (
          <>
            <p style={{ fontSize: '13px', color: '#555', margin: 0 }}>
              Your current availability:
            </p>
            <div style={{ overflowY: 'auto', flex: 1, display: 'flex', flexDirection: 'column', gap: '8px' }}>
              {existingAvailability.map((slot, i) => {
                const date = new Date(slot.date)
                const dayName = ALL_DAYS[date.getDay() === 0 ? 6 : date.getDay() - 1]
                return (
                  <div key={i} style={{
                    display: 'flex', alignItems: 'center',
                    justifyContent: 'space-between',
                    padding: '10px 14px', borderRadius: '12px',
                    background: '#fafafa', border: '1px solid #f0f0f0'
                  }}>
                    <div>
                      <div style={{ fontWeight: 600, fontSize: '13px' }}>{dayName}</div>
                      <div style={{ fontSize: '11px', color: '#aaa' }}>{formatDate(date)}</div>
                    </div>
                    <div style={{
                      fontSize: '12px', color: '#e0245e', fontWeight: 600,
                      background: '#fce4ec', padding: '4px 10px', borderRadius: '20px'
                    }}>
                      {slot.startTime?.slice(0, 5)} – {slot.endTime?.slice(0, 5)}
                    </div>
                  </div>
                )
              })}
            </div>
            <button
              onClick={() => { setMode('create'); setWeekOffset(0) }}
              style={{
                width: '100%', padding: '14px', borderRadius: '14px',
                border: 'none',
                background: 'linear-gradient(135deg, #e0245e, #ff6b6b)',
                color: 'white', fontWeight: 700, fontSize: '14px',
                cursor: 'pointer', boxShadow: '0 4px 12px rgba(224,36,94,0.3)'
              }}
            >
              📝 Set New Availability
            </button>
          </>
        )}

        {/* Create mode */}
        {mode === 'create' && (
          <>
            {/* Week navigation */}
            <div style={{
              display: 'flex', alignItems: 'center', justifyContent: 'space-between',
              background: '#fafafa', borderRadius: '14px',
              padding: '10px 14px', border: '1px solid #f0f0f0'
            }}>
              <button
                onClick={() => setWeekOffset(w => w - 1)}
                disabled={weekOffset === 0}
                style={{
                  border: 'none',
                  background: weekOffset === 0 ? '#f5f5f5' : 'linear-gradient(135deg, #e0245e, #ff6b6b)',
                  color: weekOffset === 0 ? '#ccc' : 'white',
                  borderRadius: '10px', width: '32px', height: '32px',
                  cursor: weekOffset === 0 ? 'not-allowed' : 'pointer',
                  fontWeight: 700, fontSize: '16px'
                }}
              >‹</button>

              <div style={{ textAlign: 'center' }}>
                <div style={{ fontWeight: 700, fontSize: '14px', color: '#333' }}>
                  {formatHeaderDate(currentWeekDays[0]?.date)}
                </div>
                <div style={{ fontSize: '11px', color: '#e0245e', fontWeight: 600, marginTop: '2px' }}>
                  Week {weekOffset + 1} of {weeks.length}
                </div>
              </div>

              <button
                onClick={() => setWeekOffset(w => w + 1)}
                disabled={weekOffset === weeks.length - 1}
                style={{
                  border: 'none',
                  background: weekOffset === weeks.length - 1 ? '#f5f5f5' : 'linear-gradient(135deg, #e0245e, #ff6b6b)',
                  color: weekOffset === weeks.length - 1 ? '#ccc' : 'white',
                  borderRadius: '10px', width: '32px', height: '32px',
                  cursor: weekOffset === weeks.length - 1 ? 'not-allowed' : 'pointer',
                  fontWeight: 700, fontSize: '16px'
                }}
              >›</button>
            </div>

            {/* Day list */}
            <div style={{ overflowY: 'auto', flex: 1, display: 'flex', flexDirection: 'column', gap: '10px' }}>
              {currentWeekDays.map(({ day, date }) => {
                const dateKey = getDateKey(date)
                const slot = availability[dateKey] || {}
                return (
                  <div key={dateKey} style={{
                    display: 'flex', alignItems: 'center',
                    justifyContent: 'space-between',
                    padding: '10px 12px', borderRadius: '12px',
                    background: 'white', border: '1px solid #f0f0f0', gap: '8px'
                  }}>
                    <div style={{ minWidth: '95px' }}>
                      <div style={{ fontWeight: 600, fontSize: '13px', color: '#333' }}>{day}</div>
                      <div style={{ fontSize: '11px', color: '#aaa' }}>{formatDate(date)}</div>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
                      <input
                        type="time"
                        value={slot.startTime || ''}
                        onChange={e => handleTimeChange(dateKey, 'startTime', e.target.value)}
                        style={{
                          border: '1px solid #eee', borderRadius: '8px',
                          padding: '4px 6px', fontSize: '12px',
                          color: '#333', outline: 'none', width: '88px'
                        }}
                      />
                      <span style={{ fontSize: '11px', color: '#aaa' }}>to</span>
                      <input
                        type="time"
                        value={slot.endTime || ''}
                        onChange={e => handleTimeChange(dateKey, 'endTime', e.target.value)}
                        style={{
                          border: '1px solid #eee', borderRadius: '8px',
                          padding: '4px 6px', fontSize: '12px',
                          color: '#333', outline: 'none', width: '88px'
                        }}
                      />
                    </div>
                  </div>
                )
              })}
            </div>

            {error && <div style={{ fontSize: '12px', color: '#e0245e', textAlign: 'center' }}>{error}</div>}
            {success && <div style={{ fontSize: '12px', color: '#28a745', textAlign: 'center' }}>{success}</div>}

            <button
              onClick={handleSend}
              disabled={loading}
              style={{
                width: '100%', padding: '14px', borderRadius: '14px',
                border: 'none',
                background: 'linear-gradient(135deg, #e0245e, #ff6b6b)',
                color: 'white', fontWeight: 700, fontSize: '14px',
                cursor: 'pointer', boxShadow: '0 4px 12px rgba(224,36,94,0.3)'
              }}
            >
              {loading ? <span className="spinner-border spinner-border-sm" /> : 'Send Availability'}
            </button>
          </>
        )}
      </div>
    </>
  )
}

export default AvailabilityOverlay