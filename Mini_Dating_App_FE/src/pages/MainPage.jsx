import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import axiosClient from '../services/axiosClientService'
import { startConnection, joinUserGroup } from '../services/signalRService'

function MainPage() {
  const navigate = useNavigate()
  const [activeTab, setActiveTab] = useState('login')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [loginForm, setLoginForm] = useState({ email: '' })
  const [registerForm, setRegisterForm] = useState({
    name: '', age: '', gender: '', bio: '', email: ''
  })

  const handleLogin = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const res = await axiosClient.post('/User/login', { email: loginForm.email })
      localStorage.setItem('token', res.data.token)
      localStorage.setItem('currentUser', JSON.stringify(res.data.user))

      await startConnection()
      await joinUserGroup(res.data.user.userId)

      navigate('/profiles')
    } catch (err) {
      setError(err.response?.data?.message || 'Login failed! plese try again later :<')
    } finally {
      setLoading(false)
    }
  }

  const handleRegister = async (e) => {
    e.preventDefault()
    setError('')
    setSuccess('')
    setLoading(true)
    try {
      await axiosClient.post('/User/register', {
        name: registerForm.name,
        age: parseInt(registerForm.age),
        gender: parseInt(registerForm.gender),
        bio: registerForm.bio,
        email: registerForm.email
      })
      setSuccess('Account created! Please enjoy. ❤️')
      setActiveTab('login')
      setLoginForm({ email: registerForm.email })
    } catch (err) {
      setError(err.response?.data?.message || 'Register failed! plese try again :<')
    } finally {
      setLoading(false)
    }
  }

  return (
    // Page background
    <div style={{
      minHeight: '100vh',
      width: '100vw',
      backgroundColor: '#f0f2f5',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '20px'
    }}>
      {/* Phone frame */}
      <div style={{
        width: '100%',
        maxWidth: '420px',
        minHeight: '100vh',
        maxHeight: '900px',
        backgroundColor: 'white',
        borderRadius: '24px',
        boxShadow: '0 0 40px rgba(0,0,0,0.5)',
        overflow: 'hidden',
        display: 'flex',
        flexDirection: 'column'
      }}>
        {/* Header */}
        <div style={{
          background: 'linear-gradient(135deg, #e0245e, #ff6b6b)',
          padding: '40px 24px 30px',
          textAlign: 'center',
          color: 'white'
        }}>
          <div style={{ fontSize: '48px' }}>💘</div>
          <h4 style={{margin: '8px 0 4px',fontWeight: 700,textShadow: `0 0 7px #fff,0 0 10px #fff,0 0 21px #fff,0 0 42px #0fa,
      0 0 82px #0fa,0 0 92px #0fa,0 0 102px #0fa,0 0 151px #0fa`}}>Mini Dating App</h4>
          <p style={{ margin: 0, opacity: 0.85, fontSize: '14px' }}>Find your match</p>
        </div>

        {/* Content */}
        <div style={{ flex: 1, overflowY: 'auto', padding: '24px' }}>
          {/* Tabs */}
          <ul className="nav nav-tabs mb-4">
            <li className="nav-item" style={{ flex: 1 }}>
              <button
                className={`nav-link w-100 ${activeTab === 'login' ? 'active' : ''}`}
                onClick={() => { setActiveTab('login'); setError(''); setSuccess('') }}
              >
                Login
              </button>
            </li>
            <li className="nav-item" style={{ flex: 1 }}>
              <button
                className={`nav-link w-100 ${activeTab === 'register' ? 'active' : ''}`}
                onClick={() => { setActiveTab('register'); setError(''); setSuccess('') }}
              >
                Register
              </button>
            </li>
          </ul>

          {error && <div className="alert alert-danger py-2 small">{error}</div>}
          {success && <div className="alert alert-success py-2 small">{success}</div>}

          {/* Login Form */}
          {activeTab === 'login' && (
            <form onSubmit={handleLogin}>
              <div className="mb-3">
                <label className="form-label fw-semibold">Email</label>
                <input
                  type="email"
                  className="form-control"
                  placeholder="user01@example.com"
                  value={loginForm.email}
                  onChange={e => setLoginForm({ email: e.target.value })}
                  required
                />
              </div>
              <button
                className="btn w-100 text-white fw-semibold mt-2"
                style={{ background: 'linear-gradient(135deg, #e0245e, #ff6b6b)', border: 'none' }}
                disabled={loading}
              >
                {loading && <span className="spinner-border spinner-border-sm me-2" />}
                Login
              </button>
              <p className="text-center text-muted small mt-3">
                Don't have an account?{' '}
                <span
                  style={{ color: '#e0245e', cursor: 'pointer' }}
                  onClick={() => { setActiveTab('register'); setError('') }}
                >
                  Create one here!
                </span>
              </p>
            </form>
          )}

          {/* Register Form */}
          {activeTab === 'register' && (
            <form onSubmit={handleRegister}>
              <div className="mb-3">
                <label className="form-label fw-semibold">Email</label>
                <input
                  type="email"
                  className="form-control"
                  placeholder="Your email"
                  value={registerForm.email}
                  onChange={e => setRegisterForm({ ...registerForm, email: e.target.value })}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Name</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Your name"
                  value={registerForm.name}
                  onChange={e => setRegisterForm({ ...registerForm, name: e.target.value })}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Age</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Your age"
                  min="18" max="120"
                  value={registerForm.age}
                  onChange={e => setRegisterForm({ ...registerForm, age: e.target.value })}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Gender</label>
                <select
                  className="form-select"
                  value={registerForm.gender}
                  onChange={e => setRegisterForm({ ...registerForm, gender: e.target.value })}
                  required
                >
                  <option value="">Select gender</option>
                  <option value="1">Female</option>
                  <option value="2">Male</option>
                  <option value="3">Other</option>
                </select>
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Bio</label>
                <textarea
                  className="form-control"
                  placeholder="Tell something about yourself...  <3"
                  rows={3}
                  value={registerForm.bio}
                  onChange={e => setRegisterForm({ ...registerForm, bio: e.target.value })}
                />
              </div>
             
              <button
                className="btn w-100 text-white fw-semibold mt-2"
                style={{ background: 'linear-gradient(135deg, #28a745, #20c997)', border: 'none' }}
                disabled={loading}
              >
                {loading && <span className="spinner-border spinner-border-sm me-2" />}
                Create Account
              </button>
              <p className="text-center text-muted small mt-3">
                Already have an account?{' '}
                <span
                  style={{ color: '#e0245e', cursor: 'pointer' }}
                  onClick={() => { setActiveTab('login'); setError('') }}
                >
                  Login here!
                </span>
              </p>
            </form>
          )}
        </div>
      </div>
    </div>
  )
}

export default MainPage