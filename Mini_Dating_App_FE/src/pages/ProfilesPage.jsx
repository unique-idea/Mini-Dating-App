import { useState, useEffect } from 'react'
import PhoneLayout from '../components/PhoneLayout'
import axiosClient from '../services/axiosClientService'

function ProfilesPage() {
  const [profiles, setProfiles] = useState([])
  const [currentIndex, setCurrentIndex] = useState(0)
  const [showDetail, setShowDetail] = useState(false)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [animation, setAnimation] = useState(null) 
  const [toast, setToast] = useState(null)

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const res = await axiosClient.get('/User/profiles')
        setProfiles(res.data)
      } catch (err) {
        setError('Failed to load profiles.')
      } finally {
        setLoading(false)
      }
    }
    fetchProfiles()
  }, [])

  const profile = profiles[currentIndex]
  const isEnd = !loading && currentIndex >= profiles.length

  const showToast = (msg, color) => {
    setToast({ msg, color })
    setTimeout(() => setToast(null), 2000)
  }

  const goNext = () => {
    setTimeout(() => {
      setShowDetail(false)
      setAnimation(null)
      setCurrentIndex(i => i + 1)
    }, 800)
  }

  const handleNext = () => {
    setAnimation('skip')
    goNext()
  }

  const handleLike = async () => {
    try {
      await axiosClient.post(`/LikeUser/like/${profile.userId}`)
      setAnimation('like-success')
      showToast('❤️ Liked!', '#e0245e')
      goNext()
    } catch (err) {
      setAnimation('like-fail')
      showToast('😕 Like failed, try again!', '#ff9800')
      setTimeout(() => setAnimation(null), 600)
    }
  }

  const getCardAnimation = () => {
    switch (animation) {
      case 'skip':         return 'swipeLeft 0.7s ease forwards'
      case 'like-success': return 'spinSwipe 1.7s ease forwards'
      case 'like-fail':    return 'none'
      default:             return 'none'
    }
  }

  if (loading) {
    return (
      <PhoneLayout>
        <div style={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: '12px', color: '#aaa' }}>
          <div className="spinner-border text-danger" />
          <p style={{ fontSize: '14px' }}>Still finding people for you... 👀</p>
        </div>
      </PhoneLayout>
    )
  }

  if (error) {
    return (
      <PhoneLayout>
        <div style={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: '12px' }}>
          <span style={{ fontSize: '27px', textAlign: 'center' }}>Oops we got some problems! ⚠️</span>
          <p style={{ fontSize: '14px', color: '#e0245e' }}>{error}</p>
        </div>
      </PhoneLayout>
    )
  }

  if (isEnd) {
    return (
      <PhoneLayout>
        <div style={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: '12px', color: '#aaa' }}>
          <span style={{ fontSize: '48px' }}>💘</span>
          <p style={{ fontWeight: 600, fontSize: '16px' }}>It seems you've reviewed all the profiles.</p>
          <p style={{ fontSize: '13px' }}>Please come back later. 😃</p>
        </div>
      </PhoneLayout>
    )
  }

  const genderIcon = profile.gender === 'Female' ? '👩' : profile.gender === 'Male' ? '👨' : '🧑'

  return (
    <PhoneLayout>
      <div style={{ height: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', padding: '24px', gap: '20px', position: 'relative' }}>

        {/* Toast notification */}
        {toast && (
          <div style={{
            position: 'absolute',
            top: '20px',
            left: '50%',
            transform: 'translateX(-50%)',
            background: toast.color,
            color: 'white',
            padding: '8px 20px',
            borderRadius: '20px',
            fontSize: '14px',
            fontWeight: 600,
            zIndex: 100,
            animation: 'likePopup 2s ease forwards',
            whiteSpace: 'nowrap',
            boxShadow: '0 4px 12px rgba(0,0,0,0.2)'
          }}>
            {toast.msg}
          </div>
        )}

        {/* Profile Card */}
        <div
          onClick={() => setShowDetail(v => !v)}
          style={{
            width: '73%',
            height: '90%',
            perspective: '1000px',
            borderRadius: '24px',
            background: 'linear-gradient(145deg, #f8f8f8, #ececec)',
            boxShadow: '0 8px 32px rgba(224,36,94,0.15), 0 2px 8px rgba(0,0,0,0.08)',
            border: '1.5px solid pink',
            position: 'relative',
            overflow: 'hidden',
            cursor: 'pointer',
            animation: getCardAnimation(),
          }}
        >
          <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '100px', userSelect: 'none' }}>
            {genderIcon}
          </div>

          <div style={{ position: 'absolute', bottom: 0, left: 0, right: 0, height: '45%', background: 'linear-gradient(to top, rgba(0,0,0,0.6), transparent)', borderRadius: '0 0 24px 24px' }} />

          <div style={{ position: 'absolute', bottom: '16px', left: '16px', right: '16px', color: 'white', textShadow: '0 1px 4px rgba(0,0,0,0.4)' }}>
            <div style={{ fontWeight: 700, fontSize: '22px' }}>{profile.name}, {profile.age}</div>
            <div style={{ fontSize: '13px', opacity: 0.85, marginTop: '4px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis', maxWidth: '60%'}}>
             {profile.bio}
            </div>
          </div>

          {showDetail && (
            <div style={{ position: 'absolute', inset: 0, background: 'rgba(0,0,0,0.85)', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', gap: '12px', color: 'white', padding: '28px', borderRadius: '24px' }}>
              <div style={{ fontSize: '64px' }}>{genderIcon}</div>
              <h4 style={{ margin: 0, fontWeight: 700 }}>{profile.name}, {profile.age}</h4>
              <div style={{ fontSize: '13px', opacity: 0.7, fontWeight: 500, textTransform: 'uppercase', letterSpacing: '1px' }}>{profile.gender}</div>
              <p style={{ textAlign: 'center', fontSize: '14px', opacity: 0.9, margin: 0, lineHeight: 1.6 , wordBreak: 'break-word', width: '90%'}}>
                {profile.bio || 'No bio provided.'}</p>
              <p style={{ fontSize: '12px', marginTop: '85px' }}>Tap to close</p>
            </div>
          )}
        </div>

        {/* Action Buttons */}
        <div style={{ width: '65%', display: 'flex', justifyContent: 'space-between', gap: '45px' }}>
          <button
            onClick={handleNext}
            style={{ borderRadius: '100%', border: '1.5px solid #eee', background: 'white', fontSize: '20px', cursor: 'pointer', boxShadow: '0 2px 8px rgba(0,0,0,0.5)' }}
            title="Skip"
          >❌</button>

          <button
            onClick={() => setShowDetail(v => !v)}
            style={{ borderRadius: '100%', border: '1.5px solid #eee', background: 'white', fontSize: '20px', cursor: 'pointer', boxShadow: '0 2px 8px rgba(0,0,0,0.5)' }}
            title="View Detail"
          >👁️</button>

          <button
            onClick={handleLike}
            style={{ borderRadius: '100%', border: 'none', background: 'linear-gradient(135deg, #e0245e, #ff6b6b)', fontSize: '20px', cursor: 'pointer', boxShadow: '0 4px 12px rgba(224,36,94,0.5)' }}
            title="Like"
          >❤️</button>
        </div>

      </div>
    </PhoneLayout>
  )
}

export default ProfilesPage